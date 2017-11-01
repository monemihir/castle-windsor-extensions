// 
// This file is part of - Castle Windsor Extensions
// Copyright (C) 2017 Mihir Mone
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 2.1 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Extensions.ComponentActivator;
using Castle.Windsor.Extensions.Resolvers;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Util;

namespace Castle.Windsor.Extensions.Registration
{
  /// <summary>
  ///   A <see cref="ResolvableDependency" /> component model descriptor
  /// </summary>
  public class ResolvableDependencyDescriptor : AbstractPropertyDescriptor
  {
    private readonly ResolvableDependency[] m_dependencies;
    private IDictionary<string, PropertyInfo> m_implPropertyTypes;

    private IPropertyResolver m_resolver;
    private List<string> m_resolvableProperties;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="dependencies">A collection of properties</param>
    public ResolvableDependencyDescriptor(params ResolvableDependency[] dependencies)
    {
      m_dependencies = dependencies;
      m_resolvableProperties = new List<string>();
    }

    /// <summary>
    ///   Try to accept dependency as a public property
    /// </summary>
    /// <param name="model">Current component model</param>
    /// <param name="dependency">Dependency</param>
    /// <returns>True if success, else false</returns>
    /// <exception cref="ConfigurationProcessingException">If the config property referenced is not found</exception>
    protected bool TryAcceptAsProperty(ComponentModel model, ResolvableDependency dependency)
    {
      PropertySet propKey = model.Properties.FirstOrDefault(f => f.Property.Name.Equals(dependency.Name, StringComparison.OrdinalIgnoreCase));

      if (propKey == null)
        return false;

      if (propKey.Property.GetSetMethod() == null)
        return false;

      ParameterModelCollection collection = new ParameterModelCollection();

      if (dependency.Value != null)
        collection.Add(propKey.Property.Name, dependency.Value);
      else if (dependency.IsComponent)
        collection.Add(propKey.Property.Name, "${" + dependency.ConfigName + "}");
      else if (!m_resolver.CanResolve(dependency.ConfigName))
        throw new ConfigurationProcessingException("No config property with name '" + dependency.ConfigName + "' found");
      else
      {
        //collection.Add(propKey.Property.Name, m_resolver.GetConfig(dependency.ConfigName));
        model.CustomDependencies[propKey.Property.Name] = m_resolver.GetValue(dependency.Name, propKey.Property.PropertyType);
      }

      propKey.Dependency.Init(collection);
      m_resolvableProperties.Add(propKey.Property.Name);
      return true;
    }

    /// <summary>
    ///   Try to accept dependency as a constructor parameter
    /// </summary>
    /// <param name="model">Current component model</param>
    /// <param name="dependency">Dependency</param>
    /// <returns>True if success, else false</returns>
    protected bool TryAcceptAsParameter(ComponentModel model, ResolvableDependency dependency)
    {
      bool isParameter = model.Constructors.SelectMany(c => c.Dependencies).Any(f => f.DependencyKey == dependency.Name);
      if (!isParameter)
        return false;

      if (!string.IsNullOrEmpty(dependency.Value))
      {
        //AddParameter(model, dependency.Name, dependency.Value);
        model.Parameters.Add(dependency.Name, dependency.Value);
      }
      else if (dependency.IsComponent)
        return false;
      else if (!m_resolver.CanResolve(dependency.ConfigName))
        throw new ConfigurationProcessingException("No config property with name '" + dependency.ConfigName + "' found");
      else
      {
        //AddParameter(model, dependency.Name, m_resolver.GetConfig(dependency.ConfigName));
        model.Parameters.Add(dependency.Name, m_resolver.GetConfig(dependency.ConfigName));
      }

      return true;
    }

    /// <summary>
    ///   Select the appropriate constructor candidate
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Selected constructor candidate</returns>
    protected ConstructorCandidate SelectConstructorCandidate(ComponentModel model)
    {
      string[] paramNames = model.Parameters.Select(f => f.Name).ToArray();
      ConstructorCandidate selectedCandidate = null;

      foreach (ConstructorCandidate candidate in model.Constructors)
      {
        bool selected = candidate.Dependencies.All(dependency => paramNames.Contains(dependency.DependencyKey));

        if (!selected)
          continue;

        selectedCandidate = candidate;
        break;
      }

      return selectedCandidate;
    }

    #region Implementation of IComponentModelDescriptor

    /// <summary>
    ///   Contribute to component model before standard <see cref="IContributeComponentModelConstruction" /> run.
    /// </summary>
    /// <param name="kernel"></param>
    /// <param name="model"></param>
    public override void BuildComponentModel(IKernel kernel, ComponentModel model)
    {
      PropertyInfo[] props = model.Implementation.GetProperties();

      m_implPropertyTypes = props.ToDictionary(k => k.Name, v => v);

      PropertiesSubSystem subsystem = kernel.GetSubSystem<PropertiesSubSystem>(PropertiesSubSystem.SubSystemKey);
      m_resolver = subsystem.Resolver;
    }

    /// <summary>
    ///   Contribute to component model after standard <see cref="IContributeComponentModelConstruction" /> run.
    /// </summary>
    /// <param name="kernel"></param>
    /// <param name="model"></param>
    public override void ConfigureComponentModel(IKernel kernel, ComponentModel model)
    {
      //m_constructorCandidates = model.Constructors.Where(c => c.Dependencies.Length == m_properties.Length).ToArray();

      foreach (ResolvableDependency prop in m_dependencies)
      {
        bool isProperty = TryAcceptAsProperty(model, prop);
        bool isParameter = TryAcceptAsParameter(model, prop);

        if (prop.IsComponent)
        {
          //AddParameter(model, prop.Name, "${" + prop.ConfigName + "}");
          model.Parameters.Add(prop.Name, "${" + prop.ConfigName + "}");
          isParameter = true;
        }

        if (!isProperty && !isParameter)
          throw new ConfigurationProcessingException("No public settable property or constructor parameter with name similar to '" + prop.Name + "' found on " + model.Implementation.FullName);
      }

      ConstructorCandidate selectedCandidate = SelectConstructorCandidate(model);

      model.ExtendedProperties[Constants.ConstructorCandidateKey] = selectedCandidate;
      model.ExtendedProperties[Constants.ResolvablePublicPropertiesKey] = m_resolvableProperties.ToArray();
      model.CustomComponentActivator = typeof(PropertyResolvingComponentActivator);
    }

    #endregion Implementation of IComponentModelDescriptor
  }
}
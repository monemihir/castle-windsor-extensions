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
using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Extensions.Resolvers;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Util;

namespace Castle.Windsor.Extensions.Registration
{
  /// <summary>
  ///   A <see cref="ResolvableDependency" /> component model descriptor
  /// </summary>
  public class ResolvableDependencyDescriptor : IComponentModelDescriptor
  {
    private readonly ResolvableDependency[] m_properties;
    private IDictionary<string, PropertyInfo> m_implPropertyTypes;
    private IPropertyResolver m_resolver;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="properties">A collection of properties</param>
    public ResolvableDependencyDescriptor(params ResolvableDependency[] properties)
    {
      m_properties = properties;
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
      string propKey = m_implPropertyTypes.Keys.FirstOrDefault(f => f.Equals(dependency.Name, StringComparison.OrdinalIgnoreCase));

      if (string.IsNullOrWhiteSpace(propKey))
        return false;

      if (m_implPropertyTypes[propKey].GetSetMethod() == null)
        return false;

      if (dependency.Value != null)
      {
        model.CustomDependencies[propKey] = dependency.Value;
        return true;
      }

      if (!m_resolver.CanResolve(dependency.ConfigPropertyName))
        throw new ConfigurationProcessingException("No config property with name '" + dependency.ConfigPropertyName + "' found");

      Type propType = m_implPropertyTypes[propKey].PropertyType;

      object value = m_resolver.GetValue(dependency.ConfigPropertyName, propType);

      model.CustomDependencies[propKey] = value;
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

      if (!m_resolver.CanResolve(dependency.ConfigPropertyName))
        throw new ConfigurationProcessingException("No config property with name '" + dependency.ConfigPropertyName + "' found");

      IConfiguration configuration = model.Configuration.Children[Constants.ParamsConfigKey];
      if (configuration == null)
      {
        configuration = new MutableConfiguration(Constants.ParamsConfigKey);
        model.Configuration.Children.Add(configuration);
      }

      if (dependency.Value != null)
      {
        configuration.Children.Add(new MutableConfiguration(dependency.Name, dependency.Value.ToString()));
      }
      else
      {
        MutableConfiguration mutableConfiguration = new MutableConfiguration(dependency.Name);
        mutableConfiguration.Children.Add(m_resolver.GetConfig(dependency.ConfigPropertyName));
        configuration.Children.Add(mutableConfiguration);
      }

      return true;
    }

    #region Implementation of IComponentModelDescriptor

    /// <summary>
    ///   Contribute to component model before standard <see cref="IContributeComponentModelConstruction" /> run.
    /// </summary>
    /// <param name="kernel"></param>
    /// <param name="model"></param>
    public void BuildComponentModel(IKernel kernel, ComponentModel model)
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
    public void ConfigureComponentModel(IKernel kernel, ComponentModel model)
    {
      foreach (ResolvableDependency prop in m_properties)
      {
        bool isProperty = TryAcceptAsProperty(model, prop);
        bool isParameter = TryAcceptAsParameter(model, prop);

        if (!isProperty && !isParameter)
          throw new ConfigurationProcessingException("No public settable property or constructor parameter with name similar to '" + prop.Name + "' found on " + model.Implementation.FullName);
      }
    }

    #endregion Implementation of IComponentModelDescriptor
  }
}
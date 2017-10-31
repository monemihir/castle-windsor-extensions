// 
// This file is part of - Castle Windsor Extensions
// Copyright (C) 2016 Mihir Mone
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
    private IDictionary<string, Type> m_implPropertyTypes;
    private IPropertyResolver m_resolver;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="properties">A collection of properties</param>
    public ResolvableDependencyDescriptor(params ResolvableDependency[] properties)
    {
      m_properties = properties;
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

      m_implPropertyTypes = props.ToDictionary(k => k.Name, v => v.PropertyType);

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
        if (!m_resolver.CanResolve(prop.ConfigPropertyName))
          throw new ConfigurationProcessingException("No config property with name '" + prop.ConfigPropertyName + "' found");

        string propKey = m_implPropertyTypes.Keys.FirstOrDefault(f => f.Equals(prop.Name, StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrWhiteSpace(propKey))
          throw new ConfigurationProcessingException("No public property with name similar to '" + prop.Name + "' found on " + model.Implementation.FullName);

        Type propType = m_implPropertyTypes[propKey];

        object value = prop.Value ?? m_resolver.GetValue(prop.ConfigPropertyName, propType);
        
        model.CustomDependencies[propKey] = value;
      }
    }

    #endregion
  }
}
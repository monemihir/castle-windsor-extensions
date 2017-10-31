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
using System.Xml;
using Castle.Core.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor.Extensions.Processor;
using Castle.Windsor.Extensions.Util;

namespace Castle.Windsor.Extensions.Resolvers
{
  /// <summary>
  ///   Properties resolver
  /// </summary>
  public class PropertyResolver : IPropertyResolver
  {
    private readonly IResourceProcessor m_processor;
    private readonly IConversionManager m_converter;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="processor">Properties XML processor</param>
    /// <param name="converter">Current conversion manager</param>
    public PropertyResolver(IResourceProcessor processor, IConversionManager converter)
    {
      m_processor = processor;
      m_converter = converter;
    }

    #region GetProperty methods

    /// <summary>
    ///   Whether the resolver can resolve property with given name
    /// </summary>
    /// <param name="propertyName">Name of property to check</param>
    /// <returns>True if can resolve, else false</returns>
    public bool CanResolve(string propertyName)
    {
      return m_processor.GetProperty(propertyName) != null;
    }

    /// <summary>
    ///   Get property configuration
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <returns>Property configuration</returns>
    public IConfiguration GetConfig(string propertyName)
    {
      XmlElement property = m_processor.GetProperty(propertyName);

      IConfiguration config = PropertyDeserializer.Deserialize(property);

      // get the flag indicating whether we want to resolve relative paths
      // this flag is set by the RelativePathSubDependencyResolver when it 
      // gets registered as a sub dependency resolver with castle
      bool resolveRelativePaths = m_converter.Context.Kernel.GetSettingsSubSystem().ResolveRelativePaths;

      if (resolveRelativePaths)
        RelativePathUtil.ConvertPaths(config, null);

      return config;
    }

    /// <summary>
    ///   Get the property with given name
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <returns>Property value</returns>
    public string GetValue(string propertyName)
    {
      return GetValue<string>(propertyName);
    }

    /// <summary>
    ///   Get the property with given name
    /// </summary>
    /// <typeparam name="TTargetType">Expected type of the property value</typeparam>
    /// <param name="propertyName">Property name</param>
    /// <returns>Property value</returns>
    public TTargetType GetValue<TTargetType>(string propertyName)
    {
      TTargetType propertyValue = (TTargetType)GetValue(propertyName, typeof(TTargetType));

      return propertyValue;
    }

    /// <summary>
    ///   Get the property with given name and type
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="targetType">Expected type of the property value</param>
    /// <returns>Property value</returns>
    public virtual object GetValue(string propertyName, Type targetType)
    {
      IConfiguration config = GetConfig(propertyName);

      object propertyValue = m_converter.PerformConversion(config, targetType);

      return propertyValue;
    }

    #endregion GetProperty methods

    #region GetDependency methods

    /// <summary>
    ///   Get dependency where dependency name and configuration
    ///   property name are the same
    /// </summary>
    /// <typeparam name="TTargetType">Expected type of the property value</typeparam>
    /// <param name="name">Dependency/Property name</param>
    /// <returns>Component dependency</returns>
    public Dependency GetDependency<TTargetType>(string name)
    {
      return GetDependency<TTargetType>(name, name);
    }

    /// <summary>
    ///   Get dependency where dependency name and configuration
    ///   property name are the same
    /// </summary>
    /// <param name="name">Dependency/Property name</param>
    /// <param name="targetType">Expected type of the property value</param>
    /// <returns>Component dependency</returns>
    public Dependency GetDependency(string name, Type targetType)
    {
      return GetDependency(name, name, targetType);
    }

    /// <summary>
    ///   Get dependency
    /// </summary>
    /// <typeparam name="TTargetType">Expected type of the property value</typeparam>
    /// <param name="dependencyName">Dependency name</param>
    /// <param name="propertyName">Configuration property name</param>
    /// <returns>Component dependency</returns>
    public Dependency GetDependency<TTargetType>(string dependencyName, string propertyName)
    {
      return Dependency.OnValue(dependencyName, GetValue<TTargetType>(propertyName));
    }

    /// <summary>
    ///   Get dependency
    /// </summary>
    /// <param name="dependencyName">Dependency name</param>
    /// <param name="propertyName">Configuration property name</param>
    /// <param name="targetType">Expected type of the property value</param>
    /// <returns>Component dependency</returns>
    public Dependency GetDependency(string dependencyName, string propertyName, Type targetType)
    {
      return Dependency.OnValue(dependencyName, GetValue(propertyName, targetType));
    }

    /// <summary>
    ///   Tries to get dependencies based on the property names given. Note: This method assumes
    ///   that the property names from the configuration file and the parameter names
    ///   on class constructors are the same name.
    /// </summary>
    /// <typeparam name="TComponentType">Component type for which to get dependencies</typeparam>
    /// <param name="propertyNames">A collection of properties</param>
    /// <returns>
    ///   A collection of dependencies if property names are parsed successfully, else
    ///   an empty collection of dependencies
    /// </returns>
    public Dependency[] TryGetDependencies<TComponentType>(params string[] propertyNames)
    {
      if (propertyNames == null)
        throw new ArgumentNullException(nameof(propertyNames));

      Dictionary<string, string> mapping = propertyNames.ToDictionary(k => k, v => v);

      return TryGetDependencies<TComponentType>(mapping);
    }

    /// <summary>
    ///   Tries to get dependencies based on the parameterName:propertyName mapping given. Note the
    ///   parameter name is the name to match on a class constructor while property name is
    ///   the name to match in configuration
    /// </summary>
    /// <typeparam name="TComponentType">Component type for which to get dependencies</typeparam>
    /// <param name="mapping">
    ///   A mapping of class constructor parameter names and configuration
    ///   file property names.
    /// </param>
    /// <returns>
    ///   A collection of dependencies if mapping is parsed successfully, else an empty
    ///   collection of dependencies
    /// </returns>
    public Dependency[] TryGetDependencies<TComponentType>(IDictionary<string, string> mapping)
    {
      if (mapping == null)
        throw new ArgumentNullException(nameof(mapping));

      List<Dependency> parameters = new List<Dependency>();

      if (mapping.Count == 0)
        return parameters.ToArray();

      Type type = typeof(TComponentType);

      string[] propertyNames = mapping.Keys.ToArray();
      string[] parameterNames = mapping.Values.ToArray();

      ConstructorInfo[] ctors = type.GetConstructors();

      List<ConstructorInfo> matchedCtors =
        (from ctor in ctors
          let matchedParams = ctor.GetParameters()
            .Where(p => parameterNames.Contains(p.Name))
            .Select(p => p.Name)
          where parameterNames.Length == matchedParams.Count()
          select ctor).ToList();

      foreach (ConstructorInfo ctor in matchedCtors)
      {
        ParameterInfo[] paramArr = ctor.GetParameters();
        bool parametersOk = true;
        parameters.Clear();
        for (int i = 0; i < paramArr.Length; i++)
        {
          ParameterInfo param = paramArr[i];
          try
          {
            Dependency d = GetDependency(param.Name, propertyNames[i], param.ParameterType);

            parameters.Add(d);
          }
          catch (Exception)
          {
            parametersOk = false;
          }
        }

        if (parametersOk)
          break;
      }

      return parameters.ToArray();
    }

    #endregion GetDependency methods
  }
}
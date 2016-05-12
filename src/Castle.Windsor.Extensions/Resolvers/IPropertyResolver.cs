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

using System.Collections.Generic;
using Castle.Core.Configuration;
using Castle.MicroKernel.Registration;

namespace Castle.Windsor.Extensions.Resolvers
{
  /// <summary>
  ///   Generic interface that all property resolver must implement
  /// </summary>
  public interface IPropertyResolver
  {
    /// <summary>
    ///   Whether the resolver can resolve property with given name
    /// </summary>
    /// <param name="propertyName">Name of property to check</param>
    /// <returns>True if can resolve, else false</returns>
    bool CanResolve(string propertyName);

    /// <summary>
    ///   Get property configuration
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <returns>Property configuration</returns>
    IConfiguration GetConfig(string propertyName);

    /// <summary>
    ///   Get the property with given name
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <returns>Property value</returns>
    string GetValue(string propertyName);

    /// <summary>
    ///   Get the property with given name
    /// </summary>
    /// <typeparam name="TTargetType">Expected type of the property value</typeparam>
    /// <param name="propertyName">Property name</param>
    /// <returns>Property value</returns>
    TTargetType GetValue<TTargetType>(string propertyName);

    /// <summary>
    ///   Get dependency where dependency name and configuration
    ///   property name are the same
    /// </summary>
    /// <typeparam name="TTargetType">Expected type of the property value</typeparam>
    /// <param name="name">Dependency/Property name</param>
    /// <returns>Component dependency</returns>
    Dependency GetDependency<TTargetType>(string name);

    /// <summary>
    ///   Get dependency
    /// </summary>
    /// <typeparam name="TTargetType">Expected type of the property value</typeparam>
    /// <param name="dependencyName">Dependency name</param>
    /// <param name="propertyName">Configuration property name</param>
    /// <returns>Component dependency</returns>
    Dependency GetDependency<TTargetType>(string dependencyName, string propertyName);

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
    Dependency[] TryGetDependencies<TComponentType>(params string[] propertyNames);

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
    Dependency[] TryGetDependencies<TComponentType>(IDictionary<string, string> mapping);
  }
}
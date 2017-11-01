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

namespace Castle.Windsor.Extensions.Registration
{
  /// <summary>
  ///   A resolvable dependency
  /// </summary>
  public class ResolvableDependency
  {
    /// <summary>
    ///   Constructor
    /// </summary>
    protected ResolvableDependency()
    {
      // private ctor
    }

    /// <summary>
    ///   Name of the class property/ctor parameter
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    ///   Dependency value
    /// </summary>
    public string Value { get; protected set; }

    /// <summary>
    ///   Name of the configuration property/component
    /// </summary>
    public string ConfigName { get; protected set; }

    /// <summary>
    ///   Whether this dependency is a component or a basic property
    /// </summary>
    public bool IsComponent { get; private set; }

    /// <summary>
    ///   Create a dependency where the name of config file property and the constructor parameter/public property of the class
    ///   are the same
    /// </summary>
    /// <param name="name">Constructor parameter or public property name of the class</param>
    /// <returns>Created dependency</returns>
    public static ResolvableDependency WithName(string name)
    {
      return new ResolvableDependency
      {
        Name = name,
        ConfigName = name
      };
    }

    /// <summary>
    ///   Create a dependency with given constructor parameter/property name of the class and the related config file property
    ///   name
    /// </summary>
    /// <param name="name">Constructor parameter or public property name of the class</param>
    /// <param name="configPropertyName">Config file property name</param>
    /// <returns>Created dependency</returns>
    public static ResolvableDependency WithConfigProperty(string name, string configPropertyName)
    {
      return new ResolvableDependency
      {
        Name = name,
        ConfigName = configPropertyName
      };
    }

    /// <summary>
    ///   Create a dependency with name of constructor parameter/public property of class and a name of the castle component
    /// </summary>
    /// <param name="name">Constructor parameter or public property name of the class</param>
    /// <param name="componentName">Castle component name</param>
    /// <returns>Created dependency</returns>
    public static ResolvableDependency WithComponent(string name, string componentName)
    {
      return new ResolvableDependency
      {
        Name = name,
        ConfigName = componentName,
        IsComponent = true
      };
    }

    /// <summary>
    ///   Create a dependency with name of constructor parameter/public property of class and it's value
    /// </summary>
    /// <param name="name">Constructor parameter or public property name of the class</param>
    /// <param name="value">Dependency value</param>
    /// <returns>Created dependency</returns>
    public static ResolvableDependency WithValue(string name, string value)
    {
      return new ResolvableDependency
      {
        Name = name,
        Value = value
      };
    }
  }

  /// <summary>
  ///   A resolvable property
  /// </summary>
  [Obsolete("Use ResolvableDependency instead")]
  public class ResolvableProperty : ResolvableDependency
  {
    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="name">Name of class property/ctor parameter</param>
    /// <param name="configPropertyName">
    ///   [Optional] Name of config property. Defaults to same name as specified by the <see cref="M:name" /> parameter
    /// </param>
    /// <param name="value">
    ///   [Optional] Property value to be used. Overrides the IOC resolved value. Defaults to null which asks
    ///   the IOC to resolve the value
    /// </param>
    public ResolvableProperty(string name, string configPropertyName = null, string value = null)
    {
      Name = name;
      ConfigName = configPropertyName ?? name;
      Value = value;
    }
  }
}
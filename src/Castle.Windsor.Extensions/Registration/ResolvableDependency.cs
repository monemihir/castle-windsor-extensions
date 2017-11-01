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
    /// Constructor
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
    /// Whether this dependency is a component or a basic property
    /// </summary>
    public bool IsComponent { get; protected set; }

    public static ResolvableDependency WithName(string name)
    {
      return new ResolvableDependency
      {
        Name = name,
        ConfigName = name
      };
    }

    public static ResolvableDependency WithConfigProperty(string name, string configPropertyName)
    {
      return new ResolvableDependency
      {
        Name = name,
        ConfigName = configPropertyName
      };
    }

    public static ResolvableDependency WithComponent(string name, string componentName)
    {
      return new ResolvableDependency
      {
        Name = name,
        ConfigName = componentName,
        IsComponent = true
      };
    }

    public static ResolvableDependency WithValue(string name, string value)
    {
      return new ResolvableDependency
      {
        Name = name,
        Value = value
      };
    }

    ///// <summary>
    /////   Constructor
    ///// </summary>
    ///// <param name="name">Name of class property/ctor parameter</param>
    //public ResolvableDependency(string name)
    //  : this(name, null, null)
    //{
    //  // nothing to do
    //}

    ///// <summary>
    /////   Constructor
    ///// </summary>
    ///// <param name="name">Name of class property/ctor parameter</param>
    ///// <param name="value">Property value</param>
    //public ResolvableDependency(string name, object value)
    //  : this(name, null, value)
    //{
    //  // nothing to do
    //}

    ///// <summary>
    /////   Constructor
    ///// </summary>
    ///// <param name="name">Name of class property/ctor parameter</param>
    ///// <param name="configPropertyName">Name of config property.</param>
    ///// <param name="value">Property value to be used.</param>
    //public ResolvableDependency(string name, string configPropertyName, object value)
    //{
    //  Name = name;
    //  Value = value;
    //  ConfigPropertyName = string.IsNullOrWhiteSpace(configPropertyName) ? name : configPropertyName;
    //}
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
    ///   [Optional] Name of config property. Defaults to same name as <see cref="ResolvableDependency.Name" />
    /// </param>
    /// <param name="value">
    ///   [Optional] Property value to be used. Overrides the IOC resolved value. Defaults to null which asks
    ///   the IOC to resolve the value
    /// </param>
    public ResolvableProperty(string name, string configPropertyName = null, string value = null)
    {
      Name = name;
      ConfigName = configPropertyName;
      Value = value;
    }
  }
}
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
using Castle.Core.Configuration;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Extensions.Resolvers;

namespace Castle.Windsor.Extensions.Registration
{
  /// <summary>
  ///   A resolvable dependency
  /// </summary>
  public class ResolvableDependency
  {
    /// <summary>
    ///   Name of the class property/ctor parameter
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///   Dependency value
    /// </summary>
    public object Value { get; private set; }

    /// <summary>
    ///   Name of the configuration property
    /// </summary>
    public string ConfigPropertyName { get; private set; }

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="name">Name of class property/ctor parameter</param>
    /// <param name="configPropertyName">
    ///   [Optional] Name of config property. Defaults to same name as <see cref="Name" />
    /// </param>
    /// <param name="value">
    ///   [Optional] Property value to be used. Overrides the IOC resolved value. Defaults to null which asks
    ///   the IOC to resolve the value
    /// </param>
    public ResolvableDependency(string name, string configPropertyName = null, object value = null)
    {
      Name = name;
      Value = value;
      ConfigPropertyName = string.IsNullOrWhiteSpace(configPropertyName) ? name : configPropertyName;
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
    ///   [Optional] Name of config property. Defaults to same name as <see cref="ResolvableDependency.Name" />
    /// </param>
    /// <param name="value">
    ///   [Optional] Property value to be used. Overrides the IOC resolved value. Defaults to null which asks
    ///   the IOC to resolve the value
    /// </param>
    public ResolvableProperty(string name, string configPropertyName = null, object value = null)
      : base(name, configPropertyName, value)
    {
    }
  }
}
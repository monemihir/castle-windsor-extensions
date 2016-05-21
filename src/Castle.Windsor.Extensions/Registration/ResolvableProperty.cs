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

namespace Castle.Windsor.Extensions.Registration
{
  /// <summary>
  ///   A resolvable property
  /// </summary>
  public class ResolvableProperty
  {
    /// <summary>
    ///   Name of the class property
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///   Name of the configuration property
    /// </summary>
    public string ConfigPropertyName { get; private set; }

    /// <summary>
    ///   Constructor to create a <see cref="ResolvableProperty" /> where the name of the class property
    ///   is the same as the name of the config property
    /// </summary>
    /// <param name="name">Name of property</param>
    public ResolvableProperty(string name)
      : this(name, name)
    {
      // nothing to do here
    }

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="name">Name of class property</param>
    /// <param name="configPropertyName">Name of config property</param>
    public ResolvableProperty(string name, string configPropertyName)
    {
      Name = name;
      ConfigPropertyName = configPropertyName;
    }
  }
}
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

namespace Castle.Windsor.Extensions.Registration
{
  /// <summary>
  ///   A utility class for <see cref="ResolvableProperty" />
  /// </summary>
  public static class ResolvablePropertyUtil
  {
    /// <summary>
    ///   Create resolvable properties from an entity class
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity class</typeparam>
    /// <returns>Resolvable properties</returns>
    public static IEnumerable<ResolvableProperty> From<TEntity>() where TEntity : class
    {
      return From(typeof(TEntity));
    }

    /// <summary>
    ///   Create resolvable properties from an entity class
    /// </summary>
    /// <param name="entityType">Type of the entity class</param>
    /// <returns>Resolvable properties</returns>
    public static IEnumerable<ResolvableProperty> From(Type entityType)
    {
      if (entityType == null)
        throw new ArgumentNullException("entityType");

      return entityType.GetProperties().Select(f => new ResolvableProperty(f.Name, f.Name.ToLowerCamelcase()));
    }

    /// <summary>
    ///   Converts given string to a lower camelcase
    /// </summary>
    /// <param name="str">String to be converted</param>
    /// <returns>Converted string</returns>
    private static string ToLowerCamelcase(this string str)
    {
      return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
  }
}
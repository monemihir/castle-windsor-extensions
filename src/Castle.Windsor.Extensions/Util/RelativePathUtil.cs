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
using System.IO;
using System.Reflection;
using System.Web.Hosting;
using Castle.Core.Configuration;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Extensions.Entities;

namespace Castle.Windsor.Extensions.Util
{
  /// <summary>
  ///   Relative path utility functions
  /// </summary>
  public static class RelativePathUtil
  {
    /// <summary>
    ///   True path of the current executing assembly
    /// </summary>
    private static readonly string TruePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

    /// <summary>
    ///   Holds the supported conversion operations.
    /// </summary>
    private static readonly Dictionary<EPathType, Func<string, string>> PathConversions =
      new Dictionary<EPathType, Func<string, string>>
      {
        {EPathType.Absolute, path => path},
        {EPathType.Relative, path => Path.Combine(TruePath, path)},
        {EPathType.Virtual, HostingEnvironment.MapPath},
        {EPathType.Uri, path => new Uri(path).LocalPath}
      };

    /// <summary>
    ///   Recursively updates the configuration and all it's children by inspecting
    ///   the 'pathType' attribute of the config
    /// </summary>
    /// <param name="config">Configuration to be updated</param>
    /// <param name="type">Path type</param>
    public static void ConvertPaths(IConfiguration config, EPathType? type)
    {
      type = GetPathType(config) ?? type;

      if (type != null && !string.IsNullOrWhiteSpace(config.Value))
      {
        string newValue = PlatformHelper.ConvertPath(Path.GetFullPath(PathConversions[type.Value](config.Value)));

        MutableConfiguration cfg = (MutableConfiguration)config;
        cfg.Value = newValue;
      }

      foreach (IConfiguration subConfig in config.Children)
        ConvertPaths(subConfig, type);
    }

    /// <summary>
    ///   Get path type from the config
    /// </summary>
    /// <param name="config">Configuration to be processed to retrieve the path type</param>
    /// <returns>Path type if found, else <see cref="M:EPathType.Default" /></returns>
    /// <exception cref="ConfigurationProcessingException">
    ///   Thrown if the 'pathType' attribute
    ///   value is invalid
    /// </exception>
    public static EPathType? GetPathType(IConfiguration config)
    {
      string pathTypeAttributeValue = config.Attributes[Constants.PathTypeAttributeName];
      if (string.IsNullOrWhiteSpace(pathTypeAttributeValue))
        return null;

      EPathType type;
      if (!Enum.TryParse(pathTypeAttributeValue, true, out type))
        throw new ConfigurationProcessingException(string.Format("Configuration error: Invalid pathType value '{0}'", pathTypeAttributeValue));

      return type;
    }
  }
}
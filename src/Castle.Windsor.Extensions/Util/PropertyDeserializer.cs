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
using System.Xml;
using Castle.Core.Configuration;
using Castle.Core.Configuration.Xml;

namespace Castle.Windsor.Extensions.Util
{
  /// <summary>
  ///   Property deserializer
  /// </summary>
  public static class PropertyDeserializer
  {
    private static readonly List<string> SpecialNodes = new List<string> {"array", "list", "dictionary"};

    /// <summary>
    ///   Deserializer given property to configuration
    /// </summary>
    /// <param name="property">Property to deserialize</param>
    /// <returns>Property configuration</returns>
    public static IConfiguration Deserialize(XmlElement property)
    {
      IConfiguration rawConfig = XmlConfigurationDeserializer.GetDeserializedNode(property);
      IConfiguration processedConfig = null;

      if (rawConfig.Children.Count > 0)
      {
        IConfiguration firstChild = rawConfig.Children[0];
        string configName = firstChild.Name.ToLowerInvariant();
        if (SpecialNodes.Contains(configName))
        {
          processedConfig = new MutableConfiguration(rawConfig.Name, string.Empty);
          processedConfig.Attributes.Add(rawConfig.Attributes);
          processedConfig.Children.AddRange(firstChild.Children);
        }
      }

      processedConfig = processedConfig ?? rawConfig;

      return processedConfig;
    }
  }
}
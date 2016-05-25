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
using System.Xml;
using Castle.Core.Resource;
using Castle.Windsor.Configuration.Interpreters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Castle.Windsor.Extensions.Processor
{
  /// <summary>
  ///   A properties JSON processor
  /// </summary>
  public class PropertiesJsonProcessor : IResourceProcessor
  {
    private readonly IDictionary<string, XmlElement> m_properties;

    /// <summary>
    ///   Constructor
    /// </summary>
    public PropertiesJsonProcessor()
    {
      m_properties = new Dictionary<string, XmlElement>();
    }

    /// <summary>
    ///   Processes the JSON object
    /// </summary>
    /// <param name="jsonObject">JSON object to be processed</param>
    /// <returns>Parsed XML node</returns>
    private XmlNode ProcessInternal(JObject jsonObject)
    {
      string startTemplate = "<castle><properties></properties></castle>";
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(startTemplate);

      XmlNode propertiesNode = doc.SelectSingleNode("/castle/properties");

      foreach (var token in jsonObject)
      {
        XmlElement element = doc.CreateElement(token.Key);

        JArray arr = token.Value as JArray;

        if (arr == null)
        {
          element.InnerText = token.Value.ToObject<string>();
        }
        else
        {
          XmlElement arrayElement = doc.CreateElement("array");
          foreach (var item in arr)
          {
            XmlElement itemElement = doc.CreateElement("item");

            itemElement.InnerText = item.ToObject<string>();
            arrayElement.AppendChild(itemElement);
          }
          element.AppendChild(arrayElement);
        }

        m_properties[token.Key] = element;

        // ReSharper disable once PossibleNullReferenceException
        propertiesNode.AppendChild(element);
      }

      return doc.DocumentElement;
    }

    #region Implementation of IResourceProcessor

    /// <summary>
    ///   Process given resource
    /// </summary>
    /// <param name="resource">Resource to process</param>
    /// <returns>Resource processed to an XML node</returns>
    public XmlNode Process(IResource resource)
    {
      try
      {
        using (resource)
        {
          JObject jo;
          using (var stream = resource.GetStreamReader())
          {
            string json = stream.ReadToEnd();

            jo = JsonConvert.DeserializeObject<JObject>(json);
          }

          return ProcessInternal(jo);
        }
      }
      catch (Exception ex)
      {
        var message = string.Format("Error processing node resource {0}", resource);

        throw new ConfigurationProcessingException(message, ex);
      }
    }

    /// <summary>
    ///   Process given xml node
    /// </summary>
    /// <param name="node">Node to process</param>
    /// <returns>Processed node</returns>
    public XmlNode Process(XmlNode node)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    ///   Get property with given name
    /// </summary>
    /// <param name="name">Property name</param>
    /// <returns>Property as an XML element</returns>
    public XmlElement GetProperty(string name)
    {
      XmlElement e;
      if (!m_properties.TryGetValue(name, out e))
        return null;

      return e.CloneNode(true) as XmlElement;
    }

    #endregion
  }
}
/*
* This file is part of - Castle Windsor Extensions
* Copyright (C) 2015 Mihir Mone
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU Lesser General Public License as published by
* the Free Software Foundation, either version 2 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU Lesser General Public License for more details.
*
* You should have received a copy of the GNU Lesser General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.Xml;
using Castle.Core.Resource;
using Castle.MicroKernel.SubSystems.Resource;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Configuration.Interpreters.XmlProcessor;
using Castle.Windsor.Configuration.Interpreters.XmlProcessor.ElementProcessors;

namespace Castle.Windsor.Extensions.XmlProcessor
{
  /// <summary>
  /// Properties node XML processor
  /// </summary>
  public class PropertiesXmlProcessor
  {
    private readonly DefaultXmlProcessorEngine m_engine;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="environmentName">Environment name</param>
    /// <param name="resourceSubSystem">Resource subsystem</param>
    public PropertiesXmlProcessor(string environmentName, IResourceSubSystem resourceSubSystem)
    {
      m_engine = new DefaultXmlProcessorEngine(environmentName, resourceSubSystem);
      RegisterProcessors();
    }

    /// <summary>
    /// Add an element processor
    /// </summary>
    /// <param name="t">Type of the processor to be added</param>
    protected void AddElementProcessor(Type t)
    {
      m_engine.AddNodeProcessor(t);
    }

    /// <summary>
    /// Register default processors
    /// </summary>
    private void RegisterProcessors()
    {
      AddElementProcessor(typeof(IfElementProcessor));
      AddElementProcessor(typeof(DefineElementProcessor));
      AddElementProcessor(typeof(UndefElementProcessor));
      AddElementProcessor(typeof(ChooseElementProcessor));
      AddElementProcessor(typeof(PropertiesElementProcessor));
      AddElementProcessor(typeof(AttributesElementProcessor));
      AddElementProcessor(typeof(IncludeElementProcessor));
      AddElementProcessor(typeof(IfProcessingInstructionProcessor));
      AddElementProcessor(typeof(DefinedProcessingInstructionProcessor));
      AddElementProcessor(typeof(UndefProcessingInstructionProcessor));
      AddElementProcessor(typeof(DefaultTextNodeProcessor));
      AddElementProcessor(typeof(EvalProcessingInstructionProcessor));
      AddElementProcessor(typeof(UsingElementProcessor));
    }

    /// <summary>
    /// Process given xml node
    /// </summary>
    /// <param name="node">Node to process</param>
    /// <returns>Processed node</returns>
    public XmlNode Process(XmlNode node)
    {
      try
      {
        if (node.NodeType == XmlNodeType.Document)
        {
          node = ((XmlDocument)node).DocumentElement;
        }

        m_engine.DispatchProcessAll(new DefaultXmlProcessorNodeList(node));

        return node;
      }
      catch (ConfigurationProcessingException)
      {
        throw;
      }
      catch (Exception ex)
      {
        var message = node == null ?
          "Unable to process null node" :
          string.Format("Error processing node {0}, inner content {1}", node.Name, node.InnerXml);

        throw new ConfigurationProcessingException(message, ex);
      }
    }

    /// <summary>
    /// Process given resource
    /// </summary>
    /// <param name="resource">Resource to process</param>
    /// <returns>Resource processed to an XML node</returns>
    public XmlNode Process(IResource resource)
    {
      try
      {
        using (resource)
        {
          var doc = new XmlDocument();
          using (var stream = resource.GetStreamReader())
          {
            doc.Load(stream);
          }

          m_engine.PushResource(resource);

          var element = Process(doc.DocumentElement);

          m_engine.PopResource();

          return element;
        }
      }
      catch (ConfigurationProcessingException)
      {
        throw;
      }
      catch (Exception ex)
      {
        var message = string.Format("Error processing node resource {0}", resource);

        throw new ConfigurationProcessingException(message, ex);
      }
    }

    /// <summary>
    /// Get property with given name
    /// </summary>
    /// <param name="name">Property name</param>
    /// <returns>Property as an XML element</returns>
    public XmlElement GetProperty(string name)
    {
      return m_engine.GetProperty(name);
    }
  }
}
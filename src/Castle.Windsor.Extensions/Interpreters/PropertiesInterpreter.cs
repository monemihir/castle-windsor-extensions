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

using Castle.Core.Resource;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.MicroKernel.SubSystems.Resource;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Configuration.Interpreters.XmlProcessor;
using Castle.Windsor.Extensions.Resolvers;
using Castle.Windsor.Extensions.XmlProcessor;

namespace Castle.Windsor.Extensions.Interpreters
{
  /// <summary>
  ///   Properties interpreter which parses all properties in the given
  ///   castle xml configuration file
  /// </summary>
  public class PropertiesInterpreter : AbstractInterpreter
  {
    private bool m_processResourceCalled;
    private IPropertyResolver m_resolver;

    /// <summary>
    ///   Properties resolver
    /// </summary>
    public virtual IPropertyResolver Resolver
    {
      get
      {
        if (!m_processResourceCalled)
          throw new ConfigurationProcessingException("Properties file has not been processed yet. Have you missed calling PropertiesInterpreter.ProcessResource(IResource,IConfigurationStore,IKernel)");

        return m_resolver;
      }
    }

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="filename">Castle configuration file</param>
    public PropertiesInterpreter(string filename)
      : base(filename)
    {
      m_processResourceCalled = false;
    }

    #region XmlInterpreter overrides

    /// <summary>
    ///   Should obtain the contents from the resource,
    ///   interpret it and populate the <see cref="IConfigurationStore" />
    ///   accordingly.
    /// </summary>
    /// <param name="resource">Resource to process</param>
    /// <param name="store">Windsor configuration store</param>
    /// <param name="kernel">Windsor kernel</param>
    public override void ProcessResource(IResource resource, IConfigurationStore store, IKernel kernel)
    {
      try
      {
        var resourceSubSystem = kernel.GetSubSystem(SubSystemConstants.ResourceKey) as IResourceSubSystem;

        PropertiesXmlProcessor processor = new PropertiesXmlProcessor(EnvironmentName, resourceSubSystem);

        IConversionManager converter = kernel.GetConversionManager();
        processor.Process(resource);

        // setup the properties resolver
        m_resolver = new PropertyResolver(processor, converter);
        m_processResourceCalled = true;

        //Deserialize(element, store, m_converter);
      }
      catch (XmlProcessorException e)
      {
        throw new ConfigurationProcessingException("Unable to process xml resource.", e);
      }
    }

    //protected new static void Deserialize(XmlNode section, IConfigurationStore store, IConversionManager converter)
    //{
    //  foreach (XmlNode node in section)
    //  {
    //    if (XmlConfigurationDeserializer.IsTextNode(node))
    //    {
    //      throw new ConfigurationProcessingException(string.Format("{0} cannot contain text nodes", node.Name));
    //    }
    //    if (node.NodeType == XmlNodeType.Element)
    //    {
    //      DeserializeElement(node, store, converter);
    //    }
    //  }
    //}

    //protected void DeserializeElement(XmlNode node, IConfigurationStore store, IConversionManager converter)
    //{
    //  if(PropertiesNodeName.Equals(node.Name))
    //  {
    //    m_processor.Process(node);
    //  }
    //}

    #endregion XmlInterpreter overrides
  }
}
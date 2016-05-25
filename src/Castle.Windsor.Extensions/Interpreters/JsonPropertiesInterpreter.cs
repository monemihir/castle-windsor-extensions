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

using Castle.Core.Resource;
using Castle.MicroKernel;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Extensions.Processor;
using Castle.Windsor.Extensions.Resolvers;

namespace Castle.Windsor.Extensions.Interpreters
{
  /// <summary>
  ///   A JSON properties interpreter
  /// </summary>
  public class JsonPropertiesInterpreter : AbstractInterpreter, IPropertiesInterpreter
  {
    private IPropertyResolver m_resolver;
    private bool m_processResourceCalled;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="filename">JSON config file</param>
    public JsonPropertiesInterpreter(string filename)
      : base(filename)
    {
      // nothing to do here
    }

    #region Overrides of AbstractInterpreter

    /// <summary>
    ///   Should obtain the contents from the resource,
    ///   interpret it and populate the <see cref="T:Castle.MicroKernel.SubSystems.Configuration.IConfigurationStore" />
    ///   accordingly.
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="store"></param>
    /// <param name="kernel"></param>
    public override void ProcessResource(IResource resource, IConfigurationStore store, IKernel kernel)
    {
      PropertiesJsonProcessor processor = new PropertiesJsonProcessor();
      processor.Process(resource);

      IConversionManager converter = kernel.GetConversionManager();

      // setup the properties resolver
      Resolver = new PropertyResolver(processor, converter);
      m_processResourceCalled = true;
    }

    #endregion

    #region Implementation of IPropertiesInterpreter

    /// <summary>
    ///   Properties resolver
    /// </summary>
    public IPropertyResolver Resolver
    {
      get
      {
        if (!m_processResourceCalled)
          throw new ConfigurationProcessingException("Properties file has not been processed yet. Have you missed calling PropertiesInterpreter.ProcessResource(IResource,IConfigurationStore,IKernel)");

        return m_resolver;
      }
      protected set { m_resolver = value; }
    }

    #endregion
  }
}
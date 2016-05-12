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

using Castle.MicroKernel;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Extensions.Resolvers;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Test.Helpers;
using NUnit.Framework;

namespace Castle.Windsor.Extensions.Test.SubSystems
{
  /// <summary>
  ///   PropertySubSystem unit tests
  /// </summary>
  [TestFixture]
  public class PropertiesSubSystemTest
  {
    /// <summary>
    ///   Test that getter of PropertySubSystem.Resolver throws an exception if
    ///   the subsystem is not initialised
    /// </summary>
    [Test]
    public void Get_Resolver_Throws_Exception_If_SubSystem_Not_Initialised()
    {
      // arrange
      EmbeddedResourceUtil.ExportToPath("Castle.Windsor.Extensions.Test.data", "castle.config", System.IO.Path.GetTempPath());
      string path = System.IO.Path.GetTempPath() + "\\castle.config";
      PropertiesSubSystem subSystem = new PropertiesSubSystem(path);

      ConfigurationProcessingException expected = new ConfigurationProcessingException("Properties file has not been processed yet. Have you missed calling PropertiesInterpreter.ProcessResource(IResource,IConfigurationStore,IKernel)");
      ConfigurationProcessingException actual = null;

      // act
      try
      {
        IPropertyResolver resolver = subSystem.Resolver;
      }
      catch (ConfigurationProcessingException e)
      {
        actual = e;
      }

      // assert
      Assert.IsNotNull(actual);
      Assert.AreEqual(expected.Message, actual.Message);
    }

    /// <summary>
    ///   Test that getter of PropertySubSystem.Resolver does nont throw an exception
    ///   once the subsystem is initialised
    /// </summary>
    [Test]
    public void Get_Resolver_DoesNot_Throws_Exception_Once_SubSystem_Is_Initialised()
    {
      // arrange
      EmbeddedResourceUtil.ExportToPath("Castle.Windsor.Extensions.Test.data", "castle.config", System.IO.Path.GetTempPath());
      string path = System.IO.Path.GetTempPath() + "\\castle.config";
      PropertiesSubSystem subSystem = new PropertiesSubSystem(path);
      WindsorContainer container = new WindsorContainer();
      subSystem.Init((IKernelInternal)container.Kernel);

      KernelException actual = null;
      IPropertyResolver resolver = null;

      // act
      try
      {
        resolver = subSystem.Resolver;
      }
      catch (KernelException e)
      {
        actual = e;
      }

      // assert
      Assert.IsNull(actual);
      Assert.IsNotNull(resolver);
    }
  }
}
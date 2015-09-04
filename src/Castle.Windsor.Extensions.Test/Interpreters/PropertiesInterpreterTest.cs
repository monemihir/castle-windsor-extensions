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

using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Extensions.Interpreters;
using Castle.Windsor.Extensions.Resolvers;
using Castle.Windsor.Extensions.Test.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Castle.Windsor.Extensions.Test.Interpreters
{
  /// <summary>
  ///   PropertiesInterpreter unit test
  /// </summary>
  [TestClass]
  public class PropertiesInterpreterTest : TestBase
  {
    /// <summary>
    ///   Test that property getter of PropertiesInterpreter.Resolver throws an
    ///   exception if the interpreter resource has not been processed yet
    /// </summary>
    [TestMethod]
    public void Get_Resolver_Throws_Exception_If_ProcessResource_Was_Not_Called()
    {
      // arrange
      EmbeddedResourceUtil.ExportToPath("Castle.Windsor.Extensions.Test.data", "castle.config", TestContext.DeploymentDirectory);
      string path = TestContext.DeploymentDirectory + "\\castle.config";
      PropertiesInterpreter interpreter = new PropertiesInterpreter(path);

      ConfigurationProcessingException expected =
        new ConfigurationProcessingException("Properties file has not been processed yet. Have you missed calling PropertiesInterpreter.ProcessResource(IResource,IConfigurationStore,IKernel)");
      ConfigurationProcessingException actual = null;

      // act
      try
      {
        IPropertyResolver resolver = interpreter.Resolver;
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
    ///   Test that property getter of PropertiesInterpreter.Resolver does not throws an
    ///   exception if the interpreter resource has been processed
    /// </summary>
    [TestMethod]
    public void Get_Resolver_DoesNot_Throw_Exception_If_ProcessResource_Was_Called()
    {
      // arrange
      EmbeddedResourceUtil.ExportToPath("Castle.Windsor.Extensions.Test.data", "castle.config", TestContext.DeploymentDirectory);
      string path = TestContext.DeploymentDirectory + "\\castle.config";
      PropertiesInterpreter interpreter = new PropertiesInterpreter(path);
      WindsorContainer container = new WindsorContainer();
      interpreter.ProcessResource(interpreter.Source, container.Kernel.ConfigurationStore, container.Kernel);

      ConfigurationProcessingException actual = null;
      IPropertyResolver resolver = null;

      // act
      try
      {
        resolver = interpreter.Resolver;
      }
      catch (ConfigurationProcessingException e)
      {
        actual = e;
      }

      // assert
      Assert.IsNull(actual);
      Assert.IsNotNull(resolver);
    }
  }
}
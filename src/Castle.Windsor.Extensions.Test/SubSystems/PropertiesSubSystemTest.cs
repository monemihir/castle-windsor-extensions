﻿// 
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
using System.IO;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Extensions.Resolvers;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Test.Helpers;
using Castle.Windsor.Extensions.Util;
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
    ///   Test that <see cref="PropertiesSubSystem.FromAppConfig" /> creates a windsor installer
    ///   which can initialise the subsystem from the application config file
    /// </summary>
    [Test]
    public void FromAppConfig_Returns_Correct_Installer()
    {
      // arrange
      IWindsorContainer container = new WindsorContainer();
      IWindsorInstaller installer = PropertiesSubSystem.FromAppConfig();

      // act
      container.Install(installer);

      // assert
      PropertiesSubSystem subsystem = container.Kernel.GetSubSystem<PropertiesSubSystem>(PropertiesSubSystem.SubSystemKey);
      Assert.AreEqual("Mihir", subsystem.Resolver.GetValue("name"));
      Assert.AreEqual(31, subsystem.Resolver.GetValue<int>("age"));
    }

    /// <summary>
    ///   Test that getter of PropertySubSystem.Resolver throws an exception if
    ///   the subsystem is not initialised
    /// </summary>
    [Test]
    public void Get_Resolver_Throws_Exception_If_SubSystem_Not_Initialised()
    {
      // arrange
      string path = EmbeddedResourceUtil.ExportToPath("Castle.Windsor.Extensions.Test.data", "invalid-castle.config", Path.GetTempPath());
      PropertiesSubSystem subSystem = new PropertiesSubSystem(path);

      const string expectedMessagePrefix = "Error processing node castle";
      string actualMessage = string.Empty;
      IPropertyResolver resolver = null;

      // act
      try
      {
        resolver = subSystem.Resolver;
        WindsorContainer container = new WindsorContainer();
        container.Kernel.AddSubSystem(PropertiesSubSystem.SubSystemKey, subSystem);
      }
      catch (ConfigurationProcessingException e)
      {
        actualMessage = e.Message;
      }

      // assert
      Assert.IsNull(resolver);
      Assert.IsTrue(actualMessage.StartsWith(expectedMessagePrefix));
    }

    /// <summary>
    ///   Test that getter of PropertySubSystem.Resolver does nont throw an exception
    ///   once the subsystem is initialised
    /// </summary>
    [Test]
    public void Get_Resolver_DoesNot_Throws_Exception_Once_SubSystem_Is_Initialised()
    {
      // arrange
      string path = EmbeddedResourceUtil.ExportToPath("Castle.Windsor.Extensions.Test.data", "castle.config", Path.GetTempPath());

      PropertiesSubSystem subSystem = new PropertiesSubSystem(path);
      WindsorContainer container = new WindsorContainer();
      container.Kernel.AddSubSystem(PropertiesSubSystem.SubSystemKey, subSystem);

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

    /// <summary>
    /// Test that the sub system initialises properly from a JSON file
    /// </summary>
    [Test]
    public void SubSystem_From_Json_File_Initialises_Properly()
    {
      // arrange
      string path = EmbeddedResourceUtil.ExportToPath("Castle.Windsor.Extensions.Test.data", "castle.json", Path.GetTempPath());

      PropertiesSubSystem subsystem = new PropertiesSubSystem(path);
      WindsorContainer container = new WindsorContainer();

      // act
      subsystem.Init((IKernelInternal)container.Kernel);

      // assert
      Assert.AreEqual("mihir", subsystem.Resolver.GetValue("name"));
      CollectionAssert.AreEqual(new[] { "chess", "cricket" }, subsystem.Resolver.GetValue<string[]>("hobbies"));

      IDictionary<string, string> extrasStuffMixed = subsystem.Resolver.GetValue<Dictionary<string, string>>("extraStuffMixed");

      Assert.AreEqual("31", extrasStuffMixed["age"]);
      Assert.AreEqual("male", extrasStuffMixed["sex"]);

      IDictionary<string, double> extrasStuffFloats = subsystem.Resolver.GetValue<Dictionary<string, double>>("extraStuffFloats");

      Assert.AreEqual(4.0, extrasStuffFloats["precision"]);
      Assert.AreEqual(2.56, extrasStuffFloats["start"]);
    }
  }
}
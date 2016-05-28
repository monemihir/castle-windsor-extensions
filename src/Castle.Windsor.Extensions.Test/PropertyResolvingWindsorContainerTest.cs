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

using System.IO;
using Castle.Windsor.Extensions.Interpreters;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Test.Helpers;
using Castle.Windsor.Extensions.Util;
using NUnit.Framework;

namespace Castle.Windsor.Extensions.Test
{
  /// <summary>
  ///   <see cref="PropertyResolvingWindsorContainer" /> unit tests
  /// </summary>
  [TestFixture]
  public class PropertyResolvingWindsorContainerTest
  {
    /// <summary>
    ///   Test that the constructor intiailises the properties sub system internally
    /// </summary>
    [Test]
    public void Ctor_Test()
    {
      // arrange
      EmbeddedResourceUtil.ExportToPath("Castle.Windsor.Extensions.Test.data", "castle.config", Path.GetTempPath());
      string path = Path.GetTempPath() + "\\castle.config";

      // act
      PropertyResolvingWindsorContainer container = new PropertyResolvingWindsorContainer(path);

      // assert
      Assert.IsInstanceOf<PropertiesInterpreter>(container.Interpreter);

      // get properties subsystem from kernel
      PropertiesSubSystem subsystem = container.Kernel.GetSubSystem<PropertiesSubSystem>(PropertiesSubSystem.SubSystemKey);

      // assert
      Assert.IsNotNull(subsystem);
      Assert.AreEqual("this is string", subsystem.Resolver.GetValue("strParam"));
    }

    /// <summary>
    ///   Test that the default constructor which initialises the properties subsystem from
    ///   the app.config/web.config file works as expected
    /// </summary>
    [Test]
    public void Ctor_With_No_Parameters_Test()
    {
      // act
      PropertyResolvingWindsorContainer container = new PropertyResolvingWindsorContainer();

      // assert
      Assert.IsInstanceOf<PropertiesInterpreter>(container.Interpreter);

      // get properties subsystem from kernel
      PropertiesSubSystem subsystem = container.Kernel.GetSubSystem<PropertiesSubSystem>(PropertiesSubSystem.SubSystemKey);

      // assert
      Assert.IsNotNull(subsystem);
      Assert.AreEqual("Mihir", subsystem.Resolver.GetValue("name"));
    }
  }
}
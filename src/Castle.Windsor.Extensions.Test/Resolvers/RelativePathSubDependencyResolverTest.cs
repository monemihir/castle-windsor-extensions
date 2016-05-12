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
using System.IO;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Extensions.Resolvers;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Test.Helpers;
using NUnit.Framework;

namespace Castle.Windsor.Extensions.Test.Resolvers
{
  /// <summary>
  ///   RelativePathSubDependencyResolver unit tests
  /// </summary>
  [TestFixture]
  public class RelativePathSubDependencyResolverTest
  {
    private string m_truePath;
    private Func<string, string> m_getFullPath;

    /// <summary>
    ///   Test setup
    /// </summary>
    [SetUp]
    public void Initialise()
    {
      m_truePath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
      m_getFullPath = str => Path.GetFullPath(Path.Combine(m_truePath, str));
    }

    /// <summary>
    ///   Tests that the RelativePathSubDependencyResolver resolves string
    ///   and string collection dependencies as expected when the properties
    ///   resolver is not used
    /// </summary>
    [Test]
    public void RelativePathSubDependencyResolver_Resolves_RelativePaths_As_Expected_When_No_PropertiesResolver()
    {
      // arrange
      EmbeddedResourceUtil.ExportToPath("Castle.Windsor.Extensions.Test.data", "relpath-castle.config", Path.GetTempPath());

      string path = Path.GetTempPath() + "\\relpath-castle.config";

      WindsorContainer container = new WindsorContainer(path);
      container.Kernel.Resolver.AddSubResolver(new RelativePathSubDependencyResolver(container.Kernel));

      // act
      RelPathTestClass obj = container.Resolve<RelPathTestClass>();

      // assert
      Assert.IsNotNull(obj);
      Assert.AreEqual(m_getFullPath(@"..\etc\config.ini"), obj.PathParam);
      Assert.AreEqual(3, obj.PathArrParam.Length);
      Assert.AreEqual(m_getFullPath(@"..\etc\config1.ini"), obj.PathArrParam[0]);
      Assert.AreEqual(@"C:\temp.ini", obj.PathArrParam[1]);
      Assert.AreEqual(m_getFullPath(@"..\etc\second.ini"), obj.PathArrParam[2]);
    }

    /// <summary>
    ///   Tests that the RelativePathSubDependencyResolver resolves string
    ///   and string collection dependencies as expected wtih a properties
    ///   resolver registered
    /// </summary>
    [Test]
    public void RelativePathSubDependencyResolver_Resolves_RelativePaths_As_Expected_With_PropertiesResolver()
    {
      // arrange
      EmbeddedResourceUtil.ExportToPath("Castle.Windsor.Extensions.Test.data", "relpath-castle-with-propertiesresolver.config", Path.GetTempPath());

      string path = Path.GetTempPath() + "\\relpath-castle-with-propertiesresolver.config";

      PropertiesSubSystem subSystem = new PropertiesSubSystem(path);
      WindsorContainer container = new WindsorContainer();
      container.Kernel.AddSubSystem(PropertiesSubSystem.SubSystemKey, subSystem);
      container.Kernel.Resolver.AddSubResolver(new RelativePathSubDependencyResolver(container.Kernel));

      container.Register(Component
        .For<RelPathTestClass>()
        .DependsOn(
          subSystem.Resolver.GetDependency<string>("pathParam"),
          subSystem.Resolver.GetDependency<string[]>("pathArrParam")
        ));

      // act
      RelPathTestClass obj = container.Resolve<RelPathTestClass>();

      // assert
      Assert.IsNotNull(obj);
      Assert.AreEqual(m_getFullPath(@"..\etc\config.ini"), obj.PathParam);
      Assert.AreEqual(3, obj.PathArrParam.Length);
      Assert.AreEqual(m_getFullPath(@"..\etc\config1.ini"), obj.PathArrParam[0]);
      Assert.AreEqual(@"C:\temp.ini", obj.PathArrParam[1]);
      Assert.AreEqual(m_getFullPath(@"..\etc\second.ini"), obj.PathArrParam[2]);
    }
  }
}
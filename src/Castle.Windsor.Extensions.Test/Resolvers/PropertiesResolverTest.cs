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
using System.IO;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor.Extensions.Resolvers;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Test.Helpers;
using NUnit.Framework;

namespace Castle.Windsor.Extensions.Test.Resolvers
{
  /// <summary>
  ///   PropertiesSubSystem unit tests
  /// </summary>
  [TestFixture]
  public class PropertiesResolverTest
  {
    /// <summary>
    ///   Test that the GetValue method works as expected
    /// </summary>
    [Test]
    public void GetValue_Simple_Works_As_Expected()
    {
      // arrange
      string path = EmbeddedResourceUtil.ExportToPath("Castle.Windsor.Extensions.Test.data", "castle.config", Path.GetTempPath());

      PropertiesSubSystem subSystem = new PropertiesSubSystem(path);
      WindsorContainer container = new WindsorContainer();
      container.Kernel.AddSubSystem(PropertiesSubSystem.SubSystemKey, subSystem);

      IConversionManager manager = (IConversionManager)container.Kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
      manager.Add(new PersonTypeConverter());

      IPropertyResolver resolver = subSystem.Resolver;

      // act
      string strParam = resolver.GetValue("strParam");
      double dblParam = resolver.GetValue<double>("dblParam");
      string[] arrParam = resolver.GetValue<string[]>("arrParam");
      IDictionary<string, string> dictParam = resolver.GetValue<IDictionary<string, string>>("dictParam");
      List<double> listParam = resolver.GetValue<List<double>>("listParam");
      Person[] personArr = resolver.GetValue<Person[]>("personArr");

      // assert
      Assert.AreEqual("this is string", strParam);
      Assert.AreEqual(20.12, dblParam);
      Assert.AreEqual("arr item 1", arrParam[0]);
      Assert.AreEqual(strParam, arrParam[1]);
      Assert.AreEqual(@"..\etc\config.ini", arrParam[2]);
      Assert.IsTrue(dictParam.ContainsKey("key1"));
      Assert.IsTrue(dictParam.ContainsKey("key2"));
      Assert.AreEqual("value 1", dictParam["key1"]);
      Assert.AreEqual("value 2", dictParam["key2"]);
      Assert.AreEqual(21.2, listParam[0]);
      Assert.AreEqual(3, listParam[1]);
      Assert.AreEqual(dblParam, listParam[2]);
      Assert.AreEqual("Mihir", personArr[0].Name);
      Assert.AreEqual(30, personArr[0].PersonAge);
      Assert.AreEqual("Sneha", personArr[1].Name);
      Assert.AreEqual(33, personArr[1].PersonAge);

      // act
      Dependency[] dependencies = resolver.TryGetDependencies<TestClass>("strParam", "arrParam", "refParam", "dictParam", "listParam", "personArr");

      container.Register(Component.For<TestClass>().DependsOn(dependencies));

      // assert
      Assert.IsNotNull(container.Resolve<TestClass>());
    }
  }
}
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
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Extensions.Registration;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Test.Helpers;
using NUnit.Framework;

namespace Castle.Windsor.Extensions.Test.Registration
{
  /// <summary>
  ///   <see cref="PropertyResolvingComponent" /> unit tests
  /// </summary>
  [TestFixture]
  public class PropertyResolvingComponentTest
  {
    /// <summary>
    ///   Test that the For&lt;TService&gt; method returns the correct
    ///   component
    /// </summary>
    [Test]
    public void For_Method_Returns_Correct_Component()
    {
      // arrange
      IWindsorContainer container = new WindsorContainer();
      IWindsorInstaller installer = PropertiesSubSystem.FromAppConfig();
      container.Install(installer);

      Dictionary<string, string> mappings = new Dictionary<string, string>
      {
        {"name", "name"},
        {"age", "age"}
      };

      // act
      var component = PropertyResolvingComponent.For<ICanBePerson>()
        .ImplementedBy<Person>()
        .DependsOnConfigProperties(mappings)
        .WithLifestyle(LifestyleType.Transient);
      container.Register(component);

      // assert
      ICanBePerson person = container.Resolve<ICanBePerson>();
      Assert.IsInstanceOf<Person>(person);
    }
  }
}
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
using Castle.MicroKernel.Registration;
using Castle.Windsor.Extensions.Registration;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Test.Helpers;
using NUnit.Framework;

namespace Castle.Windsor.Extensions.Test.Registration
{
  /// <summary>
  ///   <see cref="PropertyResolvingComponentRegistration{TService}" /> unit tests
  /// </summary>
  [TestFixture]
  public class PropertyResolvingComponentRegistrationTest
  {
    private IWindsorContainer m_container;
    private IWindsorInstaller m_propertySubSystemInstaller;

    /// <summary>
    ///   Test setup
    /// </summary>
    [SetUp]
    public void Setup()
    {
      m_container = new WindsorContainer();
      m_propertySubSystemInstaller = PropertiesSubSystem.FromAppConfig();
      m_container.Install(m_propertySubSystemInstaller);
    }

    /// <summary>
    ///   Test that a component with parameter
    /// </summary>
    [Test]
    public void Component_With_Parameter_Dependencies_Test()
    {
      // arrange
      Dictionary<string, string> mappings = new Dictionary<string, string>
      {
        {"name", "name"},
        {"age", "age"}
      };

      var registration = new PropertyResolvingComponentRegistration<ICanBePerson>()
        .DependsOnConfigProperties(mappings)
        .ImplementedBy<Person>();

      // act
      m_container.Register(registration);

      // assert
      Person person = (Person)m_container.Resolve<ICanBePerson>();
      Assert.AreEqual("Mihir", person.Name);
      Assert.AreEqual(31, person.Age);
      Assert.IsNull(person.PlaceOfBirth);
    }

    /// <summary>
    ///   Test that a component with properties
    /// </summary>
    [Test]
    public void Component_With_Property_Dependencies_Test()
    {
      // arrange
      ResolvableProperty prop = new ResolvableProperty("placeOfBirth");

      var registration = new PropertyResolvingComponentRegistration<ICanBePerson>()
        .DependsOnProperties(prop)
        .ImplementedBy<Person>();

      // act
      m_container.Register(registration);

      // assert
      Person person = (Person)m_container.Resolve<ICanBePerson>();
      Assert.IsNull(person.Name);
      Assert.AreEqual(0, person.Age);
      Assert.AreEqual("Pune", person.PlaceOfBirth);
    }
  }
}
// 
// This file is part of - Castle Windsor Extensions
// Copyright (C) 2017 Mihir Mone
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
using System.Linq;
using Castle.Core;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Extensions.Registration;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Test.Helpers;
using NUnit.Framework;

#pragma warning disable 618

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
    public void Component_With_Parameter_Dependencies_Resolves_As_Expected()
    {
      // arrange
      Dictionary<string, string> mappings = new Dictionary<string, string>
      {
        {"name", "name"},
        {"age", "age"}
      };

      var registration = new PropertyResolvingComponentRegistration<ICanBePerson>()
        .DependsOn(mappings)
        .ImplementedBy<Person>();

      // act
      m_container.Register(registration);

      // assert
      Person person = (Person)m_container.Resolve<ICanBePerson>();
      Assert.AreEqual("Mihir", person.Name);
      Assert.AreEqual(31, person.PersonAge);
      Assert.IsNull(person.PlaceOfBirth);
    }

    /// <summary>
    ///   Test that a component with parameter but with no property with same name as parameter
    /// </summary>
    [Test]
    public void Component_With_Parameter_Dependencies_And_ServiceOverride_Resolves_As_Expected()
    {
      // arrange
      var motherRegistration = new PropertyResolvingComponentRegistration<ICanBePerson>()
        .DependsOn(
          ResolvableDependency.WithValue("name", "Snehal"),
          ResolvableDependency.WithValue("age", "59"),
          ResolvableDependency.WithName("yearOfBirth"))
        .WithName("snehal")
        .ImplementedBy<Person>();

      var spouseRegistration = new PropertyResolvingComponentRegistration<ICanBePerson>()
        .DependsOn(
          ResolvableDependency.WithValue("name", "Akanksha"),
          ResolvableDependency.WithValue("age", "30"),
          ResolvableDependency.WithComponent("mother", "snehal"))
        .WithName("akanksha")
        .ImplementedBy<Person>();

      var registration = new PropertyResolvingComponentRegistration<ICanBePerson>()
        .DependsOn(
          ResolvableDependency.WithName("name"),
          ResolvableDependency.WithName("age"),
          ResolvableDependency.WithComponent("spouse", "akanksha"),
          ResolvableDependency.WithName("placeOfBirth"))
        .WithLifestyle(LifestyleType.Transient)
        .ImplementedBy<Person>();

      m_container.Register(motherRegistration);
      m_container.Register(spouseRegistration);
      m_container.Register(registration);

      // act
      Person[] results = m_container.ResolveAll<ICanBePerson>().Cast<Person>().ToArray();

      // assert
      Assert.AreEqual(3, results.Length);

      // first should be 'snehal'
      Person snehal = results[0];
      Assert.AreEqual("Snehal", snehal.Name);
      Assert.AreEqual(59, snehal.PersonAge);
      Assert.IsNull(snehal.PersonSpouse);
      Assert.IsNull(snehal.Mother);
      Assert.IsNull(snehal.PlaceOfBirth);
      Assert.AreEqual(1958, snehal.YearOfBirth);

      // second should be 'akanksha'
      Person akanksha = results[1];
      Assert.AreEqual("Akanksha", akanksha.Name);
      Assert.AreEqual(30, akanksha.PersonAge);
      Assert.IsNull(akanksha.PersonSpouse);
      Assert.AreEqual(snehal, akanksha.Mother);
      Assert.IsNull(akanksha.PlaceOfBirth);

      // third should be 'mihir'
      Person mihir = results[2];
      Assert.AreEqual("Mihir", mihir.Name);
      Assert.AreEqual(31, mihir.PersonAge);
      Assert.AreEqual(akanksha, mihir.PersonSpouse);
      Assert.IsNull(mihir.Mother);
      Assert.AreEqual("Pune", mihir.PlaceOfBirth);

    }

    /// <summary>
    ///   Test that a component with parameter
    /// </summary>
    [Test]
    public void Component_With_Parameter_And_Property_Dependencies_And_ServiceOverride_Resolves_As_Expected()
    {
      // arrange
      var spouseRegistration = new PropertyResolvingComponentRegistration<ICanBePerson>()
        .DependsOn(
          ResolvableDependency.WithValue("name", "Akanksha"),
          ResolvableDependency.WithValue("age", "30"))
        .WithName("spouse")
        .ImplementedBy<Person>();

      var registration = new PropertyResolvingComponentRegistration<ICanBePerson>()
        .DependsOn(
          ResolvableDependency.WithName("name"),
          ResolvableDependency.WithName("age"),
          ResolvableDependency.WithComponent("spouse", "spouse"))
        .ImplementedBy<Person>();

      m_container.Register(spouseRegistration);

      // act
      m_container.Register(registration);

      // assert
      Person person = (Person)m_container.Resolve<ICanBePerson>();
      Assert.AreEqual("Akanksha", person.Name);
      Assert.AreEqual(30, person.PersonAge);
      Assert.IsNull(person.PlaceOfBirth);
    }

    /// <summary>
    ///   Test that a component with properties
    /// </summary>
    [Test]
    public void Component_With_Property_Dependencies_Resolves_As_Expected()
    {
      // arrange
      ResolvableProperty prop = new ResolvableProperty("placeOfBirth");

      var registration = new PropertyResolvingComponentRegistration<ICanBePerson>()
        .DependsOn(prop)
        .ImplementedBy<Person>();

      // act
      m_container.Register(registration);

      // assert
      Person person = (Person)m_container.Resolve<ICanBePerson>();
      Assert.IsNull(person.Name);
      Assert.AreEqual(0, person.PersonAge);
      Assert.AreEqual("Pune", person.PlaceOfBirth);
    }

    /// <summary>
    ///   Test that a named component resolves properly
    /// </summary>
    [Test]
    public void Component_With_Name_Resolves_As_Expected()
    {
      // arrange
      ResolvableProperty prop = new ResolvableProperty("placeOfBirth");

      var registration = new PropertyResolvingComponentRegistration<ICanBePerson>()
        .DependsOn(prop)
        .ImplementedBy<Person>()
        .WithName("myPerson");

      // act
      m_container.Register(registration);

      // assert
      Person person = (Person)m_container.Resolve<ICanBePerson>("myPerson");
      Assert.IsNull(person.Name);
      Assert.AreEqual(0, person.PersonAge);
      Assert.AreEqual("Pune", person.PlaceOfBirth);
    }

    /// <summary>
    ///   Test that a component registered without specifying a lifestyle registers as Singleton
    /// </summary>
    [Test]
    public void Component_With_No_Lifestyle_Registers_As_Singleton()
    {
      // arrange
      ResolvableProperty prop = new ResolvableProperty("placeOfBirth");

      var registration = new PropertyResolvingComponentRegistration<ICanBePerson>()
        .DependsOn(prop)
        .ImplementedBy<Person>();

      // act
      m_container.Register(registration);

      // assert
      Person person1 = (Person)m_container.Resolve<ICanBePerson>();
      Person person2 = (Person)m_container.Resolve<ICanBePerson>();

      // since we have not specified the lifestyle, the container should return the same instance
      // therefore hashcode should be equal for both instances
      Assert.AreEqual(person1.GetHashCode(), person2.GetHashCode());
    }

    /// <summary>
    ///   Test that a lifestyle descriptor added using
    ///   <see cref="PropertyResolvingComponentRegistration{TService}.AddDescriptor(IComponentModelDescriptor)" />
    ///   overrides default lifestyle set using the
    ///   <see cref="PropertyResolvingComponentRegistration{TService}.WithLifestyle(LifestyleType)" /> decorator
    /// </summary>
    [Test]
    public void Component_With_LifestyleDescriptor_Overrides_Default_Lifestyle()
    {
      // arrange
      ResolvableProperty prop = new ResolvableProperty("placeOfBirth");

      var registration = new PropertyResolvingComponentRegistration<ICanBePerson>()
        .DependsOn(prop)
        .ImplementedBy<Person>();

      // now override lifestyle with a descriptor
      registration.AddDescriptor(new LifestyleDescriptor<ICanBePerson>(LifestyleType.Transient));

      // act
      m_container.Register(registration);

      // assert
      Person person1 = (Person)m_container.Resolve<ICanBePerson>();
      Person person2 = (Person)m_container.Resolve<ICanBePerson>();

      // since we have overridden the lifestyle, the container should generate 2 instances
      // therefore hashcode should be different for both instances
      Assert.AreNotEqual(person1.GetHashCode(), person2.GetHashCode());
    }
  }
}

#pragma warning restore 618

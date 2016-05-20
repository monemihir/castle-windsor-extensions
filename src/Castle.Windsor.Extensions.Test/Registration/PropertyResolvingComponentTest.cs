using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Extensions.Registration;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Test.Helpers;
using NUnit.Framework;

namespace Castle.Windsor.Extensions.Test.Registration
{
  /// <summary>
  /// <see cref="PropertyResolvingComponent"/> unit tests
  /// </summary>
  [TestFixture]
  public class PropertyResolvingComponentTest
  {
    /// <summary>
    /// Test that the For&lt;TService&gt; method returns the correct
    /// component
    /// </summary>
    [Test]
    public void For_Method_Returns_Correct_Component()
    {
      // arrange
      IWindsorContainer container = new WindsorContainer();
      IWindsorInstaller installer = PropertiesSubSystem.FromAppConfig();
      container.Install(installer);

      Dictionary<string, string> mappings = new Dictionary<string, string> {
        { "name", "name" },
        { "age", "age" }
      };

      // act
      var component = PropertyResolvingComponent.For<ICanBePerson>()
        .ImplementedBy<Person>()
        .DependsOnConfigProperties(mappings)
        .Transient;
      container.Register(component);

      // assert
      ICanBePerson person = container.Resolve<ICanBePerson>();
      Assert.IsInstanceOf<Person>(person);
    }
  }
}

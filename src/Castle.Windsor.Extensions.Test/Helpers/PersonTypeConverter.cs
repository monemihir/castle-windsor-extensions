using System;
using Castle.Core.Configuration;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor.Extensions.Conversion;

namespace Castle.Windsor.Extensions.Test.Helpers
{
  public class PersonTypeConverter : AbstractTypeConverter
  {
    #region Overrides of AbstractTypeConverter

    /// <summary>
    ///   Returns true if this instance of <c>ITypeConverter</c>
    ///   is able to handle the specified type.
    /// </summary>
    /// <param name="type" />
    /// <returns />
    public override bool CanHandleType(Type type)
    {
      return typeof (Person) == type;
    }

    /// <summary>
    ///   Should perform the conversion from the
    ///   string representation specified to the type
    ///   specified.
    /// </summary>
    /// <param name="value" />
    /// <param name="targetType" />
    /// <returns />
    public override object PerformConversion(string value, Type targetType)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    ///   Should perform the conversion from the
    ///   configuration node specified to the type
    ///   specified.
    /// </summary>
    /// <param name="configuration" />
    /// <param name="targetType" />
    /// <returns />
    public override object PerformConversion(IConfiguration configuration, Type targetType)
    {
      var converter = new DefaultConverter(configuration.Children, Context);
      string name = converter.Get<string>("name");
      int age = converter.Get<int>("age");

      return new Person(name, age);
    }

    #endregion
  }
}
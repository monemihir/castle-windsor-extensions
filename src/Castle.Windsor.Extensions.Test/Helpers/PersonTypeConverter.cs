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
      return typeof(Person) == type;
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
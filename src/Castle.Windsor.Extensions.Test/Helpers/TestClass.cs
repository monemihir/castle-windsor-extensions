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

namespace Castle.Windsor.Extensions.Test.Helpers
{
  public class TestClass
  {
    public string StrParam { get; private set; }
    public string RefParam { get; private set; }
    public string[] ArrParam { get; private set; }
    public IDictionary<string, string> DictParam { get; private set; }
    public List<double> ListParam { get; private set; }
    public IEnumerable<Person> PersonArr { get; private set; }

    public TestClass(string strParam, string refParam, string[] arrParam, IDictionary<string, string> dictParam, List<double> listParam, ICollection<Person> personArr)
    {
      StrParam = strParam;
      RefParam = refParam;
      ArrParam = arrParam;
      DictParam = dictParam;
      ListParam = listParam;
      PersonArr = personArr;
    }
  }
}
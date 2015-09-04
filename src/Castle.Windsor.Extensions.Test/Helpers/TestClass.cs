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

    public TestClass(string strParam, string refParam, string[] arrParam, IDictionary<string,string> dictParam, List<double> listParam, ICollection<Person> personArr)
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

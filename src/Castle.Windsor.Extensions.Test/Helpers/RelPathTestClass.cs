using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.Windsor.Extensions.Test.Helpers
{
  public class RelPathTestClass
  {
    public string[] PathArrParam { get; private set; }
    public string PathParam { get; private set; }

    public RelPathTestClass(string pathParam, string[] pathArrParam)
    {
      PathParam = pathParam;
      PathArrParam = pathArrParam;
    }
  }
}

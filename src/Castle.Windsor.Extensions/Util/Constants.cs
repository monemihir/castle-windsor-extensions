using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Castle.Windsor.Extensions.Util
{
  /// <summary>
  /// Castle Windsor Extensions library constants
  /// </summary>
  public static class Constants
  {
    /// <summary>
    ///   Parameters configuration key
    /// </summary>
    public const string ParamsConfigKey = "parameters";

    /// <summary>
    /// Key used to store the constructor candidate to be selected at component activation
    /// </summary>
    public const string ConstructorCandidateKey = "castle.windsor.extensions.componentactivator.seteligibleconstructor.constructorcandidate";

    /// <summary>
    /// Key used to store the names of the resolvable properties to be filtered by at component activation
    /// </summary>
    public const string ResolvablePublicPropertiesKey= "castle.windsor.extensions.componentactivator.setupproperties.resolvableproperties";
  }
}

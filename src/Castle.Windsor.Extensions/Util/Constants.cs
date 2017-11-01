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

namespace Castle.Windsor.Extensions.Util
{
  /// <summary>
  ///   Castle Windsor Extensions library constants
  /// </summary>
  public static class Constants
  {
    /// <summary>
    ///   Path type attribute name
    /// </summary>
    public const string PathTypeAttributeName = "pathType";

    /// <summary>
    ///   Parameters configuration key
    /// </summary>
    public const string ParamsConfigKey = "parameters";

    /// <summary>
    ///   Key used to store the constructor candidate to be selected at component activation
    /// </summary>
    public const string ConstructorCandidateKey = "castle.windsor.extensions.componentactivator.seteligibleconstructor.constructorcandidate";

    /// <summary>
    ///   Key used to store the names of the resolvable properties to be filtered by at component activation
    /// </summary>
    public const string ResolvablePublicPropertiesKey = "castle.windsor.extensions.componentactivator.setupproperties.resolvableproperties";
  }
}
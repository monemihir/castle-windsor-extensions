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

using System.IO;

namespace Castle.Windsor.Extensions.Util
{
  /// <summary>
  ///   A platform helper
  /// </summary>
  public static class PlatformHelper
  {
    /// <summary>
    ///   Convert given raw path to a platform specific path by replacing the directory separation character
    /// </summary>
    /// <param name="rawPath">Raw path</param>
    /// <returns>Platform specific path</returns>
    public static string ConvertPath(string rawPath)
    {
      if (IsUnix())
        return rawPath.Replace('\\', Path.DirectorySeparatorChar);

      return rawPath.Replace('/', Path.DirectorySeparatorChar);
    }

    /// <summary>
    ///   Whether the current platform is Unix/Linux
    /// </summary>
    /// <returns>True if is Unix/Linux, else false</returns>
    public static bool IsUnix()
    {
      return Path.DirectorySeparatorChar == '/';
    }
  }
}
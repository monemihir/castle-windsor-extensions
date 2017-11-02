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
using System.IO;
using System.Reflection;
using Castle.Windsor.Extensions.Util;

namespace Castle.Windsor.Extensions.Test.Helpers
{
  /// <summary>
  ///   Provides utility methods for embeded resources in DLLs
  /// </summary>
  [Serializable]
  public static class EmbeddedResourceUtil
  {
    /// <summary>
    ///   Exports the specified embedded resource from the current assembly to the path specified.
    ///   Within the assembly the fully qualified name of the resource is expected to be resourcePath + "." + resourceName.
    ///   Note: If the embedded resource is in a subdirectory, the subdirectory name can not consist of only numbers. For e.g.
    ///   if your resource is in a subdirectory names 1234, you must rename it to something like test1234 such that the
    ///   directory
    ///   name does not contain only numbers. C# in its infinite wisdom does not like directories with names containing only
    ///   numbers
    /// </summary>
    /// <param name="resPath">The fully qualified (ie namespace + subdirectory) resource path (excluding the resourceName)</param>
    /// <param name="resName">The resource name</param>
    /// <param name="outputPath">The path to save to. If you want to rename the resource specify a file name in this path.</param>
    /// <returns>The path to the saved file</returns>
    public static string ExportToPath(string resPath, string resName, string outputPath)
    {
      return ExportToPath(Assembly.GetExecutingAssembly(), resPath, resName, outputPath);
    }

    /// <summary>
    ///   Exports the specified embedded resource from the given assembly to the path specified.
    ///   Within the assembly the fully qualified name of the resource is expected to be resourcePath + "." + resourceName.
    ///   Note: If the embedded resource is in a subdirectory, the subdirectory name can not consist of only numbers. For e.g.
    ///   if your resource is in a subdirectory names 1234, you must rename it to something like test1234 such that the
    ///   directory
    ///   name does not contain only numbers. C# in its infinite wisdom does not like directories with names containing only
    ///   numbers
    /// </summary>
    /// <param name="source">The assembly containing the resource</param>
    /// <param name="resPath">The fully qualified (ie namespace + subdirectory) resource path (excluding the resourceName)</param>
    /// <param name="resName">The resource name</param>
    /// <param name="outputPath">The path to save to. If you want to rename the resource specify a file name in this path.</param>
    /// <returns>The path to the saved file</returns>
    public static string ExportToPath(Assembly source, string resPath, string resName, string outputPath)
    {
      string fullResourceName = string.Format("{0}.{1}", resPath, resName);

      // Get manifest resource stream
      Stream resourceStream = source.GetManifestResourceStream(fullResourceName);

      if (resourceStream == null)
      {
        // This means that the resource was not found, so now throw an exception
        string message = string.Format("Unable to find {0} in assembly {1}", fullResourceName, source.Location);
        throw new Exception(message);
      }

      if (!Directory.Exists(outputPath))
        Directory.CreateDirectory(outputPath);

      string filePath = string.IsNullOrWhiteSpace(Path.GetExtension(outputPath)) ? outputPath + Path.DirectorySeparatorChar + resName : outputPath;

      filePath = Path.GetFullPath(PlatformHelper.ConvertPath(filePath));

      BinaryReader reader = new BinaryReader(resourceStream);
      BinaryWriter writer = new BinaryWriter(new FileStream(filePath, FileMode.Create));

      byte[] buffer = reader.ReadBytes(1024);
      while (buffer.Length > 0)
      {
        writer.Write(buffer);
        buffer = reader.ReadBytes(1024);
      }

      writer.Close();
      reader.Close();

      return filePath;
    }
  }
}
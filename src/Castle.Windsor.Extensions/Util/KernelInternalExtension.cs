// 
// This file is part of - Castle Windsor Extensions
// Copyright (C) 2015 Mihir Mone
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//   

using Castle.MicroKernel;
using Castle.Windsor.Extensions.SubSystems;

namespace Castle.Windsor.Extensions.Util
{
  /// <summary>
  ///   Extension method for <see cref="IKernelInternal" /> interface
  /// </summary>
  public static class KernelInternalExtension
  {
    /// <summary>
    ///   Get the registered settings sub system
    /// </summary>
    /// <param name="kernel">Current <see cref="IKernelInternal" /> instance</param>
    /// <returns>Settings subsystem</returns>
    public static SettingsSubSystem GetSettingsSubSystem(this IKernelInternal kernel)
    {
      ISubSystem subSystem = kernel.GetSubSystem(SettingsSubSystem.SubSystemKey);

      if (subSystem != null)
        return (SettingsSubSystem) subSystem;

      SettingsSubSystem newSubSystem = new SettingsSubSystem();
      kernel.AddSubSystem(SettingsSubSystem.SubSystemKey, newSubSystem);

      return newSubSystem;
    }
  }
}
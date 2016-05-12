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

using Castle.Windsor.Extensions.Facilities;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Util;
using NUnit.Framework;

namespace Castle.Windsor.Extensions.Test.Util
{
  /// <summary>
  ///   Unit tests for <see cref="KernelExtension" />
  /// </summary>
  [TestFixture]
  public class KernelExtensionTest
  {
    /// <summary>
    ///   Test that the <see cref="KernelExtension.GetSettingsSubSystem(MicroKernel.IKernel)" /> returns the
    ///   settings subsystem as expected
    /// </summary>
    [Test]
    public void GetSettingsSubSystem_Works_As_Expected()
    {
      // arrange
      IWindsorContainer container = new WindsorContainer();
      container.AddFacility(new RelativePathFacility());

      // act
      SettingsSubSystem subSystem = container.Kernel.GetSettingsSubSystem();

      // assert
      Assert.IsNotNull(subSystem);
    }

    /// <summary>
    ///   Test that <see cref="KernelExtension.GetSubSystem{T}(MicroKernel.IKernel, string)" /> returns
    ///   the requested sub system as expected
    /// </summary>
    [Test]
    public void GetSubSystem_Works_As_Expected()
    {
      // arrange
      IWindsorContainer container = new WindsorContainer();
      container.AddFacility(new RelativePathFacility());

      // act
      SettingsSubSystem subSystem = container.Kernel.GetSubSystem<SettingsSubSystem>(SettingsSubSystem.SubSystemKey);

      // assert
      Assert.IsNotNull(subSystem);
    }
  }
}
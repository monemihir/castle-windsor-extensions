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

using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor.Extensions.SubSystems;

namespace Castle.Windsor.Extensions.Installer
{
  /// <summary>
  ///   A <see cref="PropertiesSubSystem" /> installer which registers the sub system with the
  ///   current <see cref="IKernel" />
  /// </summary>
  public class PropertiesSubSystemInstaller : IWindsorInstaller
  {
    private readonly string m_configFile;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="configFile">Castle configuration file</param>
    public PropertiesSubSystemInstaller(string configFile = "")
    {
      m_configFile = configFile;
    }

    #region Implementation of IWindsorInstaller

    /// <summary>
    ///   Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer" />.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="store">The configuration store.</param>
    public void Install(IWindsorContainer container, IConfigurationStore store)
    {
      PropertiesSubSystem subsystem = string.IsNullOrEmpty(m_configFile) ? new PropertiesSubSystem() : new PropertiesSubSystem(m_configFile);

      container.Kernel.AddSubSystem(PropertiesSubSystem.SubSystemKey, subsystem);
    }

    #endregion
  }
}
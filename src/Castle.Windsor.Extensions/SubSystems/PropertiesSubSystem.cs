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
using Castle.Windsor.Extensions.Installer;
using Castle.Windsor.Extensions.Interpreters;
using Castle.Windsor.Extensions.Resolvers;

namespace Castle.Windsor.Extensions.SubSystems
{
  /// <summary>
  ///   Properties sub system
  /// </summary>
  public class PropertiesSubSystem : ISubSystem
  {
    private readonly IPropertiesInterpreter m_interpreter;

    /// <summary>
    ///   SubSystem registration key
    /// </summary>
    public const string SubSystemKey = "castle.windsor.extensions.properties.subsystem.key";

    /// <summary>
    ///   Properties resolver
    /// </summary>
    public IPropertyResolver Resolver { get; protected set; }

    /// <summary>
    ///   Constructor
    /// </summary>
    public PropertiesSubSystem()
      : this(new PropertiesInterpreter())
    {
      // nothing to do here
    }

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="filename">Castle configuration file</param>
    public PropertiesSubSystem(string filename)
      : this(new PropertiesInterpreter(filename))
    {
      // nothing to do here
    }

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="interpreter">A properties interpreter</param>
    public PropertiesSubSystem(IPropertiesInterpreter interpreter)
    {
      m_interpreter = interpreter;
    }

    /// <summary>
    ///   Create a <see cref="PropertiesSubSystem" /> from the app.config/web.config
    ///   file
    /// </summary>
    /// <returns>An instance of the <see cref="PropertiesSubSystem" /></returns>
    public static IWindsorInstaller FromAppConfig()
    {
      return new PropertiesSubSystemInstaller();
    }

    #region ISubSystem implementation

    /// <summary>
    ///   Initialise this sub system
    /// </summary>
    /// <param name="kernel">Windsor kernel</param>
    public void Init(IKernelInternal kernel)
    {
      m_interpreter.ProcessResource(m_interpreter.Source, kernel.ConfigurationStore, kernel);

      Resolver = m_interpreter.Resolver;
    }

    /// <summary>
    ///   Terminate this sub system
    /// </summary>
    public void Terminate()
    {
      // nothing to do here
    }

    #endregion ISubSystem implementation
  }
}
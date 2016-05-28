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

using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Extensions.Interpreters;
using Castle.Windsor.Extensions.SubSystems;

namespace Castle.Windsor.Extensions
{
  /// <summary>
  ///   A property resolving <see cref="IWindsorContainer" />
  /// </summary>
  public sealed class PropertyResolvingWindsorContainer : WindsorContainer
  {
    /// <summary>
    ///   Underlying properties interpreter
    /// </summary>
    public IPropertiesInterpreter Interpreter { get; private set; }

    /// <summary>
    ///   Constructor (initialises the <see cref="IWindsorContainer" /> from the app.config/web.config
    ///   file)
    /// </summary>
    public PropertyResolvingWindsorContainer()
    {
      Interpreter = new PropertiesInterpreter();

      Kernel.AddSubSystem(PropertiesSubSystem.SubSystemKey, new PropertiesSubSystem(Interpreter));
    }

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="configFile">XML configuration file to initialise from</param>
    public PropertyResolvingWindsorContainer(string configFile)
      : base(new XmlInterpreter(configFile))
    {
      Interpreter = new PropertiesInterpreter(configFile);

      Kernel.AddSubSystem(PropertiesSubSystem.SubSystemKey, new PropertiesSubSystem(Interpreter));
    }
  }
}
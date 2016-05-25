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

using System.Xml;
using Castle.Core.Resource;

namespace Castle.Windsor.Extensions.Processor
{
  /// <summary>
  ///   A generic <see cref="IResource" /> processor
  /// </summary>
  public interface IResourceProcessor
  {
    /// <summary>
    ///   Process given resource
    /// </summary>
    /// <param name="resource">Resource to process</param>
    /// <returns>Resource processed to an XML node</returns>
    XmlNode Process(IResource resource);

    /// <summary>
    ///   Process given xml node
    /// </summary>
    /// <param name="node">Node to process</param>
    /// <returns>Processed node</returns>
    XmlNode Process(XmlNode node);

    /// <summary>
    ///   Get property with given name
    /// </summary>
    /// <param name="name">Property name</param>
    /// <returns>Property as an XML element</returns>
    XmlElement GetProperty(string name);
  }
}
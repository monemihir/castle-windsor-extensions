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

using Castle.Windsor.Configuration;
using Castle.Windsor.Extensions.Resolvers;

namespace Castle.Windsor.Extensions.Interpreters
{
  /// <summary>
  ///   A generic interface for all property resolvers
  /// </summary>
  public interface IPropertiesInterpreter : IConfigurationInterpreter
  {
    /// <summary>
    ///   Properties resolver
    /// </summary>
    IPropertyResolver Resolver { get; }
  }
}
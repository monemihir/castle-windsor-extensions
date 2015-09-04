/*
* This file is part of - Castle Windsor Extensions
* Copyright (C) 2015 Mihir Mone
*
* This program is free software: you can redistribute it and/or modify
* it under the terms of the GNU Lesser General Public License as published by
* the Free Software Foundation, either version 2 of the License, or
* (at your option) any later version.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU Lesser General Public License for more details.
*
* You should have received a copy of the GNU Lesser General Public License
* along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Linq;
using Castle.Core.Configuration;
using Castle.MicroKernel.SubSystems.Conversion;

namespace Castle.Windsor.Extensions.Conversion
{
  /// <summary>
  ///   Default converter class
  /// </summary>
  public class DefaultConverter
  {
    /// <summary>
    ///   Configuration store
    /// </summary>
    private readonly ConfigurationCollection m_configurationCollection;

    /// <summary>
    ///   Current type converter context
    /// </summary>
    private readonly ITypeConverterContext m_context;

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="configurationCollection">Configuration store</param>
    /// <param name="context">Current type converter context</param>
    public DefaultConverter(ConfigurationCollection configurationCollection, ITypeConverterContext context)
    {
      m_configurationCollection = configurationCollection;
      m_context = context;
    }

    /// <summary>
    ///   Get converted value
    /// </summary>
    /// <typeparam name="TType">Target type</typeparam>
    /// <param name="paramter">Paramter name</param>
    /// <returns>Converted value</returns>
    public TType Get<TType>(string paramter)
    {
      var configuration = m_configurationCollection.SingleOrDefault(c => c.Name == paramter);
      if (configuration == null)
      {
        throw new ApplicationException(string.Format(
          "In the castle configuration, type '{0}' expects parameter '{1}' that was missing.",
          typeof (TType).Name, paramter));
      }
      return (TType) m_context.Composition.PerformConversion(configuration, typeof (TType));
    }
  }
}
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

using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.SubSystems.Conversion;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Util;

namespace Castle.Windsor.Extensions.Resolvers
{
  /// <summary>
  ///   Relative path dependency resolver
  /// </summary>
  public class RelativePathSubDependencyResolver : ISubDependencyResolver
  {
    /// <summary>
    ///   Parameters configuration key
    /// </summary>
    private const string ParamsConfigKey = "parameters";

    /// <summary>
    ///   All types that this resolver can resolve
    /// </summary>
    private static readonly string[] ResolvableTypes =
    {
      typeof(string).FullName,
      typeof(IList<string>).FullName,
      typeof(string[]).FullName,
      typeof(List<string>).FullName,
      typeof(ICollection<string>).FullName,
      typeof(IEnumerable<string>).FullName
    };

    private static readonly List<string> SpecialNodes = new List<string> {"array", "list"};
    private static IDictionary<string, object> VALUES;
    private readonly IConversionManager m_converter;

    /// <summary>
    ///   Constructor
    /// </summary>
    public RelativePathSubDependencyResolver(IKernel kernel)
    {
      m_converter = (IConversionManager)kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
      SettingsSubSystem settingsSubSystem = kernel.GetSettingsSubSystem();
      settingsSubSystem.ResolveRelativePaths = true;
      VALUES = new Dictionary<string, object>();
    }

    /// <summary>
    ///   Resolves the specified dependency.
    /// </summary>
    /// <param name="context">Creation context</param>
    /// <param name="contextHandlerResolver">Parent resolver</param>
    /// <param name="model">Model of the component that is requesting the dependency</param>
    /// <param name="dependency">The dependcy to satisfy</param>
    /// <returns><c>true</c> if the dependency can be satsfied by this resolver, else <c>false</c>.</returns>
    /// <returns>The resolved dependency</returns>
    public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
    {
      object value = ProcessDependency(model, dependency);

      if (value == null)
        throw new ConfigurationProcessingException(string.Format("Unable to resolve dependency '{0}'", dependency));

      return value;
    }

    /// <summary>
    ///   Determines whether this sub dependency resolver can resolve the specified dependency.
    /// </summary>
    /// <param name="context">Creation context</param>
    /// <param name="contextHandlerResolver">Parent resolver</param>
    /// <param name="model">Model of the component that is requesting the dependency</param>
    /// <param name="dependency">The dependcy to satisfy</param>
    /// <returns><c>true</c> if the dependency can be satsfied by this resolver, else <c>false</c>.</returns>
    public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model, DependencyModel dependency)
    {
      if (ResolvableTypes.Contains(dependency.TargetType.FullName))
        return ProcessDependency(model, dependency) != null;

      return false;
    }

    #region Misc methods

    /// <summary>
    ///   Finds the parameter by looking at the cache, then in the model configuration.
    /// </summary>
    /// <param name="model">Model of the component that is requesting the dependency</param>
    /// <param name="dependency">The dependcy to satisfy</param>
    /// <returns>The relative path parameter</returns>
    private object ProcessDependency(ComponentModel model, DependencyModel dependency)
    {
      if (VALUES.ContainsKey(dependency.DependencyKey))
        return VALUES[dependency.DependencyKey];

      IConfiguration parameterNodeConfig = model.Configuration.Children[ParamsConfigKey];
      if (parameterNodeConfig == null)
        return false;

      IConfiguration paramConfig = parameterNodeConfig.Children.SingleOrDefault(f => f.Name == dependency.DependencyKey);
      if (paramConfig == null)
        throw new ConfigurationProcessingException(string.Format("Missing parameter value for parameter '{0}'", dependency.DependencyKey));

      RelativePathUtil.ConvertPaths(paramConfig, null);

      IConfiguration processedConfig = null;
      if (paramConfig.Children.Count > 0)
      {
        IConfiguration firstChild = paramConfig.Children[0];
        string configName = firstChild.Name.ToLowerInvariant();
        if (SpecialNodes.Contains(configName))
        {
          processedConfig = new MutableConfiguration(paramConfig.Name, string.Empty);
          processedConfig.Children.AddRange(firstChild.Children);
        }
      }
      else
      {
        processedConfig = paramConfig;
      }

      object value = m_converter.PerformConversion(processedConfig, dependency.TargetType);

      VALUES.Add(dependency.DependencyKey, value);

      return value;
    }

    #endregion Misc methods
  }
}
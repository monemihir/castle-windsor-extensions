// 
// This file is part of - Castle Windsor Extensions
// Copyright (C) 2017 Mihir Mone
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
using System.Linq;
using System.Reflection;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Context;
using Castle.Windsor.Extensions.Registration;
using Castle.Windsor.Extensions.Util;

namespace Castle.Windsor.Extensions.ComponentActivator
{
  /// <summary>
  ///   A component activator which has a preloaded reference to the <see cref="ConstructorCandidate" /> to use for
  ///   initialisation as well as knowing which public properties are actually resolvable using the given registered config
  /// </summary>
  public class PropertyResolvingComponentActivator : DefaultComponentActivator
  {
    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="model">Component model</param>
    /// <param name="kernel">Current kernel</param>
    /// <param name="onCreation">A delegate to invoke post creation</param>
    /// <param name="onDestruction">A delegate to invoke post destruction</param>
    public PropertyResolvingComponentActivator(ComponentModel model, IKernel kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
      : base(model, kernel, onCreation, onDestruction)
    {
      // nothing to do here
    }

    /// <summary>
    ///   Select an eligible constructor for given creation context by inspecting
    ///   <see cref="ComponentModel.ExtendedProperties" /> for existence of a constructor candidate as set by
    ///   <see cref="ResolvableDependencyDescriptor" />
    /// </summary>
    /// <param name="context">Creation context</param>
    /// <returns>Selected constructor candidate</returns>
    /// <exception cref="NoResolvableConstructorFoundException">
    ///   If no resolvable constructor was set by
    ///   <see cref="ResolvableDependencyDescriptor" />
    /// </exception>
    protected override ConstructorCandidate SelectEligibleConstructor(CreationContext context)
    {
      if (!Model.ExtendedProperties.Contains(Constants.ConstructorCandidateKey))
        return base.SelectEligibleConstructor(context);

      ConstructorCandidate candidate = (ConstructorCandidate)Model.ExtendedProperties[Constants.ConstructorCandidateKey];

      if (candidate == null)
        throw new NoResolvableConstructorFoundException(context.RequestedType, Model);

      return candidate;
    }

    /// <summary>
    ///   Set up public properties on the given instance
    /// </summary>
    /// <param name="instance">Object instance to update</param>
    /// <param name="context">Current creation context</param>
    protected override void SetUpProperties(object instance, CreationContext context)
    {
      instance = ProxyUtil.GetUnproxiedInstance(instance);
      IDependencyResolver resolver = Kernel.Resolver;
      string[] resolvableProperties = (string[])Model.ExtendedProperties[Constants.ResolvablePublicPropertiesKey];
      foreach (PropertySet property in Model.Properties)
      {
        if (!resolvableProperties.Contains(property.Dependency.DependencyKey))
          continue;

        object value = ObtainPropertyValue(context, property, resolver);
        if (value == null)
          continue;

        MethodInfo setMethod = property.Property.GetSetMethod();
        try
        {
          setMethod.Invoke(instance, new[] {value});
        }
        catch (Exception ex)
        {
          string message =
            string.Format(
              "Error setting property {1}.{0} in component {2}. See inner exception for more information. If you don't want Windsor to set this property you can do it by either decorating it with {3} or via registration API.",
              property.Property.Name, instance.GetType().Name, Model.Name, typeof(DoNotWireAttribute).Name);
          throw new ComponentActivatorException(message, ex, Model);
        }
      }
    }

    /// <summary>
    ///   Get the property value from the dependency resolver
    /// </summary>
    /// <param name="context">Current creation context</param>
    /// <param name="property">Property to get value of</param>
    /// <param name="resolver">Current dependency resolver</param>
    /// <returns>Property value if resolvable, else null</returns>
    private object ObtainPropertyValue(CreationContext context, PropertySet property, IDependencyResolver resolver)
    {
      if (property.Dependency.IsOptional && !resolver.CanResolve(context, context.Handler, Model, property.Dependency))
        return null;

      try
      {
        return resolver.Resolve(context, context.Handler, Model, property.Dependency);
      }
      catch (Exception)
      {
        if (!property.Dependency.IsOptional)
        {
          throw;
        }
      }
      return null;
    }
  }
}
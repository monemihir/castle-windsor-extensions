using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Context;
using Castle.Windsor.Extensions.Util;

namespace Castle.Windsor.Extensions.ComponentActivator
{
  public class ConstructorCandidateOverridingComponentActivator : DefaultComponentActivator
  {

    public ConstructorCandidateOverridingComponentActivator(ComponentModel model, IKernel kernel, ComponentInstanceDelegate onCreation, ComponentInstanceDelegate onDestruction)
      : base(model, kernel, onCreation, onDestruction)
    {
    }

    protected override ConstructorCandidate SelectEligibleConstructor(CreationContext context)
    {
      if (Model.ExtendedProperties.Contains(Constants.ConstructorCandidateKey))
        return (ConstructorCandidate)Model.ExtendedProperties[Constants.ConstructorCandidateKey];

      return base.SelectEligibleConstructor(context);
    }

    protected override void SetUpProperties(object instance, CreationContext context)
    {
      instance = ProxyUtil.GetUnproxiedInstance(instance);
      var resolver = Kernel.Resolver;
      string[] resolvableProperties = (string[])Model.ExtendedProperties[Constants.ResolvablePublicPropertiesKey];
      foreach (var property in Model.Properties)
      {
        if (!resolvableProperties.Contains(property.Dependency.DependencyKey))
          continue;

        var value = ObtainPropertyValue(context, property, resolver);
        if (value == null)
          continue;

        var setMethod = property.Property.GetSetMethod();
        try
        {
          setMethod.Invoke(instance, new[] { value });
        }
        catch (Exception ex)
        {
          var message =
            String.Format(
              "Error setting property {1}.{0} in component {2}. See inner exception for more information. If you don't want Windsor to set this property you can do it by either decorating it with {3} or via registration API.",
              property.Property.Name, instance.GetType().Name, Model.Name, typeof(DoNotWireAttribute).Name);
          throw new ComponentActivatorException(message, ex, Model);
        }
      }
    }

    private object ObtainPropertyValue(CreationContext context, PropertySet property, IDependencyResolver resolver)
    {
      if (property.Dependency.IsOptional == false ||
          resolver.CanResolve(context, context.Handler, Model, property.Dependency))
      {
        try
        {
          return resolver.Resolve(context, context.Handler, Model, property.Dependency);
        }
        catch (Exception)
        {
          if (property.Dependency.IsOptional == false)
          {
            throw;
          }
        }
      }
      return null;
    }
  }
}

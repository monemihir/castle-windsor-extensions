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

using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.ModelBuilder.Descriptors;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Extensions.SubSystems;
using Castle.Windsor.Extensions.Util;

namespace Castle.Windsor.Extensions.Registration
{
  /// <summary>
  ///   A property resolving component registration
  /// </summary>
  public class PropertyResolvingComponentRegistration<TService> : IRegistration where TService : class
  {
    private readonly List<Type> m_potentialServices = new List<Type>();
    //private readonly List<ServiceOverride> m_services = new List<ServiceOverride>();
    private readonly List<ResolvableProperty> m_properties = new List<ResolvableProperty>();
    private readonly List<IComponentModelDescriptor> m_descriptors = new List<IComponentModelDescriptor>();

    private bool m_registered;
    private LifestyleType m_lifestyle;
    private IEnumerable<KeyValuePair<string, string>> m_dependencyMapping;

    /// <summary>
    ///   Implementation type
    /// </summary>
    public Type Implementation { get; private set; }

    /// <summary>
    ///   Name of the current component. This will be null unless a name is explicitly set using the
    ///   <see cref="WithName(string)" /> method
    /// </summary>
    public ComponentName Name { get; private set; }

    /// <summary>
    ///   Whether the lifestyle of this component has been overridden by a descriptor
    /// </summary>
    public bool IsLifestyleOverridenWithDescriptor { get; private set; }

    /// <summary>
    ///   Constructor
    /// </summary>
    public PropertyResolvingComponentRegistration()
      : this(typeof(TService))
    {
      // nothing to do here
    }

    /// <summary>
    ///   Constructor
    /// </summary>
    /// <param name="services">Types describing the services</param>
    public PropertyResolvingComponentRegistration(params Type[] services)
    {
      m_dependencyMapping = new Dictionary<string, string>();
      m_lifestyle = LifestyleType.Singleton;

      Forward(services);
    }

    /// <summary>
    ///   Gets contributers for current component registration
    /// </summary>
    /// <returns>Component model descripters describing given services</returns>
    private IComponentModelDescriptor[] GetContributors()
    {
      List<IComponentModelDescriptor> componentModelDescriptorList = new List<IComponentModelDescriptor>
      {
        new ServicesDescriptor(m_potentialServices.ToArray()),
        new DefaultsDescriptor(Name, Implementation)
      };

      componentModelDescriptorList.AddRange(m_descriptors);

      return componentModelDescriptorList.ToArray();
    }

    /// <summary>
    ///   Creates the dependency descripter for the current component registration
    /// </summary>
    /// <param name="kernel">Current <see cref="IKernel" /></param>
    private void CreateDependencyDescripters(IKernel kernel)
    {
      PropertiesSubSystem subsystem = kernel.GetSubSystem<PropertiesSubSystem>(PropertiesSubSystem.SubSystemKey);

      if (subsystem == null)
        throw new DependencyResolverException("Unable to create component dependencies. No properties resolver found. You must register a PropertiesSubSystem instance with the container");

      List<Parameter> parameters = new List<Parameter>();
      foreach (var kv in m_dependencyMapping)
      {
        if (!subsystem.Resolver.CanResolve(kv.Value))
          throw new ConfigurationProcessingException("No config property with name '" + kv.Value + "' found");

        Parameter p = Parameter.ForKey(kv.Key).Eq(subsystem.Resolver.GetConfig(kv.Value));
        parameters.Add(p);
      }

      //if (m_services.Count > 0)
      //  AddDescriptor(new ServiceOverrideDescriptor(m_services.ToArray()));
      if (m_properties.Count > 0)
        AddDescriptor(new ResolvablePropertyDescriptor(m_properties.ToArray()));
      if (parameters.Count > 0)
        AddDescriptor(new ParametersDescriptor(parameters.ToArray()));
    }

    /// <summary>
    ///   Adds <paramref name="types" /> as additional services to be exposed by this component.
    /// </summary>
    /// <param name="types">The types to forward.</param>
    /// <returns>Current component registration</returns>
    public PropertyResolvingComponentRegistration<TService> Forward(IEnumerable<Type> types)
    {
      Type[] arr = types.ToArray();

      foreach (Type type in arr)
        ComponentServicesUtil.AddService(m_potentialServices, type);

      return this;
    }

    /// <summary>
    ///   Sets the concrete type that implements the service to <typeparamref name="TImpl" />.
    ///   <para />
    ///   If not set, the class service type or first registered interface will be used as the implementation for this
    ///   component.
    /// </summary>
    /// <typeparam name="TImpl">The type that is the implementation for the service.</typeparam>
    /// <returns>Current component registration</returns>
    public PropertyResolvingComponentRegistration<TService> ImplementedBy<TImpl>() where TImpl : TService
    {
      if (Implementation != null && Implementation != typeof(LateBoundComponent))
        throw new ComponentRegistrationException(string.Format("This component has already been assigned implementation {0}", Implementation.FullName));

      Implementation = typeof(TImpl);

      return this;
    }

    /// <summary>Adds the descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>Current component registration</returns>
    public PropertyResolvingComponentRegistration<TService> AddDescriptor(IComponentModelDescriptor descriptor)
    {
      // handle special condition of someone adding a lifestyle descriptor
      var lifestyleDescriptor = descriptor as LifestyleDescriptor<TService>;

      if (lifestyleDescriptor != null)
        IsLifestyleOverridenWithDescriptor = true;

      m_descriptors.Add(descriptor);

      return this;
    }

    /// <summary>
    ///   Set a name of this registration. This is required if you have multiple components for a given service and want to be
    ///   able to resolve some specific ones. Then you'd provide the name so that Windsor knows which one of the bunch you
    ///   know. Otherwise don't bother setting the name.
    ///   <para />
    ///   If not set, the <see cref="P:System.Type.FullName" /> of the
    ///   <see cref="Implementation" />
    ///   will be used as the key to register the component.
    /// </summary>
    /// <param name="name">The name of this registration.</param>
    /// <returns>Current component registration</returns>
    /// <remarks>
    ///   Names have to be globally unique in the scope of the container.
    /// </remarks>
    public PropertyResolvingComponentRegistration<TService> WithName(string name)
    {
      if (Name != null)
        throw new ComponentRegistrationException(string.Format("This component has already been assigned name '{0}'", Name.Name));

      if (name == null)
        return this;

      Name = new ComponentName(name, true);

      return this;
    }

    /// <summary>
    ///   Registers current component with given lifestyle type
    /// </summary>
    /// <param name="type">Lifestyle for the component</param>
    /// <returns>Current component registration</returns>
    public PropertyResolvingComponentRegistration<TService> WithLifestyle(LifestyleType type)
    {
      m_lifestyle = type;

      return this;
    }

    /// <summary>
    ///   Register dependencies on config properties (i.e. the properties that come from the config file)
    /// </summary>
    /// <param name="mapping">
    ///   A mapping of dependency name to config property name where key is the
    ///   name of the component dependency and value is the name of the config property
    /// </param>
    /// <returns>Current component registration</returns>
    public PropertyResolvingComponentRegistration<TService> DependsOnConfigProperties(IDictionary<string, string> mapping)
    {
      if (mapping == null)
        throw new ArgumentNullException("mapping", "I can't work with a null value. If your component doesn't depend on anything, the best thing you could do is not call me.");

      m_dependencyMapping = m_dependencyMapping.Concat(mapping);

      return this;
    }

    /// <summary>
    ///   Register dependencies on public properties of the type described by the component (i.e. at
    ///   run time, the <see cref="IKernel" /> will try to set public properties on the component
    ///   instance by inspecting these properties
    /// </summary>
    /// <param name="properties">Property dependencies</param>
    /// <returns>Current component registratio</returns>
    public PropertyResolvingComponentRegistration<TService> DependsOnProperties(params ResolvableProperty[] properties)
    {
      m_properties.AddRange(properties);

      return this;
    }

    /// <summary>
    ///   Register dependencies on public properties of the type described by the component (i.e. at
    ///   run time, the <see cref="IKernel" /> will try to set public properties on the component
    ///   instance by inspecting these properties
    /// </summary>
    /// <param name="properties">Property dependencies</param>
    /// <returns>Current component registratio</returns>
    public PropertyResolvingComponentRegistration<TService> DependsOnProperties(IEnumerable<ResolvableProperty> properties)
    {
      if (properties == null)
        throw new ArgumentNullException("properties");

      m_properties.AddRange(properties);

      return this;
    }

    ///// <summary>
    /////   Register service dependencies i.e the other services that the current component registration
    /////   depends on
    ///// </summary>
    ///// <param name="services">Service override dependencies</param>
    ///// <returns>Current component registration</returns>
    //public PropertyResolvingComponentRegistration<TService> DependsOnServices(params ServiceOverride[] services)
    //{
    //  m_services.AddRange(services);

    //  return this;
    //}

    #region Implementation of IRegistration

    /// <summary>
    ///   Performs the registration in the <see cref="T:Castle.MicroKernel.IKernel" />.
    /// </summary>
    /// <param name="kernel">The kernel.</param>
    public virtual void Register(IKernelInternal kernel)
    {
      if (m_registered)
        return;

      // create component configuration with resolved properties
      CreateDependencyDescripters(kernel);

      // setup lifestyle
      if (!IsLifestyleOverridenWithDescriptor)
        AddDescriptor(new LifestyleDescriptor<TService>(m_lifestyle));

      IComponentModelDescriptor[] contributors = GetContributors();

      ComponentModel componentModel = kernel.ComponentModelBuilder.BuildModel(contributors);

      kernel.AddCustomComponent(componentModel);

      m_registered = true;
    }

    #endregion
  }
}
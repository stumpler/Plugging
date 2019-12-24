using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Validation;

namespace Plugging
{
    /// <summary>
    /// Holds module metadata including services the module supports.
    /// </summary>
    [DebuggerDisplay("Name = {" + nameof(Name) + "} Services = {" + nameof(Services) + "}")]
    public class Module
    {
        #region Nested Types

        private class Service
        {
            public Enum SupportedOperations { get; set; }
            
            public Func<IServiceProvider, object> GetInstanceFunc { get; set; }
        }

        private enum CommonOperations
        {
            All
        }

        #endregion

        /// <summary>
        /// Services supported by the module. The core interface of a service (e.g. IGatewayService) is used as a key for the dictionary.
        /// </summary>
        private readonly Dictionary<Type, Service> _services = new Dictionary<Type, Service>();

        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public Module([NotNull] string name)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            Name = name;
        }

        /// <summary>
        /// Module name.
        /// </summary>
        [NotNull] 
        public string Name { get; }

        /// <summary>
        /// Retrieves a list of supported core service interfaces.
        /// </summary>
        [NotNull]
        public Type[] Services
        {
            get { return _services.Keys.ToArray(); }
        }

        /// <summary>
        /// Custom properties such as a config object. Rather that using the dictionary directly,
        /// it is better to access it via an extension method defined for this class, e.g. TypedConfig GetConfig(this Module module).
        /// </summary>
        [NotNull]
        public IDictionary<string, object> Properties
        {
            get { return _properties; }
        }

        /// <summary>
        /// Registers a service for the specified core interface type.
        /// </summary>
        /// <typeparam name="TCoreInterface">The type of a core interface such as IGatewayService.</typeparam>
        /// <param name="getServiceInstanceFunc">The function to get an instance of the service being registered.</param>
        /// <param name="supportedOperations">Operations supported by a service being registered.
        /// If not specified, all operations are meant to be supported.</param>
        public void Register<TCoreInterface>(
            [NotNull] Func<IServiceProvider, TCoreInterface> getServiceInstanceFunc,
            [CanBeNull] Enum supportedOperations = default)
            where TCoreInterface : class
        {
            Requires.NotNull(getServiceInstanceFunc, nameof(getServiceInstanceFunc));

            _services.Add(typeof(TCoreInterface), new Service
            {
                GetInstanceFunc = getServiceInstanceFunc,
                SupportedOperations = supportedOperations ?? CommonOperations.All
            });
        }

        /// <summary>
        /// Decorate an existing service.
        /// </summary>
        /// <typeparam name="TCoreInterface">The type of a core interface to decorate.</typeparam>
        /// <param name="createDecoratorFunc">The function to create an instance of a decorator being added to the pipeline.</param>
        public void Decorate<TCoreInterface>([NotNull] Func<TCoreInterface, IServiceProvider, TCoreInterface> createDecoratorFunc)
            where TCoreInterface : class
        {
            Requires.NotNull(createDecoratorFunc, nameof(createDecoratorFunc));

            Service service = GetRequiredService<TCoreInterface>();
            Func<IServiceProvider, object> originalGetInstanceFunc = service.GetInstanceFunc;
            
            service.GetInstanceFunc = serviceProvider =>
            {
                TCoreInterface instance = (TCoreInterface)originalGetInstanceFunc.Invoke(serviceProvider);
                return createDecoratorFunc.Invoke(instance, serviceProvider);
            };
        }

        /// <summary>
        /// Retrieves an aggregated composition of operations supported by the specified service.
        /// </summary>
        /// <param name="coreInterfaceType">The type of a core service interface to get supported operations for.</param>
        /// <returns>Null if the service itself isn't registered or a flags enumeration representing supported operations.</returns>
        [CanBeNull]
        public Enum GetSupportedOperations([NotNull] Type coreInterfaceType)
        {
            Requires.NotNull(coreInterfaceType, nameof(coreInterfaceType));

            Service service = GetService(coreInterfaceType);
            return service?.SupportedOperations;
        }

        /// <summary>
        /// Retrieves a factory method to create the specified service.
        /// </summary>
        /// <param name="coreInterfaceType">The type of a core service interface.</param>
        /// <returns>Null if the service itself isn't registered or a method to create the specified service.</returns>
        [CanBeNull]
        public Func<IServiceProvider, object> GetServiceCreationFunc([NotNull] Type coreInterfaceType)
        {
            Requires.NotNull(coreInterfaceType, nameof(coreInterfaceType));

            Service service = GetService(coreInterfaceType);
            return service?.GetInstanceFunc;
        }

        [CanBeNull]
        private Service GetService([NotNull] Type coreInterfaceType)
        {
            _services.TryGetValue(coreInterfaceType, out Service service);
            return service;
        }

        [NotNull]
        private Service GetRequiredService<TCoreInterface>()
            where TCoreInterface : class
        {
            return GetRequiredService(typeof(TCoreInterface));
        }

        [NotNull]
        private Service GetRequiredService([NotNull] Type coreInterfaceType)
        {
            Service service = GetService(coreInterfaceType);
            return service ?? throw new InvalidOperationException($"{Name} service is not supported by {coreInterfaceType} module.");
        }
    }
}

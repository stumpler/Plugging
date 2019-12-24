using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Validation;

namespace Plugging
{
    /// <summary>
    /// Extension methods for <see cref="Module"/> object. These were intentionally defined as extension methods to show off the case
    /// where a set of core interfaces along with relevant extension methods are placed in a separate assembly.
    /// </summary>
    public static class ModuleExtensions
    {
        /// <summary>
        /// Registers a service for the specified core interface type that is supposed to have  a single implementation.
        /// </summary>
        /// <param name="module">The module info object.</param>
        /// <typeparam name="TCoreInterface">The type of a core interface.</typeparam>
        /// <param name="supportedOperations">Operations supported by a service being registered.
        /// If not specified, all operations are meant to be supported.</param>
        public static void Register<TCoreInterface>(this Module module, [CanBeNull] Enum supportedOperations = default)
            where TCoreInterface : class
        {
            Requires.NotNull(module, nameof(module));

            module.Register(serviceProvider => serviceProvider.GetService<TCoreInterface>(), supportedOperations);
        }

        /// <summary>
        /// Registers a service for the specified core interface type that is supposed to have multiple implementations.
        /// </summary>
        /// <param name="module">The module info object.</param>
        /// <typeparam name="TCoreInterface">The type of a core interface such as IGatewayService.</typeparam>
        /// <typeparam name="TSpecificInterface">The type of an interface or a class to be used when requesting a service instance from IoC container.
        /// In most cases an interface specific to the module and inherited from the core interface is passed in here, e.g. IRippleGatewayService.</typeparam>
        /// <param name="supportedOperations">Operations supported by a service being registered.
        /// If not specified, all operations are meant to be supported.</param>
        public static void Register<TCoreInterface, TSpecificInterface>(this Module module, [CanBeNull] Enum supportedOperations = default)
            where TCoreInterface : class
            where TSpecificInterface : class, TCoreInterface
        {
            Requires.NotNull(module, nameof(module));
            
            module.Register<TCoreInterface>(serviceProvider => serviceProvider.GetService<TSpecificInterface>(), supportedOperations);
        }

        /// <summary>
        /// Retrieves an aggregated composition of operations supported by the specified service.
        /// </summary>
        /// <typeparam name="TCoreInterface">The type of a core service interface to get supported operations for.</typeparam>
        /// <returns>Null if the service itself isn't registered or a flags enumeration representing supported operations.</returns>
        [CanBeNull]
        public static Enum GetSupportedOperations<TCoreInterface>(this Module module)
            where TCoreInterface : class
        {
            Requires.NotNull(module, nameof(module));
         
            return module.GetSupportedOperations(typeof(TCoreInterface));
        }

        /// <summary>
        /// Checks whether it supports the specified core interface.
        /// </summary>
        /// <typeparam name="TCoreInterface">The type of a core interface to check.</typeparam>
        /// <param name="module">The module metadata instance.</param>
        /// <returns>Returns true if the module supports the service.</returns>
        public static bool Supports<TCoreInterface>([NotNull] this Module module)
            where TCoreInterface : class
        {
            Requires.NotNull(module, nameof(module));

            return module.GetSupportedOperations<TCoreInterface>() != null;
        }

        /// <summary>
        /// Checks whether it supports the specified core interface.
        /// </summary>
        /// <param name="coreInterfaceType">The type of a core interface to check.</param>
        /// <param name="module">The module metadata instance.</param>
        /// <returns>Returns true if the module supports the service.</returns>
        public static bool Supports([NotNull] this Module module, [NotNull] Type coreInterfaceType)
        {
            Requires.NotNull(module, nameof(module));

            return module.GetSupportedOperations(coreInterfaceType) != null;
        }

        #region Private Methods

        private static bool Supports<TCoreInterface>([NotNull] this Module module, Enum operation)
            where TCoreInterface : class
        {
            Enum supportedOperations = module.GetSupportedOperations<TCoreInterface>();
            return supportedOperations?.HasFlag(operation) ?? false; 
        }

        #endregion
    }
}

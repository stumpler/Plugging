using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Validation;

namespace Plugging
{
    /// <summary>
    /// Helps to configure services for the specified module.
    /// </summary>
    public class PluggableModuleBuilder
    {
        public PluggableModuleBuilder([NotNull] IServiceCollection services, [NotNull] Module module)
        {
            Requires.NotNull(services, nameof(services));
            Requires.NotNull(module, nameof(module));

            Services = services;
            Module = module;
        }

        /// <summary>
        /// Services to be dependency injected.
        /// </summary>
        [NotNull]
        public IServiceCollection Services { get; }

        /// <summary>
        /// Module being configured.
        /// </summary>
        [NotNull]
        public Module Module { get; }
    }
}

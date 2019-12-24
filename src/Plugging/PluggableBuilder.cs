using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Validation;

namespace Plugging
{
    /// <summary>
    /// Helps to configure InfinXchange providers and their services.
    /// </summary>
    public class PluggableBuilder
    {
        public PluggableBuilder([NotNull] IServiceCollection services)
        {
            Requires.NotNull(services, nameof(services));

            Services = services;
            Options = new PluggableOptions();
        }

        /// <summary>
        /// Services to be dependency injected.
        /// </summary>
        [NotNull]
        public IServiceCollection Services { get; }

        /// <summary>
        /// Options object being configured.
        /// </summary>
        [NotNull]
        public PluggableOptions Options { get; }
    }
}
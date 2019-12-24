using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Validation;

namespace Plugging
{
    public static class PluggableBuilderExtensions
    {
        /// <summary>
        /// Registers a module with the supplied name and configuration.
        /// </summary>
        /// <param name="builder">The underlying builder object.</param>
        /// <param name="name">The module name.</param>
        /// <returns>A builder instance intended to configure the registered module.</returns>
        [NotNull]
        public static PluggableModuleBuilder AddModule(
            [NotNull] this PluggableBuilder builder,
            [NotNull] string name)
        {
            Requires.NotNull(builder, nameof(builder));
            Requires.NotNullOrEmpty(name, nameof(name));

            Module module = builder.Options.AddModule(name);
            return new PluggableModuleBuilder(builder.Services, module);
        }

        [NotNull]
        public static PluggableBuilder AddPlugging([NotNull] this PluggableBuilder builder)
        {
            Requires.NotNull(builder, nameof(builder));

            builder.Services.Configure<PluggableOptions>(options => options.AddModules(builder.Options.Modules.Values));
            builder.Services.TryAddSingleton<IPluggableServiceProvider, PluggableServiceProvider>();
            builder.Services.TryAddSingleton(typeof(IPluggableRegistry<>), typeof(PluggableRegistry<>));
            
            return builder;
        }
    }
}
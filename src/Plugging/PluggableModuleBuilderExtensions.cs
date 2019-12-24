using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Validation;

namespace Plugging
{
    public static class PluggableModuleBuilderExtensions
    {
        public static PluggableModuleBuilder AddService<TService, TImplementation>(
            [NotNull] this PluggableModuleBuilder builder)
            where TService : class
            where TImplementation : class, TService
        {
            Requires.NotNull(builder, nameof(builder));

            return builder.AddService<TService, TImplementation, Enum>(null);
        }

        public static PluggableModuleBuilder AddService<TService, TImplementation, TOperations>(
            [NotNull] this PluggableModuleBuilder builder,
            TOperations unsupportedOperations)
            where TService : class
            where TImplementation : class, TService
            where TOperations : Enum
        {
            Requires.NotNull(builder, nameof(builder));

            TOperations supportedOperations = ConvertToSupportedOperations(unsupportedOperations);
            builder.Module.Register<TService, TImplementation>(supportedOperations);

            builder.Services.AddTransient<TImplementation>();

            return builder;
        }

        private static TOperation ConvertToSupportedOperations<TOperation>(TOperation unsupportedOperations)
            where TOperation : Enum
        {
            if (unsupportedOperations == null)
            {
                return default;
            }

            int supportedOperations = ((int[])Enum.GetValues(typeof(TOperation)))
                .Aggregate((all, next) => all | next);

            int result = supportedOperations ^ Convert.ToInt32(unsupportedOperations);

            return (TOperation)(result as object);
        }
    }
}

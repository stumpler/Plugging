using System;
using Microsoft.Extensions.Options;
using Validation;

namespace Plugging
{
    public class PluggableRegistry<T> : IPluggableRegistry<T>
        where T : class
    {
        private readonly IPluggableServiceProvider _pluggableServiceProvider;
        private readonly PluggableOptions _options;

        public PluggableRegistry(IPluggableServiceProvider pluggableServiceProvider, IOptions<PluggableOptions> options)
        {
            _pluggableServiceProvider = pluggableServiceProvider;
            _options = options.Value;
        }

        public T GetService(string moduleName)
        {
            Requires.NotNullOrEmpty(moduleName, nameof(moduleName));

            return _pluggableServiceProvider.GetService<T>(moduleName);
        }

        public bool Supports<TOperation>(string moduleName, TOperation operation)
            where TOperation : Enum
        {
            Requires.NotNullOrEmpty(moduleName, nameof(moduleName));

            if (_options.Modules.TryGetValue(moduleName, out Module module))
            {
                Enum supportedOperations = module.GetSupportedOperations(typeof(T));

                // If the operation hasn't been specified, check only whether the service is supported or not.
                return supportedOperations != null &&
                    (operation == null || supportedOperations.HasFlag(operation));
            }

            return false;
        }
    }
}

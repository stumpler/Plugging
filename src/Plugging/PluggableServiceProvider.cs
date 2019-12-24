using System;
using Microsoft.Extensions.Options;

namespace Plugging
{
    public class PluggableServiceProvider : IPluggableServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PluggableOptions _options;

        public PluggableServiceProvider(IServiceProvider serviceProvider, IOptions<PluggableOptions> options)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        public T GetService<T>(string moduleName)
            where T : class
        {
            if (_options.Modules.TryGetValue(moduleName, out Module module))
            {
                Func<IServiceProvider, object> serviceCreationFunc = module.GetServiceCreationFunc(typeof(T));

                return serviceCreationFunc?.Invoke(_serviceProvider) as T;
            }

            return null;
        }
    }
}

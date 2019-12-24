using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Validation;

namespace Plugging
{
    /// <summary>
    /// Represents the configuration of ontology modules and their services.
    /// </summary>
    public class PluggableOptions
    {
        private readonly Dictionary<string, Module> _modules = new Dictionary<string, Module>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, object> _properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Registered modules.
        /// </summary>
        [NotNull]
        public IReadOnlyDictionary<string, Module> Modules
        {
            get { return _modules; }
        }

        /// <summary>
        /// Properties to be primarily used in extension methods for the purpose of extending the basic behavior of pluggable options.
        /// For instance, AddIntegrationEvent() extension method might be defined to support infrastructure services.
        /// </summary>
        public IDictionary<string, object> Properties
        {
            get { return _properties; }
        }

        public void AddModules([NotNull] IEnumerable<Module> modules)
        {
            foreach (Module module in modules)
            {
                _modules[module.Name] = module;
            }
        }

        [NotNull]
        public Module AddModule([NotNull] string name)
        {
            Requires.NotNullOrEmpty(name, nameof(name));

            if (!_modules.TryGetValue(name, out Module module))
            {
                module = new Module(name);
                _modules.Add(name, module);
            }

            return module;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backlight.Services {
    public class ServiceOptions : IServiceOptions {
        public Dictionary<Type, ProviderForTypeForTypeOptions> ProvidersForType { get; } = new Dictionary<Type, ProviderForTypeForTypeOptions>();

        public IProviderForTypeOptions For<T>() {
            var provider = new ProviderForTypeForTypeOptions();
            provider.RegisterDelegatesFor<T>();
            ProvidersForType[typeof(T)] = provider;
            return provider;
        }

        public bool CanCreate(string entityFullName) {
            if (!IsProviderConfiguredFor(entityFullName)) return false;
            var key = ProviderConfigurationKeyFrom(entityFullName);
            return ProvidersForType[key].Create != null;
        }

        public bool CanRead(string entityFullName) {
            if (!IsProviderConfiguredFor(entityFullName)) return false;
            var type = ProviderConfigurationKeyFrom(entityFullName);
            return ProvidersForType[type].Read != null;
        }

        public bool CanUpdate(string entityFullName) {
            if (!IsProviderConfiguredFor(entityFullName)) return false;
            var type = ProviderConfigurationKeyFrom(entityFullName);
            return ProvidersForType[type].Update != null;
        }

        public bool CanDelete(string entityFullName) {
            if (!IsProviderConfiguredFor(entityFullName)) return false;
            var type = ProviderConfigurationKeyFrom(entityFullName);
            return ProvidersForType[type].Delete != null;
        }

        private bool IsProviderConfiguredFor(string entityFullName) {
            return ProvidersForType.Keys.Any(entityType => entityType.FullName.Equals(entityFullName));
        }
        
        private Type ProviderConfigurationKeyFrom(string entityFullName) {
            return ProvidersForType.Keys.First(x => x.FullName.Equals(entityFullName));
        }

    }

}
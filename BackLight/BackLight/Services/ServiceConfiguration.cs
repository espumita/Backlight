using System;
using System.Collections.Generic;
using System.Linq;

namespace Backlight.Services {
    public class ServiceConfiguration {
        public Dictionary<Type, ProvidersConfiguration> ProvidersConfiguration { get; } = new Dictionary<Type, ProvidersConfiguration>();

        public ProvidersConfiguration For<T>() {
            var providersConfiguration = new ProvidersConfiguration();
            providersConfiguration.RegisterCreationDelegationFor<T>()
                .RegisterReadDelegationFor<T>()
                .RegisterUpdateDelegationFor<T>()
                .RegisterDeleteDelegationFor<T>();
            ProvidersConfiguration[typeof(T)] = providersConfiguration;
            return providersConfiguration;
        }

        public bool CanCreate(string entityFullName) {
            if (!IsProviderConfiguredFor(entityFullName)) return false;
            var key = ProviderConfigurationKeyFrom(entityFullName);
            return ProvidersConfiguration[key].CreateProvider != null;
        }

        public bool CanRead(string entityFullName) {
            if (!IsProviderConfiguredFor(entityFullName)) return false;
            var key = ProviderConfigurationKeyFrom(entityFullName);
            return ProvidersConfiguration[key].ReadProvider != null;
        }

        public bool CanUpdate(string entityFullName) {
            if (!IsProviderConfiguredFor(entityFullName)) return false;
            var key = ProviderConfigurationKeyFrom(entityFullName);
            return ProvidersConfiguration[key].UpdateProvider != null;
        }

        public bool CanDelete(string entityFullName) {
            if (!IsProviderConfiguredFor(entityFullName)) return false;
            var key = ProviderConfigurationKeyFrom(entityFullName);
            return ProvidersConfiguration[key].DeleteProvider != null;
        }

        private bool IsProviderConfiguredFor(string entityFullName) {
            return ProvidersConfiguration.Keys.Any(entityType => entityType.FullName.Equals(entityFullName));
        }
        
        private Type ProviderConfigurationKeyFrom(string entityFullName) {
            return ProvidersConfiguration.Keys.First(x => x.FullName.Equals(entityFullName));
        }

    }

}
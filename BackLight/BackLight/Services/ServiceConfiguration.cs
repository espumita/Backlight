using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Backlight.Services {
    public class ServiceConfiguration {
        public Dictionary<Type, ProvidersConfiguration> ProvidersConfiguration { get; } = new Dictionary<Type, ProvidersConfiguration>();
        public Dictionary<Type, Action<string, string>> Update { get; } = new Dictionary<Type, Action<string, string>>();
        public Dictionary<Type, Action<string>> Delete { get; } = new Dictionary<Type, Action<string>>();


        public ProvidersConfiguration For<T>() {
            ProvidersConfiguration[typeof(T)] = new ProvidersConfiguration();
            ProvidersConfiguration[typeof(T)].RegisterCreationDelegationFor<T>();
            ProvidersConfiguration[typeof(T)].RegisterReadDelegationFor<T>();
            Update[typeof(T)] = (entityId, entityPayload) => {
                var entity = JsonSerializer.Deserialize<T>(entityPayload);
                ProvidersConfiguration[typeof(T)].UpdateProvider.Update(entityId, entity);
            };
            Delete[typeof(T)] = (entityId) => {
                ProvidersConfiguration[typeof(T)].DeleteProvider.Delete<T>(entityId);
            };
            return ProvidersConfiguration[typeof(T)];
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
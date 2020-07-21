﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Backlight.Services {
    public class ServiceOptions : IServiceOptions {
        public Dictionary<Type, ProviderOptions> ProvidersOptions { get; } = new Dictionary<Type, ProviderOptions>();

        public IProviderOptions For<T>() {
            var providerOptions = new ProviderOptions();
            providerOptions.RegisterDelegatesFor<T>();
            ProvidersOptions[typeof(T)] = providerOptions;
            return providerOptions;
        }

        public bool CanCreate(string entityFullName) {
            if (!IsProviderConfiguredFor(entityFullName)) return false;
            var key = ProviderConfigurationKeyFrom(entityFullName);
            return ProvidersOptions[key].Create != null;
        }

        public bool CanRead(string entityFullName) {
            if (!IsProviderConfiguredFor(entityFullName)) return false;
            var key = ProviderConfigurationKeyFrom(entityFullName);
            return ProvidersOptions[key].Read != null;
        }

        public bool CanUpdate(string entityFullName) {
            if (!IsProviderConfiguredFor(entityFullName)) return false;
            var key = ProviderConfigurationKeyFrom(entityFullName);
            return ProvidersOptions[key].Update != null;
        }

        public bool CanDelete(string entityFullName) {
            if (!IsProviderConfiguredFor(entityFullName)) return false;
            var key = ProviderConfigurationKeyFrom(entityFullName);
            return ProvidersOptions[key].Delete != null;
        }

        private bool IsProviderConfiguredFor(string entityFullName) {
            return ProvidersOptions.Keys.Any(entityType => entityType.FullName.Equals(entityFullName));
        }
        
        private Type ProviderConfigurationKeyFrom(string entityFullName) {
            return ProvidersOptions.Keys.First(x => x.FullName.Equals(entityFullName));
        }

    }

}
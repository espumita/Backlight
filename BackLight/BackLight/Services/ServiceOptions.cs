using System;
using System.Collections.Generic;
using Backlight.Exceptions;

namespace Backlight.Services {
    public class ServiceOptions : IServiceOptions {
        public Dictionary<Type, ProviderForTypeOptions> ProvidersForType { get; }

        public ServiceOptions() {
            ProvidersForType = new Dictionary<Type, ProviderForTypeOptions>();
        }

        public IProviderForTypeOptions For<T>() {
            CheckIfExistsConfigurationForType<T>();
            var provider = new ProviderForTypeOptions();
            provider.RegisterDelegatesFor<T>();
            ProvidersForType[typeof(T)] = provider;
            return provider;
        }

        private void CheckIfExistsConfigurationForType<T>() {
            if (ProvidersForType.ContainsKey(typeof(T))) throw new CannotConfigureTheSameEntityTwiceException();
        }

    }
}
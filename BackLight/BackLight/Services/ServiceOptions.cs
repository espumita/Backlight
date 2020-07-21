using System;
using System.Collections.Generic;

namespace Backlight.Services {
    public class ServiceOptions : IServiceOptions {
        public Dictionary<Type, ProviderForTypeForTypeOptions> ProvidersForType { get; }

        public ServiceOptions() {
            ProvidersForType = new Dictionary<Type, ProviderForTypeForTypeOptions>();
        }

        public IProviderForTypeOptions For<T>() {
            var provider = new ProviderForTypeForTypeOptions();
            provider.RegisterDelegatesFor<T>();
            ProvidersForType[typeof(T)] = provider;
            return provider;
        }

    }

}
using System;
using System.Collections.Generic;
using Backlight.Exceptions;
using Backlight.Services.EntitySerialization;

namespace Backlight.Services {
    public class ServiceOptions : IServiceOptions {
        private EntitySerializer entitySerializer;
        public Dictionary<Type, ProviderForTypeOptions> ProvidersForType { get; }

        public ServiceOptions(EntitySerializer entitySerializer) {
            this.entitySerializer = entitySerializer;
            ProvidersForType = new Dictionary<Type, ProviderForTypeOptions>();
        }

        public IProviderForTypeOptions For<T>() where T : class, BacklightEntity {
            CheckIfExistsConfigurationForType<T>();
            var provider = new ProviderForTypeOptions();
            provider.RegisterDelegatesFor<T>(entitySerializer);
            ProvidersForType[typeof(T)] = provider;
            return provider;
        }

        private void CheckIfExistsConfigurationForType<T>() {
            if (ProvidersForType.ContainsKey(typeof(T))) throw new CannotConfigureTheSameEntityTwiceException();
        }

    }

}
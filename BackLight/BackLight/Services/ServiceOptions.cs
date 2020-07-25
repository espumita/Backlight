using System;
using System.Collections.Generic;
using System.Reflection;
using Backlight.Exceptions;

namespace Backlight.Services {
    public class ServiceOptions : IServiceOptions {
        public Dictionary<Type, ProviderForTypeOptions> ProvidersForType { get; }
        public Dictionary<string, Assembly> AssemblyForType { get; }

        public ServiceOptions() {
            ProvidersForType = new Dictionary<Type, ProviderForTypeOptions>();
            AssemblyForType = new Dictionary<string, Assembly>();
        }

        public IProviderForTypeOptions For<T>() {
            CheckIfExistsConfigurationForType<T>();
            var assembly = Assembly.GetAssembly(typeof(T));
            AssemblyForType.Add(typeof(T).FullName, assembly);
            var provider = new ProviderForTypeOptions();
            provider.RegisterDelegatesFor<T>(new JsonEntitySerializer());
            ProvidersForType[typeof(T)] = provider;
            return provider;
        }

        private void CheckIfExistsConfigurationForType<T>() {
            if (ProvidersForType.ContainsKey(typeof(T))) throw new CannotConfigureTheSameEntityTwiceException();
        }

    }

}
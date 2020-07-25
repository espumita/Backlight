using System;
using System.Linq;
using System.Reflection;
using Backlight.Exceptions;

namespace Backlight.Services {
    public class BacklightService {
        public ServiceOptions Options { get; set; }

        public BacklightService(ServiceOptions options) {
            Options = options;
        }

        public virtual Action<string> CreateProviderFor(Type type) {
            var type2 = ProviderForTypeFrom(type);
            if (!Options.ProvidersForType[type2].CanCreate()) throw new EntityProviderIsNotAvailableException();
            var providersConfiguration = Options.ProvidersForType[type2];
            return providersConfiguration.CreateDelegate;
        }

        public virtual Func<string, string> ReaderProviderFor(Type type) {
            var type2 = ProviderForTypeFrom(type);
            if (!Options.ProvidersForType[type2].CanRead()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type2];
            return backlightServicesProviderOptions.ReadDelegate;
        }

        public virtual Action<string, string> UpdateProviderFor(Type type) {
            var type2 = ProviderForTypeFrom(type);
            if (!Options.ProvidersForType[type2].CanUpdate()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type2];
            return backlightServicesProviderOptions.UpdateDelegate;
        }

        public virtual Action<string> DeleteProviderFor(Type type) {
            var type2 = ProviderForTypeFrom(type);
            if (!Options.ProvidersForType[type2].CanDelete()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type2];
            return backlightServicesProviderOptions.DeleteDelegate;
        }

        public virtual Assembly GetsAssemblyFor(string entityFullName) {
            return Options.AssemblyForType[entityFullName];
        }

        private Type ProviderForTypeFrom(Type type) {
            var providerForTypeFrom = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType == type);
            if (providerForTypeFrom == null) throw new EntityIsNotConfiguredException();
            return providerForTypeFrom;
        }
    }
}
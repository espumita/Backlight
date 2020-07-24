using System;
using System.Linq;
using Backlight.Exceptions;

namespace Backlight.Services {
    public class BacklightService {
        public ServiceOptions Options { get; set; }

        public BacklightService(ServiceOptions options) {
            Options = options;
        }

        public virtual Action<string> CreateProviderFor(string entity) {
            var type = ProviderForTypeFrom(entity);
            if (!Options.ProvidersForType[type].CanCreate()) throw new EntityProviderIsNotAvailableException();
            var providersConfiguration = Options.ProvidersForType[type];
            return providersConfiguration.CreateDelegate;
        }

        public virtual Func<string, string> ReaderProviderFor(string entity) {
            var type = ProviderForTypeFrom(entity);
            if (!Options.ProvidersForType[type].CanRead()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.ReadDelegate;
        }

        public virtual Action<string, string> UpdateProviderFor(string entity) {
            var type = ProviderForTypeFrom(entity);
            if (!Options.ProvidersForType[type].CanUpdate()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.UpdateDelegate;
        }

        public virtual Action<string> DeleteProviderFor(string entity) {
            var type = ProviderForTypeFrom(entity);
            if (!Options.ProvidersForType[type].CanDelete()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.DeleteDelegate;
        }

        private Type ProviderForTypeFrom(string entity) {
            var providerForTypeFrom = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            if (providerForTypeFrom == null) throw new EntityIsNotConfiguredException();
            return providerForTypeFrom;
        }
    }
}
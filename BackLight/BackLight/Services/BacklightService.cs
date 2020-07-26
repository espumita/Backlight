using System;
using System.Linq;
using Backlight.Api.Serialization;
using Backlight.Exceptions;

namespace Backlight.Services {
    public class BacklightService {
        public ServiceOptions Options { get; set; }

        public BacklightService(ServiceOptions options) {
            Options = options;
        }

        public virtual Action<string> CreateProviderFor(EntityPayload entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityPayload.TypeName);
            if (!Options.ProvidersForType[type].CanCreate()) throw new EntityProviderIsNotAvailableException();
            var providersConfiguration = Options.ProvidersForType[type];
            return providersConfiguration.CreateDelegate;
        }

        public virtual Func<string, string> ReaderProviderFor(EntityPayload entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityPayload.TypeName);
            if (!Options.ProvidersForType[type].CanRead()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.ReadDelegate;
        }

        public virtual Action<string, string> UpdateProviderFor(EntityPayload entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityPayload.TypeName);
            if (!Options.ProvidersForType[type].CanUpdate()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.UpdateDelegate;
        }

        public virtual Action<string> DeleteProviderFor(EntityPayload entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityPayload.TypeName);
            if (!Options.ProvidersForType[type].CanDelete()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.DeleteDelegate;
        }

        private Type TryToGetConfiguredTypeFrom(string typeName) {
            var isEntityConfigured = Options.AssemblyForType.Keys.Contains(typeName);
            if (!isEntityConfigured) throw new EntityIsNotConfiguredException();
            var assembly = Options.AssemblyForType[typeName];
            return assembly.GetType(typeName);
       
        }

    }
}
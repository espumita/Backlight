using System;
using System.Linq;

namespace Backlight.Services {
    public class BacklightService {
        public ServiceOptions Options { get; set; }

        public BacklightService(ServiceOptions options) {
            Options = options;
        }

        public virtual bool IsEntityConfiguredFor(string entity) {
            return Options.ProvidersForType.Keys.Any(entityType => entityType.Name.Equals(entity));
        }

        public virtual Action<string> CreateProviderFor(string entity) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var providersConfiguration = Options.ProvidersForType[type];
            return providersConfiguration.CreateDelegate;
        }

        public virtual Func<string, string> ReaderProviderFor(string entity) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.ReadDelegate;
        }

        public virtual Action<string, string> UpdateProviderFor(string entity) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.UpdateDelegate;
        }

        public virtual Action<string> DeleteProviderFor(string entity) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.DeleteDelegate;
        }

        public virtual bool CanCreate(string entity) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var providerForType = Options.ProvidersForType[type];
            return providerForType.CanCreate();
        }

        public virtual bool CanRead(string entity) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var providerForType = Options.ProvidersForType[type];
            return providerForType.CanRead();
        }

        public virtual bool CanUpdate(string entity) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var providerForType = Options.ProvidersForType[type];
            return providerForType.CanUpdate();
        }

        public virtual bool CanDelete(string entity) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var providerForType = Options.ProvidersForType[type];
            return providerForType.CanDelete();
        }
    }
}
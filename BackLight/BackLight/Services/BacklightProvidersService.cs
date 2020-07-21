using System;
using System.Linq;
using System.Text.Json;

namespace Backlight.Services {
    public class BacklightProvidersService {
        public ServiceOptions Options { get; private set; }

        public BacklightProvidersService(ServiceOptions options) {
            Options = options;
        }

        public string EntitiesToInject() {
            var entities = Options.ProvidersOptions.Keys.Select(key => {
                var entityProvidersOptions = Options.ProvidersOptions[key];
                var canCreate = Options.CanCreate(key.FullName);
                var canRead = Options.CanRead(key.FullName);
                var canUpdate = Options.CanUpdate(key.FullName);
                var canDelete = Options.CanDelete(key.FullName);
                return new HtmlEntity(key.Name.ToString(), canCreate, canRead, canUpdate, canDelete);
            }).ToList();
            return JsonSerializer.Serialize(entities);
        }

        public virtual bool IsEntityConfiguredFor(string entity) {
            return Options.ProvidersOptions.Keys.Any(entityType => entityType.Name.Equals(entity));
        }

        public virtual bool IsProviderAvailableFor(string entity, string httpMethod) {
            var type = Options.ProvidersOptions.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersOptions[type];
            return false; //TODO CHECK Configurations can methods
        }

        public virtual Action<string> CreateProviderFor(string entity, string httpMethod) {
            var type = Options.ProvidersOptions.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var providersConfiguration = Options.ProvidersOptions[type];
            return providersConfiguration.CreateDelegate;
        }

        public virtual Func<string, string> ReaderProviderFor(string entity, string httpMethod) {
            var type = Options.ProvidersOptions.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersOptions[type];
            return backlightServicesProviderOptions.ReadDelegate;
        }

        public virtual Action<string, string> UpdateProviderFor(string entity, string httpMethod) {
            var type = Options.ProvidersOptions.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersOptions[type];
            return backlightServicesProviderOptions.UpdateDelegate;
        }

        public virtual Action<string> DeleteProviderFor(string entity, string httpMethod) {
            var type = Options.ProvidersOptions.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersOptions[type];
            return backlightServicesProviderOptions.DeleteDelegate;
        }

    }


}
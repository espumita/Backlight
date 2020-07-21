using System;
using System.Linq;
using System.Text.Json;

namespace Backlight.Services {
    public class BacklightProvidersService {
        public BacklightServiceOptions Options { get; private set; }

        public BacklightProvidersService(BacklightServiceOptions options) {
            this.Options = options;
        }

        public string EntitiesToInject() {
            var entities = Options.ProvidersConfiguration.Keys.Select(key => {
                var entityProvidersOptions = Options.ProvidersConfiguration[key];
                var canCreate = Options.CanCreate(key.FullName);
                var canRead = Options.CanRead(key.FullName);
                var canUpdate = Options.CanUpdate(key.FullName);
                var canDelete = Options.CanDelete(key.FullName);
                return new HtmlEntity(key.Name.ToString(), canCreate, canRead, canUpdate, canDelete);
            }).ToList();
            return JsonSerializer.Serialize(entities);
        }

        public virtual bool IsEntityConfiguredFor(string entity) {
            return Options.ProvidersConfiguration.Keys.Any(entityType => entityType.Name.Equals(entity));
        }

        public virtual bool IsProviderAvailableFor(string entity, string httpMethod) {
            var type = Options.ProvidersConfiguration.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersConfiguration[type];
            return false; //TODO CHECK Configurations can methods
        }

        public virtual Action<string> CreateProviderFor(string entity, string httpMethod) {
            var type = Options.ProvidersConfiguration.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var providersConfiguration = Options.ProvidersConfiguration[type];
            return providersConfiguration.Create;
        }

        public virtual Func<string, string> ReaderProviderFor(string entity, string httpMethod) {
            var type = Options.ProvidersConfiguration.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersConfiguration[type];
            return backlightServicesProviderOptions.Read;
        }

        public virtual Action<string, string> UpdateProviderFor(string entity, string httpMethod) {
            var type = Options.ProvidersConfiguration.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersConfiguration[type];
            return backlightServicesProviderOptions.Update;
        }

        public virtual Action<string> DeleteProviderFor(string entity, string httpMethod) {
            var type = Options.ProvidersConfiguration.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersConfiguration[type];
            return backlightServicesProviderOptions.Delete;
        }

    }


}
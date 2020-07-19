using System;
using System.Linq;
using System.Text.Json;

namespace Backlight.Services {
    public class BacklightProvidersService {
        public ServiceConfiguration Configuration { get; private set; }

        public BacklightProvidersService(ServiceConfiguration configuration) {
            this.Configuration = configuration;
        }

        public string EntitiesToInject() {
            var entities = Configuration.ProvidersConfiguration.Keys.Select(key => {
                var entityProvidersOptions = Configuration.ProvidersConfiguration[key];
                var canCreate = Configuration.CanCreate(key.FullName);
                var canRead = Configuration.CanRead(key.FullName);
                var canUpdate = Configuration.CanUpdate(key.FullName);
                var canDelete = Configuration.CanDelete(key.FullName);
                return new HtmlEntity(key.Name.ToString(), canCreate, canRead, canUpdate, canDelete);
            }).ToList();
            return JsonSerializer.Serialize(entities);
        }

        public virtual bool IsEntityConfiguredFor(string entity) {
            return Configuration.ProvidersConfiguration.Keys.Any(entityType => entityType.Name.Equals(entity));
        }

        public virtual bool IsProviderAvailableFor(string entity, string httpMethod) {
            var type = Configuration.ProvidersConfiguration.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Configuration.ProvidersConfiguration[type];
            return false; //TODO CHECK Configurations can methods
        }

        public virtual Action<string> CreateProviderFor(string entity, string httpMethod) {
            var type = Configuration.ProvidersConfiguration.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var providersConfiguration = Configuration.ProvidersConfiguration[type];
            return providersConfiguration.Create;
        }

        public virtual Func<string, string> ReaderProviderFor(string entity, string httpMethod) {
            var type = Configuration.ProvidersConfiguration.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Configuration.ProvidersConfiguration[type];
            return backlightServicesProviderOptions.Read;
        }

        public virtual Action<string, string> UpdateProviderFor(string entity, string httpMethod) {
            var type = Configuration.ProvidersConfiguration.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Configuration.ProvidersConfiguration[type];
            return backlightServicesProviderOptions.Update;
        }

        public virtual Action<string> DeleteProviderFor(string entity, string httpMethod) {
            var type = Configuration.ProvidersConfiguration.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Configuration.ProvidersConfiguration[type];
            return backlightServicesProviderOptions.Delete;
        }

    }


}
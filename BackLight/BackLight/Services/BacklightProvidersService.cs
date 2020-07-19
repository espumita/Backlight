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
            var entities = Configuration.Providers.Keys.Select(key => {
                var entityProvidersOptions = Configuration.Providers[key];
                var canCreate = entityProvidersOptions.CanCreate();
                var canRead = entityProvidersOptions.CanRead();
                var canUpdate = entityProvidersOptions.CanUpdate();
                var canDelete = entityProvidersOptions.CanDelete();
                return new HtmlEntity(key.Name.ToString(), canCreate, canRead, canUpdate, canDelete);
            }).ToList();
            return JsonSerializer.Serialize(entities);
        }

        public virtual bool IsEntityConfiguredFor(string entity) {
            return Configuration.Providers.Keys.Any(entityType => entityType.Name.Equals(entity));
        }

        public virtual bool IsProviderAvailableFor(string entity, string httpMethod) {
            var type = Configuration.Providers.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Configuration.Providers[type];
            return backlightServicesProviderOptions.CanCreate();
        }

        public virtual Action<string> CreateProviderFor(string entity, string httpMethod) {
            var type = Configuration.CreateProvidersDelegates.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Configuration.CreateProvidersDelegates[type];
            return backlightServicesProviderOptions;
        }

        public virtual Func<string, string> ReaderProviderFor(string entity, string httpMethod) {
            var type = Configuration.ReadProvidersDelegates.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Configuration.ReadProvidersDelegates[type];
            return backlightServicesProviderOptions;
        }

        public virtual Action<string, string> UpdateProviderFor(string entity, string httpMethod) {
            var type = Configuration.UpdateProvidersDelegates.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Configuration.UpdateProvidersDelegates[type];
            return backlightServicesProviderOptions;
        }

        public virtual Action<string> DeleteProviderFor(string entity, string httpMethod) {
            var type = Configuration.DeleteProvidersDelegates.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Configuration.DeleteProvidersDelegates[type];
            return backlightServicesProviderOptions;
        }

    }


}
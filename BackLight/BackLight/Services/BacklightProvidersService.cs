using System;
using System.Linq;
using System.Text.Json;
using Backlight.Providers;

namespace Backlight.Services {
    public class BacklightProvidersService {
        private readonly BacklightServicesOptions options;

        public BacklightProvidersService(BacklightServicesOptions options) {
            this.options = options;
        }

        public string EntitiesToInject() {
            var entities = options.Providers.Keys.Select(key => {
                var entityProvidersOptions = options.Providers[key];
                var canCreate = entityProvidersOptions.CanCreate();
                var canRead = entityProvidersOptions.CanRead();
                var canUpdate = entityProvidersOptions.CanUpdate();
                var canDelete = entityProvidersOptions.CanDelete();
                return new HtmlEntity(key.FullName.ToString(), canCreate, canRead, canUpdate, canDelete);
            }).ToList();
            return JsonSerializer.Serialize(entities);
        }

        public virtual bool IsEntityConfiguredFor(string entity) {
            return options.Providers.Keys.Any(entityType => entityType.Name.Equals(entity));
        }

        public virtual bool IsProviderAvailableFor(string entity, string httpMethod) {
            var type = options.Providers.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = options.Providers[type];
            return backlightServicesProviderOptions.CanCreate();
        }

        public virtual Action<string> ProviderFor(string entity, string httpMethod) {
            var type = options.CreateProvidersDelegates.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = options.CreateProvidersDelegates[type];
            return backlightServicesProviderOptions;
        }

        public virtual Func<string, string> ReaderProviderFor(string entity, string httpMethod) {
            var type = options.ReadProvidersDelegates.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = options.ReadProvidersDelegates[type];
            return backlightServicesProviderOptions;
        }

    }


}
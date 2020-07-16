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
            return options.Providers.Keys.Any(entityType => entityType.FullName.Equals(entity));
        }

        public bool CanCreate(string entity) {
            var type = options.Providers.Keys.FirstOrDefault(entityType => entityType.FullName.Equals(entity));
            var backlightServicesProviderOptions = options.Providers[type];
            return backlightServicesProviderOptions.CanCreate();
        }

        public CreateProvider CreateProvider(string entity) {
            var type = options.Providers.Keys.FirstOrDefault(entityType => entityType.FullName.Equals(entity));
            var backlightServicesProviderOptions = options.Providers[type];
            return backlightServicesProviderOptions.CreateProvider;
        }

        public Type GetType(string entity) {
            return Type.GetType(entity);
            var type = options.Providers.Keys.FirstOrDefault(entityType => entityType.FullName.Equals(entity));
            return type;
        }
    }


}
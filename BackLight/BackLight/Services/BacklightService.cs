using System;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Backlight.Services {
    public class BacklightService {
        public ServiceOptions Options { get; private set; }

        public BacklightService(ServiceOptions options) {
            Options = options;
        }

        public string EntitiesToInject() {
            var entities = Options.ProvidersForType.Keys.Select(type => {
                var providerForType = Options.ProvidersForType[type];
                var canCreate = providerForType.CanCreate();
                var canRead = providerForType.CanRead();
                var canUpdate = providerForType.CanUpdate();
                var canDelete = providerForType.CanDelete();
                return new HtmlEntity(type.Name.ToString(), canCreate, canRead, canUpdate, canDelete);
            }).ToList();
            return JsonSerializer.Serialize(entities);
        }

        public virtual bool IsEntityConfiguredFor(string entity) {
            return Options.ProvidersForType.Keys.Any(entityType => entityType.Name.Equals(entity));
        }

        public virtual bool IsProviderAvailableFor(string entity, string httpMethod) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var providerForType = Options.ProvidersForType[type];
            if (httpMethod == HttpMethods.Put && providerForType.CanCreate()) return true;
            if (httpMethod == HttpMethods.Get && providerForType.CanRead()) return true;
            if (httpMethod == HttpMethods.Post && providerForType.CanUpdate()) return true;
            if (httpMethod == HttpMethods.Delete && providerForType.CanDelete()) return true;
            return false;
        }

        public virtual Action<string> CreateProviderFor(string entity, string httpMethod) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var providersConfiguration = Options.ProvidersForType[type];
            return providersConfiguration.CreateDelegate;
        }

        public virtual Func<string, string> ReaderProviderFor(string entity, string httpMethod) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.ReadDelegate;
        }

        public virtual Action<string, string> UpdateProviderFor(string entity, string httpMethod) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.UpdateDelegate;
        }

        public virtual Action<string> DeleteProviderFor(string entity, string httpMethod) {
            var type = Options.ProvidersForType.Keys.FirstOrDefault(entityType => entityType.Name.Equals(entity));
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.DeleteDelegate;
        }

    }


}
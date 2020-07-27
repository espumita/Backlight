using System;
using System.Linq;
using System.Threading.Tasks;
using Backlight.Api.Serialization;
using Backlight.Exceptions;

namespace Backlight.Services {
    public class BacklightService {
        public ServiceOptions Options { get; set; }

        public BacklightService(ServiceOptions options) {
            Options = options;
        }

        public virtual async Task<string> Create(string entityTypeName, string entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityTypeName);
            var createDelegate = TryToGetCreateDelegateForType(type);
            return await createDelegate(entityPayload);
        }

        public virtual async Task<string> Read(string entityTypeName, string entityId) {
            var type = TryToGetConfiguredTypeFrom(entityTypeName);
            var readDelegate = TryToGetReadDelegateForType(type);
            return await readDelegate(entityId);
        }

        public virtual async Task Update(string entityTypeName, string entityId, string entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityTypeName);
            var updateDelegate = TryToGetUpdateDelegateForType(type);
            await updateDelegate(entityId, entityPayload);
        }

        public virtual async Task Delete(string entityTypeName, string entityId) {
            var type = TryToGetConfiguredTypeFrom(entityTypeName);
            var deleteDelegate = TryToGetDeleteDelegateForType(type);
            await deleteDelegate(entityId);
        }

        private Type TryToGetConfiguredTypeFrom(string typeName) {
            var isEntityConfigured = Options.ProvidersForType.Keys.Any(type  => type.FullName.Equals(typeName));
            if (!isEntityConfigured) throw new EntityIsNotConfiguredException();
            var typeForName = Options.ProvidersForType.Keys.FirstOrDefault(type => type.FullName.Equals(typeName));
            return typeForName;
        }

        private Func<string, Task<string>> TryToGetCreateDelegateForType(Type type) {
            if (!Options.ProvidersForType[type].CanCreate()) throw new EntityProviderIsNotAvailableException();
            return Options.ProvidersForType[type].CreateDelegate;
        }

        private Func<string, Task<string>> TryToGetReadDelegateForType(Type type) {
            if (!Options.ProvidersForType[type].CanRead()) throw new EntityProviderIsNotAvailableException();
            return Options.ProvidersForType[type].ReadDelegate;
        }

        private Func<string, string, Task> TryToGetUpdateDelegateForType(Type type) {
            if (!Options.ProvidersForType[type].CanUpdate()) throw new EntityProviderIsNotAvailableException();
            return Options.ProvidersForType[type].UpdateDelegate;
        }

        private Func<string, Task> TryToGetDeleteDelegateForType(Type type) {
            if (!Options.ProvidersForType[type].CanDelete()) throw new EntityProviderIsNotAvailableException();
            return Options.ProvidersForType[type].DeleteDelegate;
        }

    }
}
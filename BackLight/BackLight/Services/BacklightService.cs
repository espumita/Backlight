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

        public virtual async Task Create(EntityPayload entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityPayload.TypeName);
            var createDelegate = TryToGetCreateDelegateForType(type);
            await createDelegate(entityPayload.Value);
        }

        public virtual async Task<string> Read(EntityPayload entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityPayload.TypeName);
            var readDelegate = TryToGetReadDelegateForType(type);
            return await readDelegate(entityPayload.Value);
        }

        public virtual async Task Update(EntityPayload entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityPayload.TypeName);
            var updateDelegate = TryToGetUpdateDelegateForType(type);
            await updateDelegate("TODOEntityId", entityPayload.Value);
        }

        public virtual async Task Delete(EntityPayload entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityPayload.TypeName);
            var deleteDelegate = TryToGetDeleteDelegateForType(type);
            await deleteDelegate(entityPayload.Value);
        }

        private Type TryToGetConfiguredTypeFrom(string typeName) {
            var isEntityConfigured = Options.AssemblyForType.Keys.Contains(typeName);
            if (!isEntityConfigured) throw new EntityIsNotConfiguredException();
            var assembly = Options.AssemblyForType[typeName];
            return assembly.GetType(typeName);
        }

        private Func<string, Task> TryToGetCreateDelegateForType(Type type) {
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
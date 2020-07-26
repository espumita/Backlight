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
            if (!Options.ProvidersForType[type].CanCreate()) throw new EntityProviderIsNotAvailableException();
            var providersConfiguration = Options.ProvidersForType[type];
            var create = providersConfiguration.CreateDelegate;
            await create(entityPayload.Value);
        }

        public virtual async Task<string> Read(EntityPayload entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityPayload.TypeName);
            if (!Options.ProvidersForType[type].CanRead()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            var read = backlightServicesProviderOptions.ReadDelegate;
            return await read(entityPayload.Value);
        }

        public virtual async Task Update(EntityPayload entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityPayload.TypeName);
            if (!Options.ProvidersForType[type].CanUpdate()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            var update = backlightServicesProviderOptions.UpdateDelegate;
            await update("TODOEntityId", entityPayload.Value);
        }

        public virtual async Task Delete(EntityPayload entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityPayload.TypeName);
            if (!Options.ProvidersForType[type].CanDelete()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            var delete = backlightServicesProviderOptions.DeleteDelegate;
            await delete(entityPayload.Value);
        }

        private Type TryToGetConfiguredTypeFrom(string typeName) {
            var isEntityConfigured = Options.AssemblyForType.Keys.Contains(typeName);
            if (!isEntityConfigured) throw new EntityIsNotConfiguredException();
            var assembly = Options.AssemblyForType[typeName];
            return assembly.GetType(typeName);
       
        }

    }
}
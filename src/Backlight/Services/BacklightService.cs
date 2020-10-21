using System;
using System.Linq;
using System.Threading.Tasks;
using Backlight.Exceptions;
using Backlight.Providers;
using Backlight.Services.EntitySerialization;

namespace Backlight.Services {
    public class BacklightService {
        private readonly EntitySerializer entitySerializer;
        public ServiceOptions Options { get; set; }

        public BacklightService(ServiceOptions options, EntitySerializer entitySerializer) {
            this.entitySerializer = entitySerializer;
            Options = options;
        }

        public virtual async Task<string> ReadAllIds(string entityTypeName) {
            var type = TryToGetConfiguredTypeFrom(entityTypeName);
            var getAllIdsProvider = TryToGetReadAllIdsProviderForType(type);
            var allEntitiesIds = await getAllIdsProvider.ReadAllIds();
            return entitySerializer.Serialize(allEntitiesIds, allEntitiesIds.GetType());
        }

        public virtual async Task<string> Create(string entityTypeName, string entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityTypeName);
            var createProvider = TryToGetCreateProvider(type);
            var entity = entitySerializer.Deserialize(entityPayload, type);
            return await createProvider.Create(entity);
        }

        public virtual async Task<string> Read(string entityTypeName, string entityId) {
            var type = TryToGetConfiguredTypeFrom(entityTypeName);
            var readProvider = TryToGetReadProviderForType(type);
            var entity = await readProvider.Read(entityId, type);
            return entitySerializer.Serialize(entity, type);
        }

        public virtual async Task Update(string entityTypeName, string entityId, string entityPayload) {
            var type = TryToGetConfiguredTypeFrom(entityTypeName);
            var updateProvider = TryToGetUpdateProviderForType(type);
            var entity = entitySerializer.Deserialize(entityPayload, type);
            await updateProvider.Update(entityId, entity);
        }

        public virtual async Task Delete(string entityTypeName, string entityId) {
            var type = TryToGetConfiguredTypeFrom(entityTypeName);
            var deleteDelegate = TryToGetDeleteProviderForType(type);
            await deleteDelegate.Delete(entityId);

        }

        private Type TryToGetConfiguredTypeFrom(string typeName) {
            var isEntityConfigured = Options.ProvidersForType.Keys.Any(type  => type.FullName.Equals(typeName));
            if (!isEntityConfigured) throw new EntityIsNotConfiguredException();
            var typeForName = Options.ProvidersForType.Keys.FirstOrDefault(type => type.FullName.Equals(typeName));
            return typeForName;
        }
        private ReadAllIdsProvider TryToGetReadAllIdsProviderForType(Type type) {
            if (!Options.ProvidersForType[type].CanReadAllIds()) throw new EntityProviderIsNotAvailableException();
            return Options.ProvidersForType[type].ReadAllIdses;
        }

        private CreateProvider TryToGetCreateProvider(Type type) {
            if (!Options.ProvidersForType[type].CanCreate()) throw new EntityProviderIsNotAvailableException();
            return Options.ProvidersForType[type].Create;
        }

        private ReadProvider TryToGetReadProviderForType(Type type) {
            if (!Options.ProvidersForType[type].CanRead()) throw new EntityProviderIsNotAvailableException();
            return Options.ProvidersForType[type].Read;
        }

        private UpdateProvider TryToGetUpdateProviderForType(Type type) {
            if (!Options.ProvidersForType[type].CanUpdate()) throw new EntityProviderIsNotAvailableException();
            return Options.ProvidersForType[type].Update;
        }

        private DeleteProvider TryToGetDeleteProviderForType(Type type) {
            if (!Options.ProvidersForType[type].CanDelete()) throw new EntityProviderIsNotAvailableException();
            return Options.ProvidersForType[type].Delete;
        }

    }
}
using System;
using System.Threading.Tasks;
using Backlight.Exceptions;
using Backlight.Providers;
using Backlight.Services.EntitySerialization;

namespace Backlight.Services {
    public class ProviderForTypeOptions : IProviderForTypeOptions {
        public CreateProvider Create { get; private set; }
        public ReadProvider Read { get; private set; }
        public UpdateProvider Update { get; private set; }
        public DeleteProvider Delete { get; private set; }
        public Func<string, Task<string>> CreateDelegate { get; private set; }
        public Func<string, Task<string>> ReadDelegate { get; private set; }
        public Func<string, string, Task> UpdateDelegate { get; private set; }
        public Func<string, Task> DeleteDelegate { get; private set; }

        public IProviderForTypeOptions AddCreate(CreateProvider createProvider) {
            if (Create != null) throw new CannotConfigureTheSameProviderTwiceException();
            Create = createProvider;
            return this;
        }

        public IProviderForTypeOptions AddRead(ReadProvider readProvider) {
            if (Read != null) throw new CannotConfigureTheSameProviderTwiceException();
            Read = readProvider;
            return this;
        }

        public IProviderForTypeOptions AddUpdate(UpdateProvider updateProvider) {
            if (Update != null) throw new CannotConfigureTheSameProviderTwiceException();
            Update = updateProvider;
            return this;
        }

        public IProviderForTypeOptions AddDelete(DeleteProvider deleteProvider) {
            if (Delete != null) throw new CannotConfigureTheSameProviderTwiceException();
            Delete = deleteProvider;
            return this;
        }

        public IProviderForTypeOptions AddCRUD(CRUDProvider crudProvider) {
            if (Create != null || Read != null || Update != null || Delete != null) throw new CannotConfigureTheSameProviderTwiceException();
            Create = crudProvider;
            Read = crudProvider;
            Update = crudProvider;
            Delete = crudProvider;
            return this;
        }

        public bool CanCreate() {
            return Create != null;
        }

        public bool CanRead() {
            return Read != null;
        }

        public bool CanUpdate() {
            return Update != null;
        }

        public bool CanDelete() {
            return Delete != null;
        }

        public void RegisterDelegatesFor<T>(EntitySerializer entitySerializer) {
            RegisterCreationDelegationFor<T>(entitySerializer);
            RegisterReadDelegationFor<T>(entitySerializer);
            RegisterUpdateDelegationFor<T>(entitySerializer);
            RegisterDeleteDelegationFor<T>();
        }

        private void RegisterCreationDelegationFor<T>(EntitySerializer entitySerializer) {
            CreateDelegate = async entityPayload => {
                var entity = entitySerializer.Deserialize<T>(entityPayload);
                return await Create.Create(entity);
            };
        }

        private void RegisterReadDelegationFor<T>(EntitySerializer entitySerializer) {
            ReadDelegate = async entityId => {
                var entity = await Read.Read<T>(entityId);
                return entitySerializer.Serialize(entity);
            };
        }

        private void RegisterUpdateDelegationFor<T>(EntitySerializer entitySerializer) {
            UpdateDelegate = async (entityId, entityPayload) => {
                var entity = entitySerializer.Deserialize<T>(entityPayload);
                await Update.Update(entityId, entity);
            };
        }

        private void RegisterDeleteDelegationFor<T>() {
            DeleteDelegate = async entityId => {
                await Delete.Delete<T>(entityId);
            };
        }

    }

}
using System;
using System.Text.Json;
using Backlight.Providers;

namespace Backlight.Services {
    public class ProviderForTypeForTypeOptions : IProviderForTypeOptions {
        public CreateProvider Create { get; private set; }
        public ReadProvider Read { get; private set; }
        public UpdateProvider Update { get; private set; }
        public DeleteProvider Delete { get; private set; }
        public Action<string> CreateDelegate { get; private set; }
        public Func<string, string> ReadDelegate { get; private set; }
        public Action<string, string> UpdateDelegate { get; private set; }
        public Action<string> DeleteDelegate { get; private set; }

        public IProviderForTypeOptions AddCreate(CreateProvider createProvider) {
            Create = createProvider;
            return this;
        }

        public IProviderForTypeOptions AddRead(ReadProvider readProvider) {
            Read = readProvider;
            return this;
        }

        public IProviderForTypeOptions AddUpdate(UpdateProvider updateProvider) {
            Update = updateProvider;
            return this;
        }

        public IProviderForTypeOptions AddDelete(DeleteProvider deleteProvider) {
            Delete = deleteProvider;
            return this;
        }

        public IProviderForTypeOptions AddCRUD(CRUDProvider crudProvider) {
            Create = crudProvider;
            Read = crudProvider;
            Update = crudProvider;
            Delete = crudProvider;
            return this;
        }

        public void RegisterDelegatesFor<T>() {
            RegisterCreationDelegationFor<T>();
            RegisterReadDelegationFor<T>();
            RegisterUpdateDelegationFor<T>();
            RegisterDeleteDelegationFor<T>();
        }

        private void RegisterCreationDelegationFor<T>() {
            CreateDelegate = entityPayload => {
                var entity = JsonSerializer.Deserialize<T>(entityPayload);
                Create.Create(entity);
            };
        }

        private void RegisterReadDelegationFor<T>() {
            ReadDelegate = entityId => {
                var entity = Read.Read<T>(entityId);
                return JsonSerializer.Serialize(entity);
            };
        }

        private void RegisterUpdateDelegationFor<T>() {
            UpdateDelegate = (entityId, entityPayload) => {
                var entity = JsonSerializer.Deserialize<T>(entityPayload);
                Update.Update(entityId, entity);
            };
        }

        private void RegisterDeleteDelegationFor<T>() {
            DeleteDelegate = entityId => {
                Delete.Delete<T>(entityId);
            };
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
        
    }
}
using System;
using System.Text.Json;
using Backlight.Providers;

namespace Backlight.Services {
    public class BacklightProvidersConfiguration : ProvidersConfiguration {
        public CreateProvider CreateProvider { get; private set; }
        public ReadProvider ReadProvider { get; private set; }
        public UpdateProvider UpdateProvider { get; private set; }
        public DeleteProvider DeleteProvider { get; private set; }
        public Action<string> Create { get; private set; }
        public Func<string, string> Read { get; private set; }
        public Action<string, string> Update { get; private set; }
        public Action<string> Delete { get; private set; }

        public ProvidersConfiguration AddCreate(CreateProvider createProvider) {
            CreateProvider = createProvider;
            return this;
        }

        public ProvidersConfiguration AddRead(ReadProvider readProvider) {
            ReadProvider = readProvider;
            return this;
        }
        public ProvidersConfiguration AddUpdate(UpdateProvider updateProvider) {
            UpdateProvider = updateProvider;
            return this;
        }

        public ProvidersConfiguration AddDelete(DeleteProvider deleteProvider) {
            DeleteProvider = deleteProvider;
            return this;
        }

        public ProvidersConfiguration AddCRUD(CRUDProvider crudProvider) {
            CreateProvider = crudProvider;
            ReadProvider = crudProvider;
            UpdateProvider = crudProvider;
            DeleteProvider = crudProvider;
            return this;
        }

        public void RegisterDelegatesFor<T>() {
            RegisterCreationDelegationFor<T>();
            RegisterReadDelegationFor<T>();
            RegisterUpdateDelegationFor<T>();
            RegisterDeleteDelegationFor<T>();
        }

        private void RegisterCreationDelegationFor<T>() {
            Create = entityPayload => {
                var entity = JsonSerializer.Deserialize<T>(entityPayload);
                CreateProvider.Create(entity);
            };
        }

        private void RegisterReadDelegationFor<T>() {
            Read = entityId => {
                var entity = ReadProvider.Read<T>(entityId);
                return JsonSerializer.Serialize(entity);
            };
        }

        private void RegisterUpdateDelegationFor<T>() {
            Update = (entityId, entityPayload) => {
                var entity = JsonSerializer.Deserialize<T>(entityPayload);
                UpdateProvider.Update(entityId, entity);
            };
        }

        private void RegisterDeleteDelegationFor<T>() {
            Delete = entityId => {
                DeleteProvider.Delete<T>(entityId);
            };
        }

    }

}
using Backlight.Providers;

namespace Backlight.Services {
    public class ProvidersConfiguration {
        public CreateProvider CreateProvider { get; private set; }
        public ReadProvider ReadProvider { get; private set; }
        public UpdateProvider UpdateProvider { get; private set; }
        public DeleteProvider DeleteProvider { get; private set; }

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

        public bool CanCreate() {
            return CreateProvider != null;
        }

        public bool CanRead() {
            return ReadProvider != null;
        }

        public bool CanUpdate() {
            return UpdateProvider != null;
        }

        public bool CanDelete() {
            return DeleteProvider != null;
        }
    }

}
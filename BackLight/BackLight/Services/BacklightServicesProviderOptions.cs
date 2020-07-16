using Backlight.Providers;

namespace Backlight.Services {
    public class BacklightServicesProviderOptions {
        public CreateProvider CreateProvider { get; private set; }
        public ReadProvider ReadProvider { get; private set; }
        public UpdateProvider UpdateProvider { get; private set; }
        public DeleteProvider DeleteProvider { get; private set; }

        public BacklightServicesProviderOptions AddCreate(CreateProvider createProvider) {
            CreateProvider = createProvider;
            return this;
        }

        public BacklightServicesProviderOptions AddRead(ReadProvider readProvider) {
            ReadProvider = readProvider;
            return this;
        }
        public BacklightServicesProviderOptions AddUpdate(UpdateProvider updateProvider) {
            UpdateProvider = updateProvider;
            return this;
        }

        public BacklightServicesProviderOptions AddDelete(DeleteProvider deleteProvider) {
            DeleteProvider = deleteProvider;
            return this;
        }

        public BacklightServicesProviderOptions AddCRUD(CRUDProvider crudProvider) {
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
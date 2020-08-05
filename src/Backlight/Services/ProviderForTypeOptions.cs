using Backlight.Exceptions;
using Backlight.Providers;

namespace Backlight.Services {
    public class ProviderForTypeOptions : IProviderForTypeOptions {
        public CreateProvider Create { get; private set; }
        public ReadProvider Read { get; private set; }
        public UpdateProvider Update { get; private set; }
        public DeleteProvider Delete { get; private set; }

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

    }

}
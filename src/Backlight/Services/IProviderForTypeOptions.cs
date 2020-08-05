using Backlight.Providers;

namespace Backlight.Services {
    public interface IProviderForTypeOptions {
        IProviderForTypeOptions AddCreate(CreateProvider createProvider);
        IProviderForTypeOptions AddRead(ReadProvider readProvider);
        IProviderForTypeOptions AddUpdate(UpdateProvider updateProvider);
        IProviderForTypeOptions AddDelete(DeleteProvider deleteProvider);
        IProviderForTypeOptions AddCRUD(CRUDProvider crudProvider);
    }
}
using Backlight.Providers;

namespace Backlight.Services {
    public interface IProviderOptions {
        IProviderOptions AddCreate(CreateProvider createProvider);
        IProviderOptions AddRead(ReadProvider readProvider);
        IProviderOptions AddUpdate(UpdateProvider updateProvider);
        IProviderOptions AddDelete(DeleteProvider deleteProvider);
        IProviderOptions AddCRUD(CRUDProvider crudProvider);
    }
}
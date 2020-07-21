using Backlight.Providers;

namespace Backlight.Services {
    public interface ProvidersConfiguration {
        ProvidersConfiguration AddCreate(CreateProvider createProvider);
        ProvidersConfiguration AddRead(ReadProvider readProvider);
        ProvidersConfiguration AddUpdate(UpdateProvider updateProvider);
        ProvidersConfiguration AddDelete(DeleteProvider deleteProvider);
        ProvidersConfiguration AddCRUD(CRUDProvider crudProvider);
    }
}
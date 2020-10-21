using Backlight.Providers;

namespace Backlight.Services {
    public interface IProviderForTypeOptions {
        IProviderForTypeOptions AddCreate(CreateProvider createProvider);
        IProviderForTypeOptions AddRead<T>(T readProvider) where T : ReadProvider, ReadAllIdsProvider;
        IProviderForTypeOptions AddUpdate(UpdateProvider updateProvider);
        IProviderForTypeOptions AddDelete(DeleteProvider deleteProvider);
        IProviderForTypeOptions AddCRUD<T>(T crudProvider) where T : CRUDProvider, ReadAllIdsProvider;
    }
}
namespace Backlight.Services {
    public interface IServiceOptions {
        IProviderForTypeOptions For<T>() where T : class, BacklightEntity;
    }
}
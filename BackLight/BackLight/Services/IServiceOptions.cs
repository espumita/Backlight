namespace Backlight.Services {
    public interface IServiceOptions {
        IProviderOptions For<T>();
    }
}
namespace Backlight.Providers {
    public interface CreateProvider : Provider {
        void Create<T>(T entity);

    }

}
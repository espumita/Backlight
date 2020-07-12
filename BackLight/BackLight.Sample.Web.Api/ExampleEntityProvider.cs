namespace Backlight.Sample.Web.Api {
    public class ExampleEntityProvider : CreateProvider, ReadProvider, UpdateProvider {
        public void Create<T>(T entity) {
        }

        public T Read<T>(string entityId) where T : new() {
            return new T();
        }

        public void Update<T>(string entityId, T entity) {
        }
    }
}
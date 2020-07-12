namespace Backlight.Sample.Web.Api {
    public class ExampleEntity2Provider : CRUDProvider {

        public void Create<T>(T entity) {
        }

        public T Read<T>(string entityId) where T : new() {
            return new T();
        }

        public void Update<T>(string entityId, T entity) {
        }

        public void Delete<T>(string entityId) {
        }
    }
}
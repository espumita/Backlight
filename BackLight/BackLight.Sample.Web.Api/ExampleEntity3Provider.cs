namespace Backlight.Sample.Web.Api {
    public class ExampleEntity3Provider : ReadProvider {

        public T Read<T>(string entityId) where T : new() {
            return new T();
        }
    }
}
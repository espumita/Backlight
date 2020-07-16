using Backlight.Providers;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntity3Provider : ReadProvider {

        public T Read<T>(string entityId) where T : new() {
            return new T();
        }
    }
}
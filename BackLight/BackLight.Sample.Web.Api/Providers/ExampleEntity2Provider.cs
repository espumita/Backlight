using System;
using Backlight.Providers;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntity2Provider : CRUDProvider {

        public void Create<T>(T entity) {
            Console.WriteLine("Created Entity2");
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
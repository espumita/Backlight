using System;
using Backlight.Providers;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntityProvider : CreateProvider, ReadProvider, UpdateProvider {
        public void Create<T>(T entity) {
            Console.WriteLine("Created Entity");
        }

        public T Read<T>(string entityId) where T : new() {
            return new T();
        }

        public void Update<T>(string entityId, T entity) {
        }
    }
}
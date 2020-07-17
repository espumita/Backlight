using System;
using Backlight.Providers;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntity3Provider : ReadProvider {

        public T Read<T>(string entityId) {
            Console.WriteLine("Readed Entity3");
            return default;
        }
    }
}
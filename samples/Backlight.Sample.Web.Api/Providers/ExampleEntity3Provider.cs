using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Sample.Web.Api.Entities;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExampleEntity3Provider : ReadProvider, ReadAllIdsProvider {

        public async Task<object> Read<T>(string entityId, T returnType) {
            Console.WriteLine($"Readed {entityId}");
            return new ExampleEntity3();
        }

        public async Task<List<string>> ReadAllIds() {
            return new List<string> {
                "1",
                "3",
                "4",
                "77",
                "78"
            };
        }
    }
}
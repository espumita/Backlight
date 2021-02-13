using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Sample.Web.Api.Entities;

namespace Backlight.Sample.Web.Api.Providers {
    public class UsersProvider : CRUDProvider {

        public async Task<string> Create<T>(T entity) {
            var user = entity as User;
            Console.WriteLine($"Created {user.Name}");
            return Guid.NewGuid().ToString();
        }

        public async Task<object> Read<T>(string entityId, T returnType) {
            Console.WriteLine($"Readed {entityId}");
            return new User {
                Id = entityId
            };
        }

        public async Task Update<T>(string entityId, T entity) {
            var user = entity as User;
            Console.WriteLine($"Updated {user.Name} with id {entityId}");
        }

        public async Task Delete(string entityId) {
            Console.WriteLine($"Deleted {entityId}");
        }

        public async Task<List<string>> ReadAllIds() {
            return new List<string> {
                "00000000",
                "11111111",
                "22222222",
                "33333333",
                "44444444",
                "55555555",
                "66666666",
                "77777777",
                "88888888",
                "99999999",
                "AAAAAAAA",
                "BBBBBBBB",
                "CCCCCCCC",
                "DDDDDDDD",
                "EEEEEEEE",
                "FFFFFFFF"
            };
        }
    }
}
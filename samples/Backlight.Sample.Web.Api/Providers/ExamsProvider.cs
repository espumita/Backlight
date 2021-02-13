using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Sample.Web.Api.Entities;

namespace Backlight.Sample.Web.Api.Providers {
    public class ExamsProvider : CreateProvider, ReadProvider, UpdateProvider, ReadAllIdsProvider {
        public async Task<string> Create<T>(T entity) {
            var exam = entity as Exam;
            Console.WriteLine($"Created {exam.Grade}");
            return "4";
        }

        public async Task<object> Read<T>(string entityId, T returnType) {
            Console.WriteLine($"Readed {entityId}");
            return new Exam {
                Id = entityId
            };
        }

        public async Task Update<T>(string entityId, T entity) {
            var exam = entity as Exam;
            Console.WriteLine($"Updated {exam.Grade} with id {entityId}");
        }

        public async Task<List<string>> ReadAllIds() {
            return new List<string> {
                "1",
                "2",
                "3"
            };
        }
    }
}
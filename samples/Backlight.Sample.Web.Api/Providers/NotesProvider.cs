using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Sample.Web.Api.Entities;

namespace Backlight.Sample.Web.Api.Providers {
    public class NotesProvider : ReadProvider, ReadAllIdsProvider {

        public async Task<object> Read<T>(string entityId, T returnType) {
            Console.WriteLine($"Readed {entityId}");
            return new Note {
                Id = entityId
            };
        }

        public async Task<List<string>> ReadAllIds() {
            return new List<string> {
                "C7895DD6-5CBC-4D0E-A0CC-D3DE280DD0F5",
                "AB5109E7-0F42-43E9-8370-05199D19E3AC",
                "77206228-0086-4D19-93FE-8DCC890FC6F8",
                "EBE48698-60C6-4AA5-AB49-D3B710B599EF",
                "475D426E-95CD-45B4-8F99-C95CB166375F",
                "F1D76FC8-26F8-4BAA-8734-06AEA8B6B66D",
                "253305F8-9119-4047-953D-10B9EC87EA6E"  
            };
        }
    }
}
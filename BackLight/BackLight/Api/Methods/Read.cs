using System.Threading.Tasks;
using Backlight.Api.Serialization;
using Backlight.Services;
using Microsoft.AspNetCore.Http;

namespace Backlight.Api.Methods {
    public class Read : ApiMethod {
        private readonly BacklightService service;
        private readonly HttpContext httpContext;

        public Read(BacklightService service, HttpContext httpContext) {
            this.service = service;
            this.httpContext = httpContext;
        }

        public override async Task<ApiResult> Execute(EntityPayload entityPayload) {
            var read = service.ReaderProviderFor(entityPayload);
            var serializedEntity = read(entityPayload.Value);
            return await OkResponse(serializedEntity, httpContext);
        }
    }
}
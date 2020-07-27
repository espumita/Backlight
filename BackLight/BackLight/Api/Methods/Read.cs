using System.Threading.Tasks;
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

        public async Task<ApiResult> Execute(string entityTypeName, string entityId) {
            var serializedEntity = await service.Read(entityTypeName, entityId);
            return await OkResponse(serializedEntity, httpContext);
        }
    }
}
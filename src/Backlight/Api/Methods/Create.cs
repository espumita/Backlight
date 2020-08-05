using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Backlight.Api.Serialization;
using Backlight.Services;
using Microsoft.AspNetCore.Http;

namespace Backlight.Api.Methods {
    public class Create : ApiMethod {
        private readonly BacklightService service;
        private readonly HttpContext httpContext;

        public Create(BacklightService service, HttpContext httpContext) {
            this.service = service;
            this.httpContext = httpContext;
        }

        public async Task<ApiResult> Execute(string entityTypeName, string entityPayload) {
            var entityId = await service.Create(entityTypeName, entityPayload);
            return await OkResponse($"{SuccessMessages.EntityCreated} with id: {entityId}", httpContext);
        }
    }

}
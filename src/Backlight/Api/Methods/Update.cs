using System.Threading.Tasks;
using Backlight.Api.Serialization;
using Backlight.Services;
using Microsoft.AspNetCore.Http;

namespace Backlight.Api.Methods {
    public class Update : ApiMethod {
        private readonly BacklightService service;
        private readonly HttpContext httpContext;

        public Update(BacklightService service, HttpContext httpContext) {
            this.service = service;
            this.httpContext = httpContext;
        }

        public async Task<ApiResult> Execute(string entityTypeName, string entityId, string entityPayload) {
            await service.Update(entityTypeName, entityId, entityPayload);
            return await OkResponse(SuccessMessages.EntityUpdated, httpContext);
        }
    }
}
using System.Threading.Tasks;
using Backlight.Api.Serialization;
using Backlight.Services;
using Microsoft.AspNetCore.Http;

namespace Backlight.Api.Methods {
    public class Delete : ApiMethod {
        private readonly BacklightService service;
        private readonly HttpContext httpContext;

        public Delete(BacklightService service, HttpContext httpContext) {
            this.service = service;
            this.httpContext = httpContext;
        }

        public override async Task<ApiResult> Execute(EntityPayload entityPayload) {
            await service.Delete(entityPayload);
            return await OkResponse(SuccessMessages.EntityDeleted, httpContext);
        }
    }
}
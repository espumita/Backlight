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

        public override async Task<ApiResult> Execute(EntityPayload entityPayload) {
            await service.Create(entityPayload);
            return await OkResponse(SuccessMessages.EntityCreated, httpContext);
        }
    }

}
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

        public override async Task<ApiResult> Execute(EntityPayload entityPayload) {
            var update = service.UpdateProviderFor(entityPayload);
            update("TODOEntityId", entityPayload.Value);
            return await OkResponse(SuccessMessages.EntityUpdated, httpContext);
        }
    }
}
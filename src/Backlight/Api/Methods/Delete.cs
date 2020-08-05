using System.Threading.Tasks;
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

        public async Task<ApiResult> Execute(string entityTypeName, string entityId) {
            await service.Delete(entityTypeName, entityId);
            return await OkResponse(SuccessMessages.EntityDeleted, httpContext);
        }
    }
}
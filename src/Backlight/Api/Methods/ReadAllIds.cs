using System.Threading.Tasks;
using Backlight.Services;
using Microsoft.AspNetCore.Http;

namespace Backlight.Api.Methods {
    public class ReadAllIds : ApiMethod {
        private readonly BacklightService service;
        private readonly HttpContext httpContext;

        public ReadAllIds(BacklightService service, HttpContext httpContext) {
            this.service = service;
            this.httpContext = httpContext;
        }

        public async Task<ApiResult> Execute(string entityTypeName) {
            var allEntityIds = await service.ReadAllIds(entityTypeName);
            return await OkResponse(allEntityIds, httpContext);
        }
    }
}
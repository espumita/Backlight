using System.Net;
using System.Text;
using System.Threading.Tasks;
using Backlight.Api.Serialization;
using Microsoft.AspNetCore.Http;

namespace Backlight.Api.Methods {
    public abstract class ApiMethod {

        public abstract Task<ApiResult> Execute(EntityPayload entityPayload);
        protected async Task<ApiResult> OkResponse(string responseBody, HttpContext httpContext) {
            await ResponseWith(HttpStatusCode.OK, responseBody, httpContext);
            return ApiResult.OK;
        }

        private async Task ResponseWith(HttpStatusCode httpStatusCode, string responseBody, HttpContext httpContext) {
            httpContext.Response.StatusCode = (int)httpStatusCode;
            await httpContext.Response.WriteAsync(responseBody, Encoding.UTF8);
        }
    }
}
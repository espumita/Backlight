using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Backlight.Api.Methods {
    public class ApiMethod {

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
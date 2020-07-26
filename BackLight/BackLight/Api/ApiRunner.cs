using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Backlight.Api.Methods;
using Backlight.Api.Serialization;
using Backlight.Exceptions;
using Backlight.Services;
using Microsoft.AspNetCore.Http;

namespace Backlight.Api {
    public class ApiRunner {
        private readonly BacklightService service;
        private readonly StreamSerializer streamSerializer;

        public ApiRunner(BacklightService service, StreamSerializer streamSerializer) {
            this.service = service;
            this.streamSerializer = streamSerializer;
        }

        public async Task<ApiResult> Run(HttpContext httpContext) {
            if (IsNotAllowed(httpContext.Request.Method)) return await MethodNotAllowedResponse(httpContext);
            try {
                var entityPayload = await streamSerializer.EntityPayloadFrom(httpContext.Request.Body);
                return await ApiMethodFor(httpContext).Execute(entityPayload);
            } catch (EntityDeserializationException) {
                return await EntityDeserializationErrorResponse(httpContext);
            } catch (EntityProviderIsNotAvailableException) {
                return await EntityProviderIsNotAvailableResponse(httpContext);
            } catch (EntityIsNotConfiguredException) {
                return await EntityIsNotConfiguredResponse(httpContext);
            }
        }

        private static bool IsNotAllowed(string requestMethod) {
            var allowedMethods = new List<string> {
                HttpMethods.Put,
                HttpMethods.Get,
                HttpMethods.Post,
                HttpMethods.Delete
            };
            return !allowedMethods.Contains(requestMethod);
        }

        private async Task<ApiResult> MethodNotAllowedResponse(HttpContext httpContext) {
            await ResponseWith(HttpStatusCode.MethodNotAllowed, ErrorMessages.MethodNotAllowed, httpContext);
            return ApiResult.ERROR;
        }

        private ApiMethod ApiMethodFor(HttpContext httpContext) {
            return httpContext.Request.Method switch {
                var method when method.Equals(HttpMethods.Put) => new Create(service, httpContext),
                var method when method.Equals(HttpMethods.Get) => new Read(service, httpContext),
                var method when method.Equals(HttpMethods.Post) => new Update(service, httpContext),
                var method when method.Equals(HttpMethods.Delete) => new Delete(service, httpContext),
            };
        }

        private async Task<ApiResult> EntityDeserializationErrorResponse(HttpContext httpContext) {
            await ResponseWith(HttpStatusCode.BadRequest, ErrorMessages.EntityDeserializationError, httpContext);
            return ApiResult.ERROR;
        }

        private async Task<ApiResult> EntityIsNotConfiguredResponse(HttpContext httpContext) {
            await ResponseWith(HttpStatusCode.BadRequest, ErrorMessages.EntityIsNotConfigured, httpContext);
            return ApiResult.ERROR;
        }

        private async Task<ApiResult> EntityProviderIsNotAvailableResponse(HttpContext httpContext) {
            await ResponseWith(HttpStatusCode.BadRequest, ErrorMessages.EntityProviderIsNotAvailable, httpContext);
            return ApiResult.ERROR;
        }

        private async Task ResponseWith(HttpStatusCode httpStatusCode, string responseBody, HttpContext httpContext) {
            httpContext.Response.StatusCode = (int) httpStatusCode;
            await httpContext.Response.WriteAsync(responseBody, Encoding.UTF8);
        }

    }

}
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Backlight.Api.Serialization;
using Backlight.Exceptions;
using Backlight.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Backlight.Api {
    public class ApiRunner {
        private readonly IApplicationBuilder applicationBuilder;
        private readonly StreamSerializer streamSerializer;

        public ApiRunner(IApplicationBuilder applicationBuilder, StreamSerializer streamSerializer) {
            this.applicationBuilder = applicationBuilder;
            this.streamSerializer = streamSerializer;
        }

        public async Task<ApiResult> Run(HttpContext httpContext) {
            var httpMethod = httpContext.Request.Method;
            if (IsNotAllowed(httpMethod)) return await MethodNotAllowedResponse(httpContext);
            try {
                var entityPayload = await streamSerializer.EntityPayloadFrom(httpContext.Request.Body);
                var service = applicationBuilder.ApplicationServices.GetService<BacklightService>();
                if (httpMethod == HttpMethods.Put) return await Create(entityPayload, service, httpContext);
                if (httpMethod == HttpMethods.Get) return await Read(entityPayload, service, httpContext);
                if (httpMethod == HttpMethods.Post) return await Update(entityPayload, service, httpContext);
                if (httpMethod == HttpMethods.Delete) return await Delete(entityPayload, service, httpContext);
            } catch (EntityDeserializationException exception) {
                return await EntityDeserializationErrorResponse(httpContext);
            } catch (EntityProviderIsNotAvailableException exception) {
                return await EntityProviderIsNotAvailableResponse(httpContext);
            } catch (EntityIsNotConfiguredException exception) {
                return await EntityIsNotConfiguredResponse(httpContext);
            }
            return ApiResult.ERROR;
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

        private async Task<ApiResult> OkResponse(string responseBody, HttpContext httpContext) {
            await ResponseWith(HttpStatusCode.OK, responseBody, httpContext);
            return ApiResult.OK;
        }

        private async Task ResponseWith(HttpStatusCode httpStatusCode, string responseBody, HttpContext httpContext) {
            httpContext.Response.StatusCode = (int) httpStatusCode;
            await httpContext.Response.WriteAsync(responseBody, Encoding.UTF8);
        }

        private async Task<ApiResult> Create(EntityPayload entityPayload, BacklightService service, HttpContext httpContext) {
            var create = service.CreateProviderFor(entityPayload.TypeName);
            create(entityPayload.Value);
            return await OkResponse(SuccessMessages.EntityCreated, httpContext);
        }

        private async Task<ApiResult> Read(EntityPayload entityPayload, BacklightService service, HttpContext httpContext) {
            var read = service.ReaderProviderFor(entityPayload.TypeName);
            var serializedEntity = read(entityPayload.Value);
            return await OkResponse(serializedEntity, httpContext);
        }

        private async Task<ApiResult> Update(EntityPayload entityPayload, BacklightService service, HttpContext httpContext) {
            var update = service.UpdateProviderFor(entityPayload.TypeName);
            update("TODOEntityId", entityPayload.Value);
            return await OkResponse(SuccessMessages.EntityUpdated, httpContext);
        }

        private async Task<ApiResult> Delete(EntityPayload entityPayload, BacklightService service, HttpContext httpContext) {
            var delete = service.DeleteProviderFor(entityPayload.TypeName);
            delete(entityPayload.Value);
            return await OkResponse(SuccessMessages.EntityDeleted, httpContext);
        }

    }

}
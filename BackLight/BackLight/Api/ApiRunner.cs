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
                var entity = await streamSerializer.EntityFrom(httpContext.Request.Body);
                var service = applicationBuilder.ApplicationServices.GetService<BacklightService>();
                var entityIsConfigured = service.IsEntityConfiguredFor(entity);
                if (!entityIsConfigured) return await EntityIsNotConfiguredResponse(httpContext);
                if (httpMethod == HttpMethods.Put) {
                    if (!service.CanCreate(entity)) return await EntityProviderIsNotAvailableResponse(httpContext);
                    var entityPayload = await streamSerializer.EntityPayLoadFrom(httpContext.Request.Body);
                    var create = service.CreateProviderFor(entity);
                    create(entityPayload);
                    return await OkResponse(SuccessMessages.EntityCreated, httpContext);
                }
                if (httpMethod == HttpMethods.Get) {
                    if (!service.CanRead(entity)) return await EntityProviderIsNotAvailableResponse(httpContext);
                    var entityPayload = await streamSerializer.EntityPayLoadFrom(httpContext.Request.Body);
                    var read = service.ReaderProviderFor(entity);
                    var serializedEntity = read(entityPayload);
                    return await OkResponse(serializedEntity, httpContext);
                }
                if (httpMethod == HttpMethods.Post) {
                    if (!service.CanUpdate(entity)) return await EntityProviderIsNotAvailableResponse(httpContext);
                    var entityPayload = await streamSerializer.EntityPayLoadFrom(httpContext.Request.Body);
                    var update = service.UpdateProviderFor(entity);
                    update("TODOEntityId", entityPayload);
                    return await OkResponse(SuccessMessages.EntityUpdated, httpContext);
                }
                if (httpMethod == HttpMethods.Delete) {
                    if (!service.CanDelete(entity)) return await EntityProviderIsNotAvailableResponse(httpContext);
                    var entityPayload = await streamSerializer.EntityPayLoadFrom(httpContext.Request.Body);
                    var delete = service.DeleteProviderFor(entity);
                    delete(entityPayload);
                    return await OkResponse(SuccessMessages.EntityDeleted, httpContext);
                }
            } catch (EntityDeserializationException exception) {
                return await EntityDeserializationErrorResponse(httpContext);
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

    }

}
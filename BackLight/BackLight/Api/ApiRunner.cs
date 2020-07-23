using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

        public async Task Run(HttpContext httpContext) {
            var httpMethod = httpContext.Request.Method;

            if (IsNotAllowed(httpMethod)) {
                await ResponseWith(HttpStatusCode.MethodNotAllowed, ResponsesErrorMessages.MethodNotAllowed, httpContext);
                return;
            }
            var entity = string.Empty;
            
            try {
                entity = await streamSerializer.EntityFrom(httpContext.Request.Body);
            } catch (EntityDeserializationException exception) {
                await ResponseWith(HttpStatusCode.BadRequest, ResponsesErrorMessages.EntityDeserializationError, httpContext);
                return;
            }
            var service = applicationBuilder.ApplicationServices.GetService<BacklightService>();
            var entityIsConfigured = service.IsEntityConfiguredFor(entity);
            if (!entityIsConfigured) {
                await ResponseWith(HttpStatusCode.BadRequest, ResponsesErrorMessages.EntityIsNotConfigured, httpContext);
                return;
            }

            if (httpMethod == HttpMethods.Put) {
                if (!service.CanCreate(entity)) {
                    await ResponseWith(HttpStatusCode.BadRequest, ResponsesErrorMessages.EntityProviderIsNotAvailable, httpContext);
                    return;
                } else {
                    var entityPayload = await streamSerializer.EntityPayLoadFrom(httpContext.Request.Body);
                    var create = service.CreateProviderFor(entity);
                    create(entityPayload);
                    await ResponseWith(HttpStatusCode.OK, ResponsesSuccessMessages.EntityCreated, httpContext);
                    return;
                }
            }
            if (httpMethod == HttpMethods.Get) {
                if (!service.CanRead(entity)) {
                    await ResponseWith(HttpStatusCode.BadRequest, ResponsesErrorMessages.EntityProviderIsNotAvailable, httpContext);
                    return;
                } else {
                    var entityPayload = await streamSerializer.EntityPayLoadFrom(httpContext.Request.Body);
                    var read = service.ReaderProviderFor(entity);
                    var serializedEntity = read(entityPayload);
                    await ResponseWith(HttpStatusCode.OK, serializedEntity, httpContext);
                    return;
                }
            }
            if (httpMethod == HttpMethods.Post) {
                if (!service.CanUpdate(entity)) {
                    await ResponseWith(HttpStatusCode.BadRequest, ResponsesErrorMessages.EntityProviderIsNotAvailable, httpContext);
                    return;
                } else {
                    var entityPayload = await streamSerializer.EntityPayLoadFrom(httpContext.Request.Body);
                    var update = service.UpdateProviderFor(entity);
                    update("TODOEntityId", entityPayload);
                    await ResponseWith(HttpStatusCode.OK, ResponsesSuccessMessages.EntityUpdated, httpContext);
                    return;
                }
            }
            if (httpMethod == HttpMethods.Delete) {
                if (!service.CanDelete(entity)) {
                    await ResponseWith(HttpStatusCode.BadRequest, ResponsesErrorMessages.EntityProviderIsNotAvailable, httpContext);
                    return;
                } else {
                    var entityPayload = await streamSerializer.EntityPayLoadFrom(httpContext.Request.Body);
                    var delete = service.DeleteProviderFor(entity);
                    delete(entityPayload);
                    await ResponseWith(HttpStatusCode.OK, ResponsesSuccessMessages.EntityDelete, httpContext);
                    return;
                }
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

        private async Task ResponseWith(HttpStatusCode httpStatusCode, string responseBody, HttpContext httpContext) {
            httpContext.Response.StatusCode = (int) httpStatusCode;
            await httpContext.Response.WriteAsync(responseBody, Encoding.UTF8);
        }

    }

}
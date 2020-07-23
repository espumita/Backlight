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
            if (IsNotAllowed(httpContext.Request.Method)) {
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
            var backlightProvidersService = applicationBuilder.ApplicationServices.GetService<BacklightService>();
            var entityIsConfigured = backlightProvidersService.IsEntityConfiguredFor(entity);
            if (!entityIsConfigured) {
                await ResponseWith(HttpStatusCode.BadRequest, ResponsesErrorMessages.EntityIsNotConfigured, httpContext);
                return;
            }
            var entityProviderIsAvailable = backlightProvidersService.IsProviderAvailableFor(entity, httpContext.Request.Method);
            if (!entityProviderIsAvailable) {
                await ResponseWith(HttpStatusCode.BadRequest, ResponsesErrorMessages.EntityProviderIsNotAvailable, httpContext);
                return;
            }
            if (httpContext.Request.Method == HttpMethods.Put) {
                var createProviderExecution = backlightProvidersService.CreateProviderFor(entity, httpContext.Request.Method);
                var entityPayload = await streamSerializer.EntityPayLoadFrom(httpContext.Request.Body);
                createProviderExecution(entityPayload);
                await ResponseWith(HttpStatusCode.OK, ResponsesSuccessMessages.EntityCreated, httpContext);
                return;
            }
            if (httpContext.Request.Method == HttpMethods.Get) {
                var readProviderExecution = backlightProvidersService.ReaderProviderFor(entity, httpContext.Request.Method);
                var entityPayload = await streamSerializer.EntityPayLoadFrom(httpContext.Request.Body);
                var serializedEntity = readProviderExecution(entityPayload);
                await ResponseWith(HttpStatusCode.OK, serializedEntity, httpContext);
                return;
            }
            if (httpContext.Request.Method == HttpMethods.Post) {
                var updateProviderExecution = backlightProvidersService.UpdateProviderFor(entity, httpContext.Request.Method);
                var entityPayload = await streamSerializer.EntityPayLoadFrom(httpContext.Request.Body);
                updateProviderExecution("TODOEntityId", entityPayload);
                await ResponseWith(HttpStatusCode.OK, ResponsesSuccessMessages.EntityUpdated, httpContext);
                return;
            }
            if (httpContext.Request.Method == HttpMethods.Delete) {
                var deleteProviderExecution = backlightProvidersService.DeleteProviderFor(entity, httpContext.Request.Method);
                var entityPayload = await streamSerializer.EntityPayLoadFrom(httpContext.Request.Body);
                deleteProviderExecution(entityPayload);
                await ResponseWith(HttpStatusCode.OK, ResponsesSuccessMessages.EntityDelete, httpContext);
                return;
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
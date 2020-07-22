using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Exceptions;
using Backlight.Middleware;
using Backlight.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Backlight.Api {
    public class ApiRunner {
        private readonly IApplicationBuilder applicationBuilder;

        public ApiRunner(IApplicationBuilder applicationBuilder) {
            this.applicationBuilder = applicationBuilder;
        }

        public async Task Run(HttpContext httpContext) {
            if (IsNotAllowed(httpContext.Request.Method)) {
                await ResponseWith(HttpStatusCode.MethodNotAllowed, ResponsesErrorMessages.MethodNotAllowed, httpContext);
                return;
            }
            string entity = String.Empty;
            string body = String.Empty;
            
            try {
                body = await GetBodyFrom(httpContext.Request.Body);
                entity = EntityFrom(body);
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
                var entityPayload = EnitytPayLoadFrom(body);
                createProviderExecution(entityPayload);
                await ResponseWith(HttpStatusCode.OK, ResponsesSuccessMessages.EntityCreated, httpContext);
                return;
            }
            if (httpContext.Request.Method == HttpMethods.Get) {
                var readProviderExecution = backlightProvidersService.ReaderProviderFor(entity, httpContext.Request.Method);
                var entityPayload = EnitytPayLoadFrom(body);
                var serializedEntity = readProviderExecution(entityPayload);
                await ResponseWith(HttpStatusCode.OK, serializedEntity, httpContext);
                return;
            }
            if (httpContext.Request.Method == HttpMethods.Post) {
                var updateProviderExecution = backlightProvidersService.UpdateProviderFor(entity, httpContext.Request.Method);
                var entityPayload = EnitytPayLoadFrom(body);
                updateProviderExecution("aNewEntityId", entityPayload);
                await ResponseWith(HttpStatusCode.OK, ResponsesSuccessMessages.EntityUpdated, httpContext);
                return;
            }
            if (httpContext.Request.Method == HttpMethods.Delete) {
                var deleteProviderExecution = backlightProvidersService.DeleteProviderFor(entity, httpContext.Request.Method);
                var entityPayload = EnitytPayLoadFrom(body);
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
            //var responseStream = await ResponseBodyStreamWith(responseBody);
            // httpContext.Response.Body = responseStream;// TODO FIX TEST READ BODY
        }

        private async Task<Stream> ResponseBodyStreamWith(string responseBody) {
            var bodyStream = new MemoryStream();
            var streamWriter = new StreamWriter(bodyStream);
            await streamWriter.WriteAsync(responseBody);
            await streamWriter.FlushAsync();
            bodyStream.Seek(0, SeekOrigin.Begin);
            return bodyStream;
        }

        private static async Task<string> GetBodyFrom(Stream bodyStream) {
            var memoryStream = new MemoryStream();
            await bodyStream.CopyToAsync(memoryStream);
            var streamReader = new StreamReader(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var readToEndAsync = await streamReader.ReadToEndAsync();
            streamReader.Close();
            return readToEndAsync;
        }

        private static string EntityFrom(string body) {
            try {
                var backlightApiRequest = JsonSerializer.Deserialize<ApiRequest>(body);
                return backlightApiRequest.Entity;
            } catch (Exception exception) {
                //TODO log
                throw new EntityDeserializationException(exception);
            }
        }

        private static string EnitytPayLoadFrom(string body) {
            try {
                var backlightApiRequest = JsonSerializer.Deserialize<ApiRequest>(body);
                return backlightApiRequest.PayLoad;
            } catch (Exception exception) {
                //TODO log
                throw new EntityDeserializationException(exception);
            }
        }

    }

}
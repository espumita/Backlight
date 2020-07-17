using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Backlight.Middleware {
    public class ApiRunner {
        private readonly IApplicationBuilder applicationBuilder;
        private readonly HttpContext context;

        public ApiRunner(IApplicationBuilder applicationBuilder, HttpContext context) {
            this.applicationBuilder = applicationBuilder;
            this.context = context;
        }

        public async Task Run() {
            if (IsNotAllowed(context.Request.Method)) {
                await ResponseWith(HttpStatusCode.MethodNotAllowed, ResponsesErrorMessages.MethodNotAllowed);
                return;
            }
            string entity = String.Empty;
            string body = String.Empty;
            
            try {
                body = await GetBodyFrom(context.Request.Body);
                entity = EntityFrom(body);
            } catch (EntityDeserializationException exception) {
                await ResponseWith(HttpStatusCode.BadRequest, ResponsesErrorMessages.EntityDeserializationError);
                return;
            }
            var backlightProvidersService = applicationBuilder.ApplicationServices.GetService<BacklightProvidersService>();
            var entityIsConfigured = backlightProvidersService.IsEntityConfiguredFor(entity);
            if (!entityIsConfigured) {
                await ResponseWith(HttpStatusCode.BadRequest, ResponsesErrorMessages.EntityIsNotConfigured);
                return;
            }
            var entityProviderIsAvailable = backlightProvidersService.IsProviderAvailableFor(entity, context.Request.Method);
            if (!entityProviderIsAvailable) {
                await ResponseWith(HttpStatusCode.BadRequest, ResponsesErrorMessages.EntityProviderIsNotAvailable);
                return;
            }
            if (context.Request.Method == HttpMethods.Put) {
                var createProviderExecution = backlightProvidersService.CreateProviderFor(entity, context.Request.Method);
                var entityPayload = EnitytPayLoadFrom(body);
                createProviderExecution(entityPayload);
                await ResponseWith(HttpStatusCode.OK, ResponsesSuccessMessages.EntityCreated);
                return;
            }
            if (context.Request.Method == HttpMethods.Get) {
                var readProviderExecution = backlightProvidersService.ReaderProviderFor(entity, context.Request.Method);
                var entityPayload = EnitytPayLoadFrom(body);
                var serializedEntity = readProviderExecution(entityPayload);
                await ResponseWith(HttpStatusCode.OK, serializedEntity);
                return;
            }
            if (context.Request.Method == HttpMethods.Post) {
                var updateProviderExecution = backlightProvidersService.UpdateProviderFor(entity, context.Request.Method);
                var entityPayload = EnitytPayLoadFrom(body);
                updateProviderExecution("aNewEntityId", entityPayload);
                await ResponseWith(HttpStatusCode.OK, ResponsesSuccessMessages.EntityUpdated);
                return;
            }
            if (context.Request.Method == HttpMethods.Delete) {
                var deleteProviderExecution = backlightProvidersService.DeleteProviderFor(entity, context.Request.Method);
                var entityPayload = EnitytPayLoadFrom(body);
                deleteProviderExecution(entityPayload);
                await ResponseWith(HttpStatusCode.OK, ResponsesSuccessMessages.EntityDelete);
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

        private async Task ResponseWith(HttpStatusCode httpStatusCode, string responseBody) {
            context.Response.StatusCode = (int) httpStatusCode;
            var responseStream = await ResponseBodyStreamWith(responseBody);
            context.Response.Body = responseStream;
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
                var backlightApiRequest = JsonSerializer.Deserialize<BacklightApiRequest>(body);
                return backlightApiRequest.Entity;
            } catch (Exception exception) {
                //TODO log
                throw new EntityDeserializationException(exception);
            }
        }

        private static string EnitytPayLoadFrom(string body) {
            try {
                var backlightApiRequest = JsonSerializer.Deserialize<BacklightApiRequest>(body);
                return backlightApiRequest.PayLoad;
            } catch (Exception exception) {
                //TODO log
                throw new EntityDeserializationException(exception);
            }
        }

    }

}
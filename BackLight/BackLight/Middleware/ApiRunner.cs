using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
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
            if (context.Request.Method == HttpMethods.Put) {
                var backlightProvidersService = applicationBuilder.ApplicationServices.GetService<BacklightProvidersService>();
                var body = await GetBodyFrom(context.Request.Body);
                var entity = await EntityFrom(body);
                var entityIsConfigured = backlightProvidersService.IsEntityConfiguredFor(entity);
                if (!entityIsConfigured) {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync("Entity is not configured");
                } else {
                    var canCreate = backlightProvidersService.CanCreate(entity);
                    if (!canCreate) {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync("Entity Creation is not configured");
                    } else {
                        var createProvider = backlightProvidersService.CreateProvider(entity);
                        var type = backlightProvidersService.GetType(entity);
                        var entityPayload = await EntityPayloadFrom(type, body);
                        createProvider.Create(entityPayload);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        await context.Response.WriteAsync("Create");
                    }
                }
                return;
            }

            if (context.Request.Method == HttpMethods.Get) {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync("Read");
                return;
            }

            if (context.Request.Method.Equals(HttpMethods.Post)) {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync("Update");
                return;
            }

            if (context.Request.Method == HttpMethods.Delete) {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync("Delete");
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
            await context.Response.WriteAsync(ResponsesErrorMessages.MethodNotAllowed);
        }
        
        private static async Task<string> GetBodyFrom(Stream requestBody) {
            using (StreamReader stream = new StreamReader(requestBody)) {
                return await stream.ReadToEndAsync();
            }
        }

        private static async Task<T> EntityPayloadFrom<T>(T type, string body) {
            return JsonSerializer.Deserialize<T>(body);
        }
        private static async Task<string> EntityFrom(string body) {
            var backlightApiRequest = JsonSerializer.Deserialize<BacklightApiRequest>(body);
            return backlightApiRequest.Entity;
        }

    }

}
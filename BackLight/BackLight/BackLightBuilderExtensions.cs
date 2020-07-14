using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Backlight {
    public static class BacklightBuilderExtensions {
        public static IApplicationBuilder UseBacklight(this IApplicationBuilder applicationBuilder,  Action<BacklightConfiguration> setupAction = null) {
            var configuration = new BacklightConfiguration();
            if (setupAction != null) {
                setupAction(configuration);
            } else {
                configuration = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<BacklightConfiguration>>().Value;
            }

            applicationBuilder.UseMiddleware<BacklightMiddleware>(configuration);
            applicationBuilder.Map($"/{configuration.RoutePrefix}/api", MapApi);
            return applicationBuilder;
        }

        private static void MapApi(IApplicationBuilder applicationBuilder) {
            applicationBuilder.Run(async context => {
                if (context.Request.Method == HttpMethods.Put) {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    await context.Response.WriteAsync("Create");
                }
                if (context.Request.Method == HttpMethods.Get) {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    await context.Response.WriteAsync("Read");
                }
                if (context.Request.Method == HttpMethods.Post) {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    await context.Response.WriteAsync("Update");
                }
                if (context.Request.Method == HttpMethods.Delete) {
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    await context.Response.WriteAsync("Delete");
                }

                context.Response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
                await context.Response.WriteAsync("Bad Request");

            });
        }
    }
}
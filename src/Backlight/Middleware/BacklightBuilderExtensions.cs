using System;
using Backlight.Api;
using Backlight.OpenApi;
using Backlight.UI;
using Microsoft.AspNetCore.Builder;

namespace Backlight.Middleware {
    public static class BacklightBuilderExtensions {
        public static IApplicationBuilder UseBacklight(this IApplicationBuilder applicationBuilder,  Action<MiddlewareConfiguration> setupAction = null) {
            var configuration = new MiddlewareConfiguration();
            if (setupAction != null) {
                setupAction(configuration);
            }
            return applicationBuilder.UseMiddleware<BacklightMiddleware>(configuration, new IndexHtmlLoader())
                .Map($"/{configuration.RoutePrefix}/api", ConfigureApiEndpointRunner())
                .Map($"/{configuration.RoutePrefix}/openapi.json", ConfigureOpenApiEndpointRunner());
        }

        private static Action<IApplicationBuilder> ConfigureApiEndpointRunner() {
            return applicationBuilder => {
                applicationBuilder.Run(async httpContext => {
                    await GetApiRunnerFrom(applicationBuilder).Run(httpContext);
                });
            };
        }

        private static ApiRunner GetApiRunnerFrom(IApplicationBuilder applicationBuilder) {
            return (ApiRunner) applicationBuilder.ApplicationServices.GetService(typeof(ApiRunner));
        }

        private static Action<IApplicationBuilder> ConfigureOpenApiEndpointRunner() {
            return applicationBuilder => {
                applicationBuilder.Run(async httpContext => {
                    await GetOpenApiGeneratorFrom(applicationBuilder).GenerateAsync(httpContext);
                });
            };
        }

        private static OpenApiGenerator GetOpenApiGeneratorFrom(IApplicationBuilder applicationBuilder) {
            return (OpenApiGenerator) applicationBuilder.ApplicationServices.GetService(typeof(OpenApiGenerator));
        }

    }
}
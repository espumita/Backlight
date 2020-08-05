using System;
using Backlight.Api;
using Backlight.Middleware.Html;
using Microsoft.AspNetCore.Builder;

namespace Backlight.Middleware {
    public static class BacklightBuilderExtensions {
        public static IApplicationBuilder UseBacklight(this IApplicationBuilder applicationBuilder,  Action<MiddlewareConfiguration> setupAction = null) {
            var configuration = new MiddlewareConfiguration();
            if (setupAction != null) {
                setupAction(configuration);
            }
            return applicationBuilder.UseMiddleware<BacklightMiddleware>(configuration, new IndexHtmlLoader())
                .Map($"/{configuration.RoutePrefix}/api/entity", ConfigureApiEndpointRunner());
        }

        private static Action<IApplicationBuilder> ConfigureApiEndpointRunner() {
            return applicationBuilder => {
            var apiRunner = GetApiRunnerFrom(applicationBuilder);
            applicationBuilder.Run(async httpContext => {
                    await apiRunner.Run(httpContext);
                });
            };
        }

        private static ApiRunner GetApiRunnerFrom(IApplicationBuilder applicationBuilder) {
            return (ApiRunner) applicationBuilder.ApplicationServices.GetService(typeof(ApiRunner));
        }

    }
}
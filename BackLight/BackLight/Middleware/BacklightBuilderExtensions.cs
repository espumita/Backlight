using System;
using Backlight.Api;
using Backlight.Api.Serialization;
using Backlight.Middleware.Html;
using Backlight.Services;
using Microsoft.AspNetCore.Builder;

namespace Backlight.Middleware {
    public static class BacklightBuilderExtensions {
        public static IApplicationBuilder UseBacklight(this IApplicationBuilder applicationBuilder,  Action<MiddlewareConfiguration> setupAction = null) {
            var configuration = new MiddlewareConfiguration();
            if (setupAction != null) {
                setupAction(configuration);
            }
            return applicationBuilder.UseMiddleware<BacklightMiddleware>(configuration, new IndexHtmlLoader())
                .Map($"/{configuration.RoutePrefix}/api", ConfigureApiEndpointRunner());
        }

        private static Action<IApplicationBuilder> ConfigureApiEndpointRunner() {
            return applicationBuilder => {
            var service = GetBacklightServiceFrom(applicationBuilder);
            applicationBuilder.Run(async httpContext => {
                    await new ApiRunner(service, new JsonStreamSerializer()).Run(httpContext);
                });
            };
        }

        private static BacklightService GetBacklightServiceFrom(IApplicationBuilder applicationBuilder) {
            return (BacklightService) applicationBuilder.ApplicationServices.GetService(typeof(BacklightService));
        }

    }
}
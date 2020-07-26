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
            var service = GetBacklightServiceFrom(applicationBuilder);
            return applicationBuilder.UseMiddleware<BacklightMiddleware>(configuration, new IndexHtmlLoader())
                .Map($"/{configuration.RoutePrefix}/api", ConfigureApiEndpointRunner(service));
        }

        private static BacklightService GetBacklightServiceFrom(IApplicationBuilder applicationBuilder) {
            return (BacklightService) applicationBuilder.ApplicationServices.GetService(typeof(BacklightService));
        }

        private static Action<IApplicationBuilder> ConfigureApiEndpointRunner(BacklightService service) {
            return applicationBuilder => applicationBuilder.Run(async httpContext => {
                await new ApiRunner(applicationBuilder, service, new JsonStreamSerializer()).Run(httpContext);
            });
        }

    }
}
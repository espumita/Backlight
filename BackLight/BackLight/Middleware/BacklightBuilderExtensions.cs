using System;
using Microsoft.AspNetCore.Builder;

namespace Backlight.Middleware {
    public static class BacklightBuilderExtensions {
        public static IApplicationBuilder UseBacklight(this IApplicationBuilder applicationBuilder,  Action<BacklightConfiguration> setupAction = null) {
            var configuration = new BacklightConfiguration();
            if (setupAction != null) {
                setupAction(configuration);
            } 
            return applicationBuilder.UseMiddleware<BacklightMiddleware>(configuration)
                .Map($"/{configuration.RoutePrefix}/api", ConfigureApiEndpointMapping());
        }

        private static Action<IApplicationBuilder> ConfigureApiEndpointMapping() {
            return applicationBuilder => applicationBuilder.Run(async httpContext => {
                await new ApiRunner(applicationBuilder).Run(httpContext);
            });
        }

    }
}
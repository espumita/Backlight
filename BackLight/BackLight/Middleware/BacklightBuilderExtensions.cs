using System;
using Microsoft.AspNetCore.Builder;

namespace Backlight.Middleware {
    public static class BacklightBuilderExtensions {
        public static IApplicationBuilder UseBacklight(this IApplicationBuilder applicationBuilder,  Action<BacklightConfiguration> setupAction = null) {
            var configuration = new BacklightConfiguration();
            if (setupAction != null) {
                setupAction(configuration);
            } 
            applicationBuilder.UseMiddleware<BacklightMiddleware>(configuration);
            applicationBuilder.Map($"/{configuration.RoutePrefix}/api", MapApi());
            return applicationBuilder;
        }

        private static Action<IApplicationBuilder> MapApi() {
            return applicationBuilder => applicationBuilder.Run(async context => {
                await new ApiRunner(applicationBuilder, context).Run();
            });
        }

    }
}
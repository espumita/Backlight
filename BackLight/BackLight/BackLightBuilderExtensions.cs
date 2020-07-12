using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BackLight {
    public static class BacklightBuilderExtensions {
        public static IApplicationBuilder UseBacklight(this IApplicationBuilder applicationBuilder,  Action<BacklightConfiguration> setupAction = null) {
            var configuration = new BacklightConfiguration();
            if (setupAction != null) {
                setupAction(configuration);
            } else {
                configuration = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<BacklightConfiguration>>().Value;
            }

            applicationBuilder.UseMiddleware<BacklightMiddleware>(configuration);

            return applicationBuilder;
        }
    }
}
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BackLight {
    public static class BackLightBuilderExtensions {
        public static IApplicationBuilder UseBackLight(this IApplicationBuilder applicationBuilder,  Action<BackLightConfiguration> setupAction = null) {
            var configuration = new BackLightConfiguration();
            if (setupAction != null) {
                setupAction(configuration);
            } else {
                configuration = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<BackLightConfiguration>>().Value;
            }

            applicationBuilder.UseMiddleware<BackLightMiddleware>(configuration);

            return applicationBuilder;
        }
    }
}
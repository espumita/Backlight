using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BackLight {
    public static class BackLightBuilderExtensions {
        public static IApplicationBuilder UseBackLight(this IApplicationBuilder applicationBuilder,  Action<BackLightOptions> setupAction = null) {
            var options = new BackLightOptions();
            if (setupAction != null) {
                setupAction(options);
            } else {
                options = applicationBuilder.ApplicationServices.GetRequiredService<IOptions<BackLightOptions>>().Value;
            }

            applicationBuilder.UseMiddleware<BackLightMiddleware>(options);

            return applicationBuilder;
        }
    }
}
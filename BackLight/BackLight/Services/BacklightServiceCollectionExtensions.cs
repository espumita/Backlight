using System;
using Microsoft.Extensions.DependencyInjection;

namespace Backlight.Services {
    public static class BacklightServiceCollectionExtensions {

        public static IServiceCollection AddBacklight(this IServiceCollection services, Action<BacklightServicesOptions> setupAction = null) {
            var options = new BacklightServicesOptions();
            if (setupAction != null) {
                setupAction(options);
                services.ConfigureBacklightServices(options);
            }
            return services;
        }

        public static void ConfigureBacklightServices(this IServiceCollection services, BacklightServicesOptions options) {
            var backlightProvidersService = new BacklightProvidersService(options);
            services.AddSingleton(backlightProvidersService);
            // services.Configure(setupAction);
        }
    }

}
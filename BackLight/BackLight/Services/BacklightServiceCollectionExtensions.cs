using System;
using Microsoft.Extensions.DependencyInjection;

namespace Backlight.Services {
    public static class BacklightServiceCollectionExtensions {

        public static IServiceCollection AddBacklight(this IServiceCollection services, Action<BacklightServicesConfiguration> setupServiceConfigurationAction = null) {
            var configuration = new BacklightServicesConfiguration();
            if (setupServiceConfigurationAction != null) {
                setupServiceConfigurationAction(configuration);
            }
            services.ConfigureBacklightServicesWith(configuration);
            return services;
        }

        private static void ConfigureBacklightServicesWith(this IServiceCollection services, BacklightServicesConfiguration configuration) {
            var backlightProvidersService = new BacklightProvidersService(configuration);
            services.AddSingleton(backlightProvidersService);
        }
    }

}
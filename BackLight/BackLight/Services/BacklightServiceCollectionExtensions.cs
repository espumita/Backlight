using System;
using Microsoft.Extensions.DependencyInjection;

namespace Backlight.Services {
    public static class BacklightServiceCollectionExtensions {

        public static IServiceCollection AddBacklight(this IServiceCollection services, Action<ServiceConfiguration> setupServiceConfigurationAction = null) {
            var configuration = new ServiceConfiguration();
            if (setupServiceConfigurationAction != null) {
                setupServiceConfigurationAction(configuration);
            }
            services.ConfigureBacklightServicesWith(configuration);
            return services;
        }

        private static void ConfigureBacklightServicesWith(this IServiceCollection services, ServiceConfiguration configuration) {
            var backlightProvidersService = new BacklightProvidersService(configuration);
            services.AddSingleton(backlightProvidersService);
        }
    }

}
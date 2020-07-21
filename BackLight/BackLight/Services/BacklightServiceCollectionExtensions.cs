using System;
using Microsoft.Extensions.DependencyInjection;

namespace Backlight.Services {
    public static class BacklightServiceCollectionExtensions {

        public static IServiceCollection AddBacklight(this IServiceCollection services, Action<ServiceOptions> setupServiceConfigurationAction = null) {
            var configuration = new BacklightServiceOptions();
            if (setupServiceConfigurationAction != null) {
                setupServiceConfigurationAction(configuration);
            }
            services.ConfigureBacklightServicesWith(configuration);
            return services;
        }

        private static void ConfigureBacklightServicesWith(this IServiceCollection services, BacklightServiceOptions options) {
            var backlightProvidersService = new BacklightProvidersService(options);
            services.AddSingleton(backlightProvidersService);
        }
    }

}
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Backlight.Services {
    public static class BacklightServiceCollectionExtensions {

        public static IServiceCollection AddBacklight(this IServiceCollection services, Action<IServiceOptions> setupServiceConfigurationAction = null) {
            var configuration = new ServiceOptions();
            if (setupServiceConfigurationAction != null) {
                setupServiceConfigurationAction(configuration);
            }
            services.ConfigureBacklightServicesWith(configuration);
            return services;
        }

        private static void ConfigureBacklightServicesWith(this IServiceCollection services, ServiceOptions options) {
            var backlightProvidersService = new BacklightProvidersService(options);
            services.AddSingleton(backlightProvidersService);
        }
    }

}
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Backlight.Services {
    public static class BacklightServiceCollectionExtensions {

        public static IServiceCollection AddBacklight(this IServiceCollection services, Action<IServiceOptions> setupAction = null) {
            var options = new ServiceOptions();
            if (setupAction != null) {
                setupAction(options);
            }
            var backlightProvidersService = new BacklightService(options);
            return services.AddSingleton(backlightProvidersService);
        }

    }

}
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Backlight {
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

           // services.Configure(setupAction);
        }
    }


}
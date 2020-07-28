using System;
using Backlight.Services.EntitySerialization;
using Microsoft.Extensions.DependencyInjection;

namespace Backlight.Services {
    public static class BacklightServiceCollectionExtensions {

        public static IServiceCollection AddBacklight(this IServiceCollection services, Action<IServiceOptions> setupAction = null) {
            var entitySerializer = new JsonEntitySerializer();
            var options = new ServiceOptions(entitySerializer);
            if (setupAction != null) {
                setupAction(options);
            }
            var backlightService = new BacklightService(options);
            return services.AddSingleton(backlightService);
        }

    }

}
using System;
using Backlight.Api;
using Backlight.Api.Serialization;
using Backlight.Services.EntitySerialization;
using Microsoft.Extensions.DependencyInjection;

namespace Backlight.Services {
    public static class BacklightServiceCollectionExtensions {

        public static IServiceCollection AddBacklight(this IServiceCollection services, Action<IServiceOptions> setupAction = null) {
            var options = new ServiceOptions();
            if (setupAction != null) {
                setupAction(options);
            }
            var entitySerializer = new JsonEntitySerializer();
            return services.AddSingleton(new BacklightService(options, entitySerializer))
                .AddSingleton<StreamSerializer>(new MemoryStreamSerializer())
                .AddSingleton<ApiRunner>();
        }

    }

}
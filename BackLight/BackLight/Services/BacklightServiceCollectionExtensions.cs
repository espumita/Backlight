using System;
using Backlight.Api;
using Backlight.Api.Serialization;
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
            return services.AddSingleton(new BacklightService(options))
                .AddSingleton<StreamSerializer>(new JsonStreamSerializer())
                .AddSingleton<ApiRunner>();
        }

    }

}
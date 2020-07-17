using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Backlight.Services {
    public class BacklightServicesOptions {
        public Dictionary<Type, BacklightServicesProviderOptions> Providers { get; set; } = new Dictionary<Type, BacklightServicesProviderOptions>();
        public Dictionary<Type, Action<string>> CreateProvidersDelegates { get; set; } = new Dictionary<Type, Action<string>>();
        public Dictionary<Type, Func<string, string>> ReadProvidersDelegates { get; set; } = new Dictionary<Type, Func<string, string>>();
        public Dictionary<Type, Action<string, string>> UpdateProvidersDelegates { get; set; } = new Dictionary<Type, Action<string, string>>();


        public BacklightServicesProviderOptions For<T>() {
            var backlightServicesProviderOptions = new BacklightServicesProviderOptions();
            Providers[typeof(T)] = backlightServicesProviderOptions;
            CreateProvidersDelegates[typeof(T)] = (entityPayload) => {
                var entity = JsonSerializer.Deserialize<T>(entityPayload);
                backlightServicesProviderOptions.CreateProvider.Create(entity);
            };
            return backlightServicesProviderOptions;
        }


    }

}
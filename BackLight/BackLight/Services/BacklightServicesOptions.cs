using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Backlight.Services {
    public class BacklightServicesOptions {
        public Dictionary<Type, BacklightServicesProviderOptions> Providers { get; set; } = new Dictionary<Type, BacklightServicesProviderOptions>();
        public Dictionary<Type, Action<string>> CreateProvidersDelegates { get; set; } = new Dictionary<Type, Action<string>>();
        public Dictionary<Type, Func<string, string>> ReadProvidersDelegates { get; set; } = new Dictionary<Type, Func<string, string>>();
        public Dictionary<Type, Action<string, string>> UpdateProvidersDelegates { get; set; } = new Dictionary<Type, Action<string, string>>();
        public Dictionary<Type, Action<string>> DeleteProvidersDelegates { get; set; } = new Dictionary<Type, Action<string>>();


        public BacklightServicesProviderOptions For<T>() {
            var backlightServicesProviderOptions = new BacklightServicesProviderOptions();
            Providers[typeof(T)] = backlightServicesProviderOptions;
            CreateProvidersDelegates[typeof(T)] = (entityPayload) => {
                var entity = JsonSerializer.Deserialize<T>(entityPayload);
                backlightServicesProviderOptions.CreateProvider.Create(entity);
            };
            ReadProvidersDelegates[typeof(T)] = (entityId) => {
                var entity = backlightServicesProviderOptions.ReadProvider.Read<T>(entityId);
                return JsonSerializer.Serialize(entity);
            };
            UpdateProvidersDelegates[typeof(T)] = (entityId, entityPayload) => {
                var entity = JsonSerializer.Deserialize<T>(entityPayload);
                backlightServicesProviderOptions.UpdateProvider.Update(entityId, entity);
            };
            DeleteProvidersDelegates[typeof(T)] = (entityId) => {
                backlightServicesProviderOptions.DeleteProvider.Delete<T>(entityId);
            };
            return backlightServicesProviderOptions;
        }


    }

}
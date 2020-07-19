using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Backlight.Services {
    public class ServiceConfiguration {
        public Dictionary<Type, ProvidersConfiguration> Providers { get; } = new Dictionary<Type, ProvidersConfiguration>();
        public Dictionary<Type, Action<string>> CreateProvidersDelegates { get; } = new Dictionary<Type, Action<string>>();
        public Dictionary<Type, Func<string, string>> ReadProvidersDelegates { get; } = new Dictionary<Type, Func<string, string>>();
        public Dictionary<Type, Action<string, string>> UpdateProvidersDelegates { get; } = new Dictionary<Type, Action<string, string>>();
        public Dictionary<Type, Action<string>> DeleteProvidersDelegates { get; } = new Dictionary<Type, Action<string>>();


        public ProvidersConfiguration For<T>() {
            var backlightServicesProviderOptions = new ProvidersConfiguration();
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
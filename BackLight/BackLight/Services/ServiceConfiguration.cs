﻿using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Backlight.Services {
    public class ServiceConfiguration {
        public Dictionary<Type, ProvidersConfiguration> ProvidersConfiguration { get; } = new Dictionary<Type, ProvidersConfiguration>();
        public Dictionary<Type, Action<string>> Create { get; } = new Dictionary<Type, Action<string>>();
        public Dictionary<Type, Func<string, string>> Read { get; } = new Dictionary<Type, Func<string, string>>();
        public Dictionary<Type, Action<string, string>> Update { get; } = new Dictionary<Type, Action<string, string>>();
        public Dictionary<Type, Action<string>> Delete { get; } = new Dictionary<Type, Action<string>>();


        public ProvidersConfiguration For<T>() {
            var backlightServicesProviderOptions = new ProvidersConfiguration();
            ProvidersConfiguration[typeof(T)] = backlightServicesProviderOptions;
            Create[typeof(T)] = (entityPayload) => {
                var entity = JsonSerializer.Deserialize<T>(entityPayload);
                backlightServicesProviderOptions.CreateProvider.Create(entity);
            };
            Read[typeof(T)] = (entityId) => {
                var entity = backlightServicesProviderOptions.ReadProvider.Read<T>(entityId);
                return JsonSerializer.Serialize(entity);
            };
            Update[typeof(T)] = (entityId, entityPayload) => {
                var entity = JsonSerializer.Deserialize<T>(entityPayload);
                backlightServicesProviderOptions.UpdateProvider.Update(entityId, entity);
            };
            Delete[typeof(T)] = (entityId) => {
                backlightServicesProviderOptions.DeleteProvider.Delete<T>(entityId);
            };
            return backlightServicesProviderOptions;
        }


    }

}
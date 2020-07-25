using System;
using System.Linq;
using System.Reflection;
using Backlight.Exceptions;

namespace Backlight.Services {
    public class BacklightService {
        public ServiceOptions Options { get; set; }

        public BacklightService(ServiceOptions options) {
            Options = options;
        }

        public virtual Action<string> CreateProviderFor(Type type) {
            CheckIfEntityIsConfiguredFor(type);
            if (!Options.ProvidersForType[type].CanCreate()) throw new EntityProviderIsNotAvailableException();
            var providersConfiguration = Options.ProvidersForType[type];
            return providersConfiguration.CreateDelegate;
        }

        public virtual Func<string, string> ReaderProviderFor(Type type) {
            CheckIfEntityIsConfiguredFor(type);
            if (!Options.ProvidersForType[type].CanRead()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.ReadDelegate;
        }

        public virtual Action<string, string> UpdateProviderFor(Type type) {
            CheckIfEntityIsConfiguredFor(type);
            if (!Options.ProvidersForType[type].CanUpdate()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.UpdateDelegate;
        }

        public virtual Action<string> DeleteProviderFor(Type type) {
            CheckIfEntityIsConfiguredFor(type);
            if (!Options.ProvidersForType[type].CanDelete()) throw new EntityProviderIsNotAvailableException();
            var backlightServicesProviderOptions = Options.ProvidersForType[type];
            return backlightServicesProviderOptions.DeleteDelegate;
        }

        public virtual Assembly GetsAssemblyFor(string entityFullName) {
            return Options.AssemblyForType[entityFullName];
        }

        private void CheckIfEntityIsConfiguredFor(Type type) {
            var isEntityConfigured = Options.ProvidersForType.Keys.Contains(type);
            if (!isEntityConfigured) throw new EntityIsNotConfiguredException();
        }
    }
}
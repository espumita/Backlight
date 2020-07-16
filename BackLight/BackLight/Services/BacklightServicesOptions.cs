using System;
using System.Collections.Generic;

namespace Backlight.Services {
    public class BacklightServicesOptions {
        public Dictionary<Type, BacklightServicesProviderOptions> Providers { get; set; } = new Dictionary<Type, BacklightServicesProviderOptions>();

        public BacklightServicesProviderOptions For<T>() {
            var backlightServicesProviderOptions = new BacklightServicesProviderOptions();
            Providers[typeof(T)] = backlightServicesProviderOptions;
            return backlightServicesProviderOptions;
        }
   
    }

}
using System.Text;

namespace Backlight.Middleware.Html {
    public static class StringBuilderConfigurationsExtensions {
        public static StringBuilder InjectConfiguration(this StringBuilder stringBuilder, string configurationName, string configurationValue) {
            return stringBuilder.Replace($"%({configurationName})", configurationValue);
        }

    }
}
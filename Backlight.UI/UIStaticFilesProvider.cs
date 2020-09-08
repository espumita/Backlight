using System.Reflection;

namespace Backlight.UI {
    public class UIStaticFilesProvider {
        private const string FilesNamespace = "Backlight.UI.node_modules.swagger_ui_dist";

        public Assembly Assembly() {
            return GetType().GetTypeInfo().Assembly;
        }

        public string EmbeddedFilesNamespace() {
            return FilesNamespace;
        }
    }
}
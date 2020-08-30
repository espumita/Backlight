using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backlight.UI {
    public class IndexHtmlLoader {
        private const string BacklightIndexHtml = "Backlight.UI.index.html";

        public virtual async Task<string> LoadRawWith(string documentTitle) {
            using (var fileStream = IndexHtmlFileStream()) {
                var rawIndexHtlm = await RawIndexHtmlFrom(fileStream);
                return StringBuilderConfigurationsExtensions.InjectConfiguration(new StringBuilder(rawIndexHtlm), "DocumentTitle", documentTitle)
                    .ToString();
            }
        }

        private Stream IndexHtmlFileStream() {
            return GetType().GetTypeInfo().Assembly
                .GetManifestResourceStream(BacklightIndexHtml);
        }

        private static async Task<string> RawIndexHtmlFrom(Stream fileStream) {
            return await new StreamReader(fileStream).ReadToEndAsync();
        }

    }

}
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backlight.UI {
    public class IndexHtmlLoader {
        private const string BacklightIndexHtml = "Backlight.UI.index.html";

        public virtual async Task<string> LoadRawWith(string documentTitle, string rootUrl, string urlPath) {
            using (var fileStream = IndexHtmlFileStream()) {
                var rawIndexHtlm = await RawIndexHtmlFrom(fileStream);
                return new StringBuilder(rawIndexHtlm)
                    .InjectConfiguration("DocumentTitle", documentTitle)
                    .InjectConfiguration("RootUrl", rootUrl)
                    .InjectConfiguration("UrlPath", urlPath)
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
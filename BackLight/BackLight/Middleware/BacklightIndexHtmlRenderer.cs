using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backlight.Middleware {
    public class BacklightIndexHtmlRenderer {
        private const string BacklightIndexHtml = "Backlight.index.html";

        public virtual async Task<string> RenderWith(string documentTitle) {
            using (var stream = IndexHtmlFileStream()) {
                var documentContent = await new StreamReader(stream).ReadToEndAsync();
                var responseStringBuilder = new StringBuilder(documentContent);
                var indexHtmlConfiguration = IndexHtmlConfigurationFrom(documentTitle);
                InjectIndexHtmlConfigurationInto(responseStringBuilder, indexHtmlConfiguration);
                return responseStringBuilder.ToString();
            }
        }

        private Stream IndexHtmlFileStream() {
            return GetType().GetTypeInfo().Assembly
                .GetManifestResourceStream(BacklightIndexHtml);
        }

        private static IDictionary<string, string> IndexHtmlConfigurationFrom(string documentTitle) {
            return new Dictionary<string, string> {
                { "%(DocumentTitle)", documentTitle },
            };
        }

        private static void InjectIndexHtmlConfigurationInto(StringBuilder responseStringBuilder, IDictionary<string, string> indexHtmlConfiguration) {
            indexHtmlConfiguration.Keys.ToList().ForEach(key =>
                responseStringBuilder.Replace(key, indexHtmlConfiguration[key])
            );
        }

    }
}
using System.Threading.Tasks;
using Backlight.UI;
using FluentAssertions;
using NUnit.Framework;

namespace Backlight.Test.Middleware.Html {
    public class IndexHtmlLoaderTests {
        private const string ADocumentTitle = "aDocumentTitle";
        private const string AUrlPath = "aUrlPath";
        private const string ARootUrl = "https://localhost:44349";

        [Test]
        public async Task get_raw_index_html_with_document_title() {
            var indexHtmlLoader = new IndexHtmlLoader();
            
            var rawIndexHtml = await indexHtmlLoader.LoadRawWith(ADocumentTitle, ARootUrl, AUrlPath);

            rawIndexHtml.Contains($"<title>{ADocumentTitle}</title>").Should().BeTrue();
        }
    }
}
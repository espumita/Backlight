using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BackLight {
    public class BackLightMiddleware {
        private readonly RequestDelegate next;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ILoggerFactory loggerFactory;
        private readonly BackLightConfiguration backLightConfiguration;

        public BackLightMiddleware(RequestDelegate next, IWebHostEnvironment hostingEnvironment, ILoggerFactory loggerFactory, BackLightConfiguration backLightConfiguration) {
            this.next = next;
            this.hostingEnvironment = hostingEnvironment;
            this.loggerFactory = loggerFactory;
            this.backLightConfiguration = backLightConfiguration;
        }

        public async Task Invoke(HttpContext httpContext) {
            var httpMethod = httpContext.Request.Method;
            var path = httpContext.Request.Path.Value;

            if (IsGet(httpMethod) && RoutePrefixIsRequestedWithOrWithoutSlash(backLightConfiguration.RoutePrefix, path)) {
                RedirectToIndexHtml(httpContext, path);
                return;
            }
            if (IsGet(httpMethod) && IsIndexHtmlRoute(backLightConfiguration.RoutePrefix, path)) {
                await RespondWithIndexHtml(httpContext.Response);
                return;
            }
            await next.Invoke(httpContext);
        }

        private static bool IsGet(string httpMethod) {
            return httpMethod == HttpMethods.Get;
        }

        private static bool RoutePrefixIsRequestedWithOrWithoutSlash(string routePrefix, string path) {
            return Regex.IsMatch(path, $"^/?{Regex.Escape(routePrefix)}/?$");
        }

        public void RedirectToIndexHtml(HttpContext httpContext, string path) {
            var relativeRedirectPath = path.EndsWith("/")
                ? "index.html"
                : $"{path.Split('/').Last()}/index.html";
            RespondWithRedirect(httpContext.Response, relativeRedirectPath);
        }

        private static void RespondWithRedirect(HttpResponse response, string location) {
            response.StatusCode = 301;
            response.Headers["Location"] = location;
        }

        private static bool IsIndexHtmlRoute(string routePrefix, string path) {
            return Regex.IsMatch(path, $"^/{Regex.Escape(routePrefix)}/?index.html$");
        }

        private async Task RespondWithIndexHtml(HttpResponse response) {
            response.StatusCode = 200;
            response.ContentType = "text/html;charset=utf-8";
            using (var stream = IndexHtmlFileStream()) {
                var documentContent = await new StreamReader(stream).ReadToEndAsync();
                var responseStringBuilder = new StringBuilder(documentContent);
                var indexHtmlConfiguration = IndexHtmlConfigurationFrom(backLightConfiguration);
                InjectIndexHtmlConfigurationInto(responseStringBuilder, indexHtmlConfiguration);
                await response.WriteAsync(responseStringBuilder.ToString(), Encoding.UTF8);
            }
        }

        private Stream IndexHtmlFileStream() {
            return GetType().GetTypeInfo().Assembly
                .GetManifestResourceStream("BackLight.index.html");
        }

        private static IDictionary<string, string> IndexHtmlConfigurationFrom(BackLightConfiguration backLightConfiguration) {
            return new Dictionary<string, string> {
                { "%(DocumentTitle)", backLightConfiguration.IndexHtmlDocumentTitle },
            };
        }

        private static void InjectIndexHtmlConfigurationInto(StringBuilder responseStringBuilder, IDictionary<string, string> indexHtmlConfiguration) {
            indexHtmlConfiguration.Keys.ToList().ForEach(key =>
                responseStringBuilder.Replace(key, indexHtmlConfiguration[key])
            );
        }

    }
}
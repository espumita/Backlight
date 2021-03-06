using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Backlight.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Backlight.Middleware {
    public class BacklightMiddleware {
        private readonly MiddlewareConfiguration configuration;
        private readonly IndexHtmlLoader idexHtmlLoader;
        private readonly StaticFileMiddleware staticFilesMiddleware;
        
        public BacklightMiddleware(RequestDelegate next, MiddlewareConfiguration configuration, IndexHtmlLoader idexHtmlLoader, IWebHostEnvironment webHostEnvironment, ILoggerFactory loggerFactory) {
            this.configuration = configuration;
            this.idexHtmlLoader = idexHtmlLoader;
            var staticFileOptions = StaticFileOptionsFrom(new UIStaticFilesProvider());
            staticFilesMiddleware = new StaticFileMiddleware(next, webHostEnvironment, staticFileOptions, loggerFactory);
        }

        public async Task Invoke(HttpContext httpContext) {
            var httpMethod = httpContext.Request.Method;
            var path = httpContext.Request.Path.Value;

            if (IsGet(httpMethod) && RoutePrefixIsRequestedWithOrWithoutSlash(configuration.RoutePrefix, path)) {
                RedirectToIndexHtml(httpContext, path);
                return;
            }
            if (IsGet(httpMethod) && IsIndexHtmlRoute(configuration.RoutePrefix, path)) {
                await RespondWithIndexHtml(httpContext.Response);
                return;
            }
            await staticFilesMiddleware.Invoke(httpContext);
        }

        private IOptions<StaticFileOptions> StaticFileOptionsFrom(UIStaticFilesProvider filesProvider) {
            var staticFileOptions = new StaticFileOptions {
                RequestPath = string.IsNullOrEmpty(configuration.RoutePrefix) ? string.Empty : $"/{configuration.RoutePrefix}",
                FileProvider = new EmbeddedFileProvider(filesProvider.Assembly(), filesProvider.EmbeddedFilesNamespace()),
            };
            return Options.Create(staticFileOptions);
        }

        private static bool IsGet(string httpMethod) {
            return httpMethod == HttpMethods.Get;
        }

        private static bool RoutePrefixIsRequestedWithOrWithoutSlash(string routePrefix, string path) {
            return Regex.IsMatch(path, $"^/?{Regex.Escape(routePrefix)}/?$");
        }

        private static void RedirectToIndexHtml(HttpContext httpContext, string path) {
            var relativeRedirectPath = path.EndsWith("/")
                ? "index.html"
                : $"{path.Split('/').Last()}/index.html";
            RespondWithRedirect(httpContext.Response, relativeRedirectPath);
        }

        private static void RespondWithRedirect(HttpResponse response, string location) {
            response.StatusCode = (int) HttpStatusCode.Redirect;
            response.Headers["Location"] = location;
        }

        private static bool IsIndexHtmlRoute(string routePrefix, string path) {
            return Regex.IsMatch(path, $"^/{Regex.Escape(routePrefix)}/?index.html$");
        }

        private async Task RespondWithIndexHtml(HttpResponse response) {
            response.StatusCode = 200;
            response.ContentType = "text/html;charset=utf-8";
            var rawIndexHtml = await idexHtmlLoader.LoadRawWith(configuration.IndexHtmlDocumentTitle, configuration.RoutePrefix);
            await response.WriteAsync(rawIndexHtml, Encoding.UTF8);
        }

    }
}
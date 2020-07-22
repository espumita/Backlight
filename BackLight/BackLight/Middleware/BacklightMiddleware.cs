using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Backlight.Middleware {
    public class BacklightMiddleware {
        private readonly RequestDelegate next;
        private readonly MiddlewareConfiguration configuration;
        private readonly BacklightIndexHtmlRenderer idexHtmlRenderer;

        public BacklightMiddleware(RequestDelegate next, MiddlewareConfiguration configuration, BacklightIndexHtmlRenderer idexHtmlRenderer) {
            this.next = next;
            this.configuration = configuration;
            this.idexHtmlRenderer = idexHtmlRenderer;
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
            await next.Invoke(httpContext);
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
            var rawIndexHtml = await idexHtmlRenderer.RenderWith(configuration.IndexHtmlDocumentTitle);
             await response.WriteAsync(rawIndexHtml, Encoding.UTF8);
           // var responseStream = await ResponseBodyStreamWith(rawIndexHtml);
           // response.Body = responseStream;// TODO FIX TEST READ BODY
        }

        private async Task<Stream> ResponseBodyStreamWith(string responseBody) {
            var bodyStream = new MemoryStream();
            var streamWriter = new StreamWriter(bodyStream);
            await streamWriter.WriteAsync(responseBody);
            await streamWriter.FlushAsync();
            bodyStream.Seek(0, SeekOrigin.Begin);
            return bodyStream;
        }

    }

}
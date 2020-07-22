using System.IO;
using System.Net;
using System.Threading.Tasks;
using Backlight.Middleware;
using Backlight.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace Backlight.Test.Middleware {
    public class BacklightMiddlewareTests {
        private const string ARoutePrefix = "back";
        private const string AIndexHtmlDocumentTitle = "BackLight";
        private const string ARawIndexHtml = "rawIndexHtml";
        private RequestDelegate next;
        private MiddlewareConfiguration configuration;
        private BacklightMiddleware middleware;
        private DefaultHttpContext httpContext;
        private BacklightIndexHtmlRenderer indexHtmlRenderer;

        [SetUp]
        public void SetUp() {
            next = Substitute.For<RequestDelegate>();
            configuration = new MiddlewareConfiguration();
            indexHtmlRenderer = Substitute.For<BacklightIndexHtmlRenderer>();
            middleware = new BacklightMiddleware(next, configuration, indexHtmlRenderer);
            httpContext = new DefaultHttpContext();
        }

        [Test]
        public async Task redirect_to_index_html_when_route_is_correct_but_do_not_have_slash() {
            configuration.RoutePrefix = ARoutePrefix;
            httpContext.Request.Method = HttpMethods.Get;
            httpContext.Request.Path = new PathString($"/{ARoutePrefix}");

            await middleware.Invoke(httpContext);

            httpContext.Response.StatusCode = (int) HttpStatusCode.Redirect;
            var responseHeader = httpContext.Response.Headers["Location"];
            responseHeader.ToString().Should().Be($"{ARoutePrefix}/index.html");
        }

        [Test]
        public async Task redirect_to_index_html_when_route_is_correct_and_have_slash() {
            configuration.RoutePrefix = ARoutePrefix;
            httpContext.Request.Method = HttpMethods.Get;
            httpContext.Request.Path = new PathString($"/{ARoutePrefix}/");

            await middleware.Invoke(httpContext);

            httpContext.Response.StatusCode = (int)HttpStatusCode.Redirect;
            var responseHeader = httpContext.Response.Headers["Location"];
            responseHeader.ToString().Should().Be("index.html");
        }

        [Test]
        public async Task render_index_html() {
            configuration.RoutePrefix = ARoutePrefix;
            configuration.IndexHtmlDocumentTitle = AIndexHtmlDocumentTitle;
            httpContext.Request.Method = HttpMethods.Get;
            httpContext.Request.Path = new PathString($"/{ARoutePrefix}/index.html");
            indexHtmlRenderer.RenderWith(AIndexHtmlDocumentTitle).Returns(ARawIndexHtml);

            await middleware.Invoke(httpContext);

            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            httpContext.Response.ContentType = "text/html;charset=utf-8";
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be(ARawIndexHtml);
        }

        private static async Task<string> ReadBodyFrom(Stream bodyStream) {
            var memoryStream = new MemoryStream();
            await bodyStream.CopyToAsync(memoryStream);
            var streamReader = new StreamReader(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var readToEndAsync = await streamReader.ReadToEndAsync();
            streamReader.Close();
            return readToEndAsync;
        }
    }
}
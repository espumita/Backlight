using System.IO;
using System.Net;
using System.Threading.Tasks;
using Backlight.Middleware;
using Backlight.UI;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Backlight.Test.Middleware {
    public class BacklightMiddlewareTests {
        private const string AUrlPath = "back";
        private const string AIndexHtmlDocumentTitle = "BackLight";
        private const string ARawIndexHtml = "rawIndexHtml";
        private RequestDelegate next;
        private MiddlewareConfiguration configuration;
        private BacklightMiddleware middleware;
        private DefaultHttpContext httpContext;
        private IndexHtmlLoader indexHtmlLoader;
        private ILoggerFactory loggerFactory;
        private IWebHostEnvironment webHostEnvironment;

        [SetUp]
        public void SetUp() {
            next = Substitute.For<RequestDelegate>();
            configuration = new MiddlewareConfiguration();
            indexHtmlLoader = Substitute.For<IndexHtmlLoader>();
            loggerFactory = Substitute.For<ILoggerFactory>();
            webHostEnvironment = Substitute.For<IWebHostEnvironment>();
            middleware = new BacklightMiddleware(next, configuration, indexHtmlLoader, webHostEnvironment, loggerFactory);
            httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
        }

        [Test]
        public async Task redirect_to_index_html_when_route_is_correct_but_do_not_have_slash() {
            configuration.UrlPath = AUrlPath;
            httpContext.Request.Method = HttpMethods.Get;
            httpContext.Request.Path = new PathString($"/{AUrlPath}");

            await middleware.Invoke(httpContext);

            httpContext.Response.StatusCode = (int) HttpStatusCode.Redirect;
            var responseHeader = httpContext.Response.Headers["Location"];
            responseHeader.ToString().Should().Be($"{AUrlPath}/index.html");
        }

        [Test]
        public async Task redirect_to_index_html_when_route_is_correct_and_have_slash() {
            configuration.UrlPath = AUrlPath;
            httpContext.Request.Method = HttpMethods.Get;
            httpContext.Request.Path = new PathString($"/{AUrlPath}/");

            await middleware.Invoke(httpContext);

            httpContext.Response.StatusCode = (int)HttpStatusCode.Redirect;
            var responseHeader = httpContext.Response.Headers["Location"];
            responseHeader.ToString().Should().Be("index.html");
        }

        [Test]
        public async Task render_index_html() {
            configuration.UrlPath = AUrlPath;
            configuration.IndexHtmlDocumentTitle = AIndexHtmlDocumentTitle;
            httpContext.Request.Method = HttpMethods.Get;
            httpContext.Request.Path = new PathString($"/{AUrlPath}/index.html");
            httpContext.Request.Scheme = "https";
            httpContext.Request.Host = new HostString("localhost", 44349);
            indexHtmlLoader.LoadRawWith(AIndexHtmlDocumentTitle, "https://localhost:44349", AUrlPath).Returns(ARawIndexHtml);

            await middleware.Invoke(httpContext);

            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            httpContext.Response.ContentType = "text/html;charset=utf-8";
            var responseBody = await ReadResponseBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be(ARawIndexHtml);
        }

        private static async Task<string> ReadResponseBodyFrom(Stream bodyStream) {
            bodyStream.CanSeek.Should().BeTrue();
            bodyStream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(bodyStream);
            return await streamReader.ReadToEndAsync();
        }
    }
}
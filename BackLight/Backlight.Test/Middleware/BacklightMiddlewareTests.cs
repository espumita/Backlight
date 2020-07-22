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
        private RequestDelegate next;
        private MiddlewareConfiguration configuration;
        private BacklightService service;
        private BacklightMiddleware middleware;
        private DefaultHttpContext httpContext;

        [SetUp]
        public void SetUp() {
            next = Substitute.For<RequestDelegate>();
            configuration = new MiddlewareConfiguration();
            service = Substitute.For<BacklightService>(new object[] { null });
            middleware = new BacklightMiddleware(next, configuration, service);
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
    }
}
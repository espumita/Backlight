using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Backlight.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;

namespace Backlight.Test {
    public class ApiRunnerTests {
        private IApplicationBuilder applicationBuilder;
        private ApiRunner runner;
        private DefaultHttpContext httpContext;

        [SetUp]
        public void SetUp() {
            applicationBuilder = Substitute.For<IApplicationBuilder>();
            httpContext = new DefaultHttpContext();
            runner = new ApiRunner(applicationBuilder, httpContext);
        }



        [Test, TestCaseSource("NotAllowedMethods")]
        public async Task get_method_not_allowed_response_when_try_to_run_under_a_not_allowed_http_method(string httpMethod) {
            httpContext.Request.Method = httpMethod;

            await runner.Run();

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.MethodNotAllowed);
        }

        public static IEnumerable<string> NotAllowedMethods() {
            yield return HttpMethods.Connect;
            yield return HttpMethods.Head;
            yield return HttpMethods.Options;
            yield return HttpMethods.Patch;
            yield return HttpMethods.Trace;
        }


    }
}
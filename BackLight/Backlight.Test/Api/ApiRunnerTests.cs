using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Backlight.Api;
using Backlight.Api.Serialization;
using Backlight.Exceptions;
using Backlight.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Backlight.Test.Api {
    public class ApiRunnerTests {
        private string AEntityName = typeof(UserEntity).FullName;
        private const string AnEntityId = "anEntityId";
        private const string ASerializedEntity = "aSerializedEntity";
        private ApiRunner runner;
        private DefaultHttpContext httpContext;
        private BacklightService backlightService;
        private StreamSerializer streamSerializer;

        [SetUp]
        public void SetUp() {
            backlightService = Substitute.For<BacklightService>(new object[] { null });
            httpContext = new DefaultHttpContext();
            httpContext.Request.Path = $"/{AnEntityId}";
            httpContext.Response.Body = new MemoryStream();
            streamSerializer = Substitute.For<StreamSerializer>();

            runner = new ApiRunner(backlightService, streamSerializer);
        }

        [Test, TestCaseSource("NotAllowedMethods")]
        public async Task get_method_not_allowed_response_when_try_to_run_under_a_not_allowed_http_method(string httpMethod) {
            httpContext.Request.Method = httpMethod;

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.MethodNotAllowed);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Http method is not allowed");
        }

        [Test, TestCaseSource("AllowedMethods")]
        public async Task get_bad_request_when_the_entity_request_body_deserialization_has_an_error(string httpMethod) {
            httpContext.Request.Method = httpMethod;
            streamSerializer.EntityRequestBodyFrom(Arg.Any<Stream>()).Throws(new EntityRequestBodyDeserializationException());

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity deserialization error");
        }

        [Test, TestCaseSource("AllowedMethods")]
        public async Task get_bad_request_when_entity_type_is_not_configured(string httpMethod) {
            httpContext.Request.Method = httpMethod;
            GivenARequestBodyWith(new EntityRequestBody {
                TypeName = AEntityName,
                PayLoad = string.Empty
            });
            backlightService.Create(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityIsNotConfiguredException>();
            backlightService.Read(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityIsNotConfiguredException>();
            backlightService.Update(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Throws<EntityIsNotConfiguredException>();
            backlightService.Delete(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityIsNotConfiguredException>();

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity is not configured");
        }

        [Test, TestCaseSource("AllowedMethods")]
        public async Task get_bad_request_when_entity_provider_available(string httpMethod) {
            httpContext.Request.Method = httpMethod;
            GivenARequestBodyWith(new EntityRequestBody {
                TypeName = AEntityName
            });
            backlightService.Create(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityProviderIsNotAvailableException>();
            backlightService.Read(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityProviderIsNotAvailableException>();
            backlightService.Update(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Throws<EntityProviderIsNotAvailableException>();
            backlightService.Delete(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityProviderIsNotAvailableException>();

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity provider is not available");
        }

        [Test, TestCaseSource("AllowedMethods")]
        public async Task get_bad_request_when_entity_deserialization_throws_an_exception(string httpMethod) {
            httpContext.Request.Method = httpMethod;
            GivenARequestBodyWith(new EntityRequestBody {
                TypeName = AEntityName
            });
            backlightService.Create(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityDeserializationException>();
            backlightService.Read(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityDeserializationException>();
            backlightService.Update(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Throws<EntityDeserializationException>();
            backlightService.Delete(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityDeserializationException>();

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity payload deserialization error");
        }

        [Test, TestCaseSource("BadPathsWithMethods")]
        public async Task get_bad_request_when_request_has_bad_path((string httpMethod, string path) testCaseSource) {
            httpContext.Request.Method = testCaseSource.httpMethod;
            httpContext.Request.Path = testCaseSource.path;

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Error in entity id path");
        }

        [Test]
        public async Task execute_service_create_entity() {
            var httpMethod = HttpMethods.Put;
            httpContext.Request.Path = string.Empty;
            httpContext.Request.Method = httpMethod;
            var anEntityPayload = new EntityRequestBody {
                TypeName = AEntityName,
                PayLoad = ASerializedEntity
            };
            GivenARequestBodyWith(anEntityPayload);
            backlightService.Create(AEntityName, ASerializedEntity).Returns(AnEntityId);

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be($"Enity created with id: {AnEntityId}");
        }

        [Test]
        public async Task execute_service_read_entity() {
            var httpMethod = HttpMethods.Get;
            httpContext.Request.Method = httpMethod;
            httpContext.Request.Path = $"/{AnEntityId}";
            var anEntityPayLoad = new EntityRequestBody {
                TypeName = AEntityName
            };
            GivenARequestBodyWith(anEntityPayLoad);
            backlightService.Read(AEntityName, AnEntityId).Returns(ASerializedEntity);

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be(ASerializedEntity);
        }

        [Test]
        public async Task execute_service_update_entity() {
            var httpMethod = HttpMethods.Post;
            httpContext.Request.Method = httpMethod;
            httpContext.Request.Path = $"/{AnEntityId}";
            var anEntityPayload = new EntityRequestBody {
                TypeName = AEntityName,
                PayLoad = ASerializedEntity
            };
            GivenARequestBodyWith(anEntityPayload);

            await runner.Run(httpContext);

            await backlightService.Received().Update(AEntityName, AnEntityId, ASerializedEntity);
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity updated");
        }

        [Test]
        public async Task execute_delete_entity_provider() {
            var httpMethod = HttpMethods.Delete;
            httpContext.Request.Method = httpMethod;
            httpContext.Request.Path = $"/{AnEntityId}";
            var anEntityPayload = new EntityRequestBody {
                TypeName = AEntityName
            };
            GivenARequestBodyWith(anEntityPayload);

            await runner.Run(httpContext);

            await backlightService.Received().Delete(AEntityName, AnEntityId);
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity deleted");
        }

        public static IEnumerable<string> NotAllowedMethods() {
            yield return HttpMethods.Connect;
            yield return HttpMethods.Head;
            yield return HttpMethods.Options;
            yield return HttpMethods.Patch;
            yield return HttpMethods.Trace;
        }

        public static IEnumerable<string> AllowedMethods() {
            yield return HttpMethods.Put;
            yield return HttpMethods.Get;
            yield return HttpMethods.Post;
            yield return HttpMethods.Delete;
        }

        public static IEnumerable<(string httpMethod, string path)> BadPathsWithMethods() {
            return new List<string> {
                HttpMethods.Get,
                HttpMethods.Post,
                HttpMethods.Delete
            }.SelectMany(method => {
                return BadPaths().Select(path => (method, path));
            });
        }

        private static IEnumerable<string> BadPaths() {
            yield return "/";
            yield return "//";
            yield return $"/{AnEntityId}/";
            yield return $"/{AnEntityId}/{AnEntityId}";
            yield return string.Empty;
        }

        private static async Task<string> ReadBodyFrom(Stream bodyStream) {
            bodyStream.CanSeek.Should().BeTrue();
            bodyStream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(bodyStream);
            return await streamReader.ReadToEndAsync();
        }
        private void GivenARequestBodyWith(EntityRequestBody entityRequestBody) {
            streamSerializer.EntityRequestBodyFrom(Arg.Any<Stream>())
                .Returns(entityRequestBody);
        }
    }

}
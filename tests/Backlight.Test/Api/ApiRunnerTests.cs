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
            backlightService = Substitute.For<BacklightService>(null, null);
            httpContext = new DefaultHttpContext();
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

        [Test, TestCaseSource("BadTypePathsWithMethods")]
        public async Task get_bad_request_when_request_has_bad_type_path((string httpMethod, string path) testCaseSource) {
            httpContext.Request.Method = testCaseSource.httpMethod;
            httpContext.Request.Path = testCaseSource.path;

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Type error in path");
        }

        [Test, TestCaseSource("BadTypesPaths")]
        public async Task get_bad_request_when_request_has_bad_type_path_for_puth_method(string path) {
            httpContext.Request.Method = HttpMethods.Put;
            httpContext.Request.Path = path;

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Type error in path");
        }

        [Test, TestCaseSource("BadEntityIdsPathsWithMethods")]
        public async Task get_bad_request_when_request_has_bad_entity_path((string httpMethod, string path) testCaseSource) {
            httpContext.Request.Method = testCaseSource.httpMethod;
            httpContext.Request.Path = testCaseSource.path;

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Error in entity id path");
        }

        [Test, TestCaseSource("AllowedMethodsWithPath")]
        public async Task get_bad_request_when_entity_type_is_not_configured((string httpMethod, string path) testCaseSource) {
            httpContext.Request.Method = testCaseSource.httpMethod;
            httpContext.Request.Path = testCaseSource.path;
            GivenARequestBodyWith(string.Empty);
            backlightService.Create(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityIsNotConfiguredException>();
            backlightService.Read(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityIsNotConfiguredException>();
            backlightService.Update(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Throws<EntityIsNotConfiguredException>();
            backlightService.Delete(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityIsNotConfiguredException>();

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity is not configured");
        }

        [Test, TestCaseSource("AllowedMethodsWithPath")]
        public async Task get_bad_request_when_entity_provider_available((string httpMethod, string path) testCaseSource) {
            httpContext.Request.Method = testCaseSource.httpMethod;
            httpContext.Request.Path = testCaseSource.path;
            backlightService.Create(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityProviderIsNotAvailableException>();
            backlightService.Read(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityProviderIsNotAvailableException>();
            backlightService.Update(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Throws<EntityProviderIsNotAvailableException>();
            backlightService.Delete(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityProviderIsNotAvailableException>();

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity provider is not available");
        }

        [Test, TestCaseSource("AllowedMethodsWithPath")]
        public async Task get_bad_request_when_entity_deserialization_throws_an_exception((string httpMethod, string path) testCaseSource) {
            httpContext.Request.Method = testCaseSource.httpMethod;
            httpContext.Request.Path = testCaseSource.path;
            backlightService.Create(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityDeserializationException>();
            backlightService.Read(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityDeserializationException>();
            backlightService.Update(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Throws<EntityDeserializationException>();
            backlightService.Delete(Arg.Any<string>(), Arg.Any<string>()).Throws<EntityDeserializationException>();

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity payload deserialization error");
        }

        [Test]
        public async Task execute_service_create_entity() {
            httpContext.Request.Method = HttpMethods.Put;
            httpContext.Request.Path = $"/types/{AEntityName}";
            var anEntityPayload = ASerializedEntity;
            GivenARequestBodyWith(anEntityPayload);
            backlightService.Create(AEntityName, ASerializedEntity).Returns(AnEntityId);

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be($"Enity created with id: {AnEntityId}");
        }

        [Test]
        public async Task execute_service_read_entity() {
            httpContext.Request.Path = $"/types/{AEntityName}/entities/{AnEntityId}";
            httpContext.Request.Method = HttpMethods.Get;
            backlightService.Read(AEntityName, AnEntityId).Returns(ASerializedEntity);

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be(ASerializedEntity);
        }

        [Test]
        public async Task execute_service_update_entity() {
            httpContext.Request.Method = HttpMethods.Post;
            httpContext.Request.Path = $"/types/{AEntityName}/entities/{AnEntityId}";
            var anEntityPayload = ASerializedEntity;
            GivenARequestBodyWith(anEntityPayload);

            await runner.Run(httpContext);

            await backlightService.Received().Update(AEntityName, AnEntityId, ASerializedEntity);
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity updated");
        }

        [Test]
        public async Task execute_delete_entity_provider() {
            httpContext.Request.Method = HttpMethods.Delete;
            httpContext.Request.Path = $"/types/{AEntityName}/entities/{AnEntityId}";

            await runner.Run(httpContext);

            await backlightService.Received().Delete(AEntityName, AnEntityId);
            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be("Enity deleted");
        }

        [Test]
        public async Task execute_read_all_ids_service() {
            httpContext.Request.Path = $"/types/{AEntityName}/all";
            httpContext.Request.Method = HttpMethods.Get;
            backlightService.ReadAllIds(AEntityName).Returns($"[\"${AnEntityId}\"]");

            await runner.Run(httpContext);

            httpContext.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(httpContext.Response.Body);
            responseBody.Should().Be($"[\"${AnEntityId}\"]");
        }

        public static IEnumerable<string> NotAllowedMethods() {
            yield return HttpMethods.Connect;
            yield return HttpMethods.Head;
            yield return HttpMethods.Options;
            yield return HttpMethods.Patch;
            yield return HttpMethods.Trace;
        }

        public static IEnumerable<(string httpMethod, string path)> AllowedMethodsWithPath() {
            return new List<(string httpMethod, string path)> {
                (HttpMethods.Put, "/types/Backlight.Test.UserEntity"),
                (HttpMethods.Get, "/types/Backlight.Test.UserEntity/entities/anEntityId"),
                (HttpMethods.Post, "/types/Backlight.Test.UserEntity/entities/anEntityId"),
                (HttpMethods.Delete, "/types/Backlight.Test.UserEntity/entities/anEntityId")
            };
        }

        public static IEnumerable<(string httpMethod, string path)> BadTypePathsWithMethods() {
            return new List<string> {
                HttpMethods.Get,
                HttpMethods.Post,
                HttpMethods.Delete
            }.SelectMany(method => {
                return BadTypesPaths().Select(path => (method, path));
            });
        }

        private static IEnumerable<string> BadTypesPaths() {
            yield return string.Empty;
            yield return "/";
            yield return "//";
            yield return "/types/";
            yield return "/types//";
            yield return "/types/Backlight.Test.UserEntity/";
            yield return "/types/Backlight.Test.UserEntity/Backlight.Test.UserEntity";
        }
        public static IEnumerable<(string httpMethod, string path)> BadEntityIdsPathsWithMethods() {
            return new List<string> {
                HttpMethods.Get,
                HttpMethods.Post,
                HttpMethods.Delete
            }.SelectMany(method => {
                return BadEntityIdPaths().Select(path => (method, path));
            });
        }

        private static IEnumerable<string> BadEntityIdPaths() {
            yield return "/types/Backlight.Test.UserEntity/entities/";
            yield return "/types/Backlight.Test.UserEntity/entities//";
            yield return "/types/Backlight.Test.UserEntity/entities/anEntityId/";
            yield return "/types/Backlight.Test.UserEntity/entities/anEntityId//";
        }

        private static async Task<string> ReadBodyFrom(Stream bodyStream) {
            bodyStream.CanSeek.Should().BeTrue();
            bodyStream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(bodyStream);
            return await streamReader.ReadToEndAsync();
        }
        private void GivenARequestBodyWith(string entityPayload) {
            streamSerializer.EntityPayloadFrom(Arg.Any<Stream>())
                .Returns(entityPayload);
        }
    }

}
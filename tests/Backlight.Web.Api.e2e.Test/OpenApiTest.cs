using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Backlight.Sample.Web.Api.Entities;
using FluentAssertions;
using Newtonsoft.Json;
using NJsonSchema;
using NSwag;
using NUnit.Framework;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Backlight.Web.Api.e2e.Test {
    public class OpenApiTest {
        private HttpClient client;

        [SetUp]
        public void SetUp() {
            client = TestFixture.serverClient;
        }

        //TODO
        //test dates
        //test complex types

        [Test]
        public async Task get_open_api_definition_from_sample() {
            var requestUri = $"/back/OpenApi.json";

            var response = await client.GetAsync(requestUri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(response.Content);
            var document = JsonConvert.DeserializeObject<OpenApiDocument>(responseBody);
            document.OpenApi.Should().Be("3.0.0");
            VerifyOpenApiDocumentInfo(document);

            VerifyCreationFor<ExampleEntity>(document);
            VerifyReadFor<ExampleEntity>(document);
            VerifyUpdateFor<ExampleEntity>(document);

            VerifyCreationFor<ExampleEntity2>(document);
            VerifyReadFor<ExampleEntity2>(document);
            VerifyUpdateFor<ExampleEntity2>(document);
            VerifyDeleteFor<ExampleEntity2>(document);

            VerifyReadFor<ExampleEntity3>(document);
        }

        private static void VerifyCreationFor<T>(OpenApiDocument document) {
            document.Paths.ContainsKey($"/api/type/{typeof(T).FullName}").Should().BeTrue();
            var putPath = document.Paths[$"/api/type/{typeof(T).FullName}"];
            putPath.ContainsKey(OpenApiOperationMethod.Put).Should().BeTrue();
            var openApiOperation = putPath[OpenApiOperationMethod.Put];
            openApiOperation.OperationId.Should().Be($"{typeof(T).FullName}-put");
            openApiOperation.RequestBody.IsRequired.Should().BeTrue();
            openApiOperation.RequestBody.Content.ContainsKey("application/json").Should().BeTrue();
            var requestBodyContent = openApiOperation.RequestBody.Content["application/json"];
            //TODO requestBodyContent.Schema.Should().BeEquivalentTo(JsonSchema.FromType<T>());
            openApiOperation.Responses.ContainsKey("200").Should().BeTrue();
        }

        private static void VerifyReadFor<T>(OpenApiDocument document) {
            document.Paths.ContainsKey($"/api/type/{typeof(T).FullName}/entity/{{id}}").Should().BeTrue();
            var getPath = document.Paths[$"/api/type/{typeof(T).FullName}/entity/{{id}}"];
            getPath.ContainsKey(OpenApiOperationMethod.Get).Should().BeTrue();
            var openApiOperation = getPath[OpenApiOperationMethod.Get];
            openApiOperation.OperationId.Should().Be($"{typeof(T).FullName}-get");
            openApiOperation.Responses.ContainsKey("200").Should().BeTrue();
            //TODO RESPONSES BODY FILL SCHEME JsonSchema.FromType<T>() ;
        }

        private static void VerifyUpdateFor<T>(OpenApiDocument document) {
            document.Paths.ContainsKey($"/api/type/{typeof(T).FullName}/entity/{{id}}").Should().BeTrue();
            var postPath = document.Paths[$"/api/type/{typeof(T).FullName}/entity/{{id}}"];
            postPath.ContainsKey(OpenApiOperationMethod.Post).Should().BeTrue();
            var openApiOperation = postPath[OpenApiOperationMethod.Post];
            openApiOperation.OperationId.Should().Be($"{typeof(T).FullName}-post");
            openApiOperation.RequestBody.IsRequired.Should().BeTrue();
            openApiOperation.RequestBody.Content.ContainsKey("application/json").Should().BeTrue();
            var requestBodyContent = openApiOperation.RequestBody.Content["application/json"];
            //TODO requestBodyContent.Schema.Should().BeEquivalentTo(JsonSchema.FromType<T>());
            openApiOperation.Responses.ContainsKey("200").Should().BeTrue();
        }

        private static void VerifyDeleteFor<T>(OpenApiDocument document) {
            document.Paths.ContainsKey($"/api/type/{typeof(T).FullName}/entity/{{id}}").Should().BeTrue();
            var postPath = document.Paths[$"/api/type/{typeof(T).FullName}/entity/{{id}}"];
            postPath.ContainsKey(OpenApiOperationMethod.Delete).Should().BeTrue();
            var openApiOperation = postPath[OpenApiOperationMethod.Delete];
            openApiOperation.OperationId.Should().Be($"{typeof(T).FullName}-delete");
            //TODO requestBodyContent.Schema.Should().BeEquivalentTo(JsonSchema.FromType<T>());
            openApiOperation.Responses.ContainsKey("200").Should().BeTrue();
        }

        private static void VerifyOpenApiDocumentInfo(OpenApiDocument document) {
            document.Info.Title.Should().Be("Backlight");
            document.Info.Description.Should().Be("Backlight Api Entity types definitions");
            document.Info.Version.Should().Be("1.0.0");
        }

        private static async Task<string> ReadBodyFrom(HttpContent httpContent) {
            var memoryStream = new MemoryStream();
            await httpContent.CopyToAsync(memoryStream);
            memoryStream.CanSeek.Should().BeTrue();
            memoryStream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(memoryStream);
            return await streamReader.ReadToEndAsync();
        }
    }

}
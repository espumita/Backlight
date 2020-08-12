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

        [Test]
        public async Task read() {
            var requestUri = $"/back/OpenApi.json";

            var response = await client.GetAsync(requestUri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseBody = await ReadBodyFrom(response.Content);
            var document = JsonConvert.DeserializeObject<OpenApiDocument>(responseBody);
            document.OpenApi.Should().Be("3.0.0");
            VerifyOpenApiDocumentInfo(document);
            VerifyCreationFor<ExampleEntity>(document);
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
            //requestBodyContent.Schema.Should().BeEquivalentTo(JsonSchema.FromType<T>());
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

    public class OpenApiDocumentTest {
        public string OpenApi { get; set; }
        public OpenApiInfoTest Info { get; set; }
        public OpenApiPathsTest Paths { get; set; }
    }

    public class OpenApiInfoTest {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }

    }

    public class OpenApiPathsTest {
        [JsonProperty("/api/type/{fullName}")]
        public string PuthPath { get; set; }

    }
}
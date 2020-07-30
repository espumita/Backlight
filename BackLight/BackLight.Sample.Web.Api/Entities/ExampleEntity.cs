using System.Text.Json.Serialization;

namespace Backlight.Sample.Web.Api.Entities {
    public class ExampleEntity : BacklightEntity {

        [JsonPropertyName("asd")]
        public string Name { get; set; } = "EntityName";

    }
}
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Api.Serialization;
using FluentAssertions;
using NUnit.Framework;

namespace Backlight.Test.Api.Serialization {
    public class MemoryStreamSerializerTests {
        private MemoryStreamSerializer serializer;
        private readonly UserEntity aUserEntity = new UserEntity{ Age = 23, Name = "aName" };

        [SetUp]
        public void SetUp() {
            serializer = new MemoryStreamSerializer();
        }

        [Test]
        public async Task get_entity_payload_from_raw_stream() {
            var rawEntity = GivenASerializedEntiyPayload(aUserEntity);
            var aStream = new MemoryStream(Encoding.ASCII.GetBytes(rawEntity));

            var entityPayload = await serializer.EntityPayloadFrom(aStream);

            entityPayload.Should().Be(JsonSerializer.Serialize(aUserEntity));
        }

        private string GivenASerializedEntiyPayload(UserEntity userEntity) {
            return JsonSerializer.Serialize(userEntity);
        }

    }
}
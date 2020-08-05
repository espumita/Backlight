using System;
using System.Collections.Generic;
using Backlight.Exceptions;
using Backlight.Services.EntitySerialization;
using FluentAssertions;
using NUnit.Framework;

namespace Backlight.Test.Services.EntitySerialization {
    public class JsonEntitySerializerTests {
        private const int anAge = 23;
        private const string aName = "aName";
        private JsonEntitySerializer serializer;

        [SetUp]
        public void SetUp() {
            serializer = new JsonEntitySerializer();
        }

        [Test]
        public void serialize() {
            var aUserEntity = new UserEntity{ Age = anAge, Name = aName};

            var rawEntity = serializer.Serialize(aUserEntity, typeof(UserEntity));

            rawEntity.Should().BeEquivalentTo($"{{\"Name\":\"{aName}\",\"Age\":{anAge}}}");
        }

        [Test]
        public void deserialize() {
            var aUserEntity = new UserEntity { Age = anAge, Name = aName };

            var rawEntity = serializer.Deserialize($"{{\"Name\":\"{aName}\",\"Age\":{anAge}}}", typeof(UserEntity));

            rawEntity.Should().BeEquivalentTo(aUserEntity);
        }

        [Test]
        public void deserialize_empty_entity() {

            var rawEntity = serializer.Deserialize("{}", typeof(UserEntity));

            rawEntity.Should().BeEquivalentTo(new UserEntity());
        }

        [Test]
        public void deserialize_only_correct_properties() {

            var rawEntity = serializer.Deserialize($"{{\"Another\":\"{aName}\",\"Age\":{anAge}}}", typeof(UserEntity));

            rawEntity.Should().BeEquivalentTo(new UserEntity{ Age = anAge });
        }

        [Test, TestCaseSource("BadRawEntityes")]
        public void throw_an_exception_when_deserialization_goes_wrong(string rawEntity) {
            Action action = () => serializer.Deserialize(rawEntity, typeof(UserEntity));

            action.Should().Throw<EntityDeserializationException>();
        }

        public static IEnumerable<string> BadRawEntityes() {
            yield return null;
            yield return string.Empty;
            yield return "[]";
        }

    }

}
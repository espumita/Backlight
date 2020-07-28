﻿using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Backlight.Api.Serialization;
using Backlight.Exceptions;
using FluentAssertions;
using NUnit.Framework;

namespace Backlight.Test.Api.Serialization {
    public class JsonStreamSerializerTests {
        private JsonStreamSerializer serializer;
        private readonly UserEntity aUserEntity = new UserEntity{ Age = 23, Name = "aName" };

        [SetUp]
        public void SetUp() {
            serializer = new JsonStreamSerializer();
        }

        [Test]
        public async Task get_entity_payload_from_raw_stream() {
            var rawEntity = GivenASerializedEntiyPayload(aUserEntity);
            var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(rawEntity));

            var entityPayload = await serializer.EntityRequestBodyFrom(memoryStream);

            entityPayload.TypeName.Should().Be(aUserEntity.GetType().FullName);
            entityPayload.PayLoad.Should().Be(JsonSerializer.Serialize(aUserEntity));
        }

        [Test]
        public async Task throw_an_exception_when_there_is_no_type() {
            var entityRequestBody = new EntityRequestBody {
                PayLoad = JsonSerializer.Serialize(aUserEntity)
            };
            var rawEntity = JsonSerializer.Serialize(entityRequestBody);
            var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(rawEntity));

           Func<Task> action = async () => await serializer.EntityRequestBodyFrom(memoryStream);

           await action.Should().ThrowAsync<EntityDeserializationException>();
        }


        [Test]
        public async Task throw_an_exception_when_there_is_no_payload() {
            var entityRequestBody = new EntityRequestBody {
                TypeName = aUserEntity.GetType().FullName
            };
            var rawEntity = JsonSerializer.Serialize(entityRequestBody);
            var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(rawEntity));

            Func<Task> action = async () => await serializer.EntityRequestBodyFrom(memoryStream);

            await action.Should().ThrowAsync<EntityDeserializationException>();
        }

        private string GivenASerializedEntiyPayload(UserEntity userEntity) {
            var entityRequestBody = new EntityRequestBody {
                TypeName = userEntity.GetType().FullName,
                PayLoad = JsonSerializer.Serialize(userEntity)
            };
            return JsonSerializer.Serialize(entityRequestBody);
        }

    }
}
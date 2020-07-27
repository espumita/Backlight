using System.Threading.Tasks;
using Backlight.Api.Serialization;
using Backlight.Providers;
using Backlight.Services;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Backlight.Test.Services {
    public class BacklightServiceTests {
        private string AEntityName = typeof(UserEntity).FullName;
        private const string ASerializedEntity = "aSerializedEntity";
        private const string AnEntityId = "anEntityId";
        private const string AName = "aName";
        private const int AnAge = 12;

        [SetUp]
        public void SetUp() {
        }

        [Test]
        public async Task use_provider_on_create() {
            var createProvider = Substitute.For<CreateProvider>();
            var entitySerializer = Substitute.For<EntitySerializer>();
            var serviceOptions = new ServiceOptions(entitySerializer);
            serviceOptions.For<UserEntity>().AddCreate(createProvider);
            var backlightService = new BacklightService(serviceOptions);
            var aUserEntity = new UserEntity { Age = AnAge, Name = AName };
            entitySerializer.Deserialize<UserEntity>(ASerializedEntity).Returns(aUserEntity);
            var anEntityPayload = new EntityPayload {
                TypeName = AEntityName,
                Value = ASerializedEntity
            };

            await backlightService.Create(anEntityPayload);

            await createProvider.Received().Create(aUserEntity);
        }

        [Test]
        public async Task use_provider_on_read() {
            var readProvider = Substitute.For<ReadProvider>();
            var entitySerializer = Substitute.For<EntitySerializer>();
            var serviceOptions = new ServiceOptions(entitySerializer);
            serviceOptions.For<UserEntity>().AddRead(readProvider);
            var backlightService = new BacklightService(serviceOptions);
            var aUserEntity = new UserEntity { Age = AnAge, Name = AName };
            var anEntityPayload = new EntityPayload {
                TypeName = AEntityName,
                Value = AnEntityId
            };
            readProvider.Read<UserEntity>(AnEntityId).Returns(aUserEntity);
            entitySerializer.Serialize(aUserEntity).Returns(ASerializedEntity);

            var readedEntity = await backlightService.Read(anEntityPayload);
            readedEntity.Should().Be(ASerializedEntity);
        }

        [Test]
        public async Task use_provider_on_update() {
            var updateProvider = Substitute.For<UpdateProvider>();
            var entitySerializer = Substitute.For<EntitySerializer>();
            var serviceOptions = new ServiceOptions(entitySerializer);
            serviceOptions.For<UserEntity>().AddUpdate(updateProvider);
            var backlightService = new BacklightService(serviceOptions);
            var aUserEntity = new UserEntity { Age = AnAge, Name = AName };
            entitySerializer.Deserialize<UserEntity>(ASerializedEntity).Returns(aUserEntity);
            var anEntityPayload = new EntityPayload {
                TypeName = AEntityName,
                Value = ASerializedEntity
            };

            await backlightService.Update(anEntityPayload);

            await updateProvider.Received().Update("TODOEntityId", aUserEntity);
        }

        [Test]
        public async Task use_provider_on_delete() {
            var deleteProvider = Substitute.For<DeleteProvider>();
            var entitySerializer = Substitute.For<EntitySerializer>();
            var serviceOptions = new ServiceOptions(entitySerializer);
            serviceOptions.For<UserEntity>().AddDelete(deleteProvider);
            var backlightService = new BacklightService(serviceOptions);
            var anEntityPayload = new EntityPayload {
                TypeName = AEntityName,
                Value = AnEntityId
            };

            await backlightService.Delete(anEntityPayload);

            await deleteProvider.Received().Delete<UserEntity>(AnEntityId);
        }

    }
}
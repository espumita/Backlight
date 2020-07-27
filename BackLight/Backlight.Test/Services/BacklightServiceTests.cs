using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Services;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Backlight.Test.Services {
    public class BacklightServiceTests {
        private string AEntityName = typeof(UserEntity).FullName;
        private EntitySerializer serializer;
        private ServiceOptions options;
        private const int AnAge = 12;
        private const string AName = "aName";
        private UserEntity aUserEntity = new UserEntity { Age = AnAge, Name = AName };
        private const string ASerializedEntity = "aSerializedEntity";
        private const string AnEntityId = "anEntityId";

        [SetUp]
        public void SetUp() {
            serializer = Substitute.For<EntitySerializer>();
            options = new ServiceOptions(serializer);
        }

        [Test]
        public async Task use_provider_on_create() {
            var provider = Substitute.For<CreateProvider>();
            options.For<UserEntity>().AddCreate(provider);
            serializer.Deserialize<UserEntity>(ASerializedEntity).Returns(aUserEntity);
            
            await ServiceWith(options).Create(AEntityName, ASerializedEntity);

            await provider.Received().Create(aUserEntity);
        }

        [Test]
        public async Task use_provider_on_read() {
            var provider = Substitute.For<ReadProvider>();
            options.For<UserEntity>().AddRead(provider);
            provider.Read<UserEntity>(AnEntityId).Returns(aUserEntity);
            serializer.Serialize(aUserEntity).Returns(ASerializedEntity);
            
            var readedEntity = await ServiceWith(options).Read(AEntityName, AnEntityId);

            readedEntity.Should().Be(ASerializedEntity);
        }

        [Test]
        public async Task use_provider_on_update() {
            var provider = Substitute.For<UpdateProvider>();
            options.For<UserEntity>().AddUpdate(provider);
            serializer.Deserialize<UserEntity>(ASerializedEntity).Returns(aUserEntity);

            await ServiceWith(options).Update(AEntityName, AnEntityId, ASerializedEntity);

            await provider.Received().Update(AnEntityId, aUserEntity);
        }

        [Test]
        public async Task use_provider_on_delete() {
            var provider = Substitute.For<DeleteProvider>();
            options.For<UserEntity>().AddDelete(provider);

            await ServiceWith(options).Delete(AEntityName, AnEntityId);

            await provider.Received().Delete<UserEntity>(AnEntityId);
        }

        private BacklightService ServiceWith(ServiceOptions options) {
            return new BacklightService(options);
        }

    }
}
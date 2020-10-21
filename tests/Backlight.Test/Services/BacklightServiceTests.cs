using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backlight.Providers;
using Backlight.Services;
using Backlight.Services.EntitySerialization;
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
            options = new ServiceOptions();
        }

        [Test]
        public async Task use_provider_on_create() {
            var provider = Substitute.For<CreateProvider>();
            options.For<UserEntity>().AddCreate(provider);
            serializer.Deserialize(ASerializedEntity, typeof(UserEntity)).Returns(aUserEntity);
            provider.Create(aUserEntity).Returns(AnEntityId);

            var entityId = await ServiceWith(options).Create(AEntityName, ASerializedEntity);

            entityId.Should().Be(AnEntityId);
        }

        [Test]
        public async Task use_provider_on_read() {
            var provider = Substitute.For<ReadProviderForTest>();
            options.For<UserEntity>().AddRead(provider);
            provider.Read(AnEntityId, typeof(UserEntity)).Returns(aUserEntity);
            serializer.Serialize(aUserEntity, typeof(UserEntity)).Returns(ASerializedEntity);
            
            var readedEntity = await ServiceWith(options).Read(AEntityName, AnEntityId);

            readedEntity.Should().Be(ASerializedEntity);
        }

        [Test]
        public async Task use_provider_on_read_all_ids() {
            var provider = Substitute.For<ReadProviderForTest>();
            options.For<UserEntity>().AddRead(provider);
            provider.ReadAllIds().Returns(new List<string>{ AnEntityId });
            serializer.Serialize(Arg.Is<List<string>>(x => x.Count == 1 && x.First().Equals(AnEntityId)), typeof(List<string>)).Returns($"[\"${AnEntityId}\"]");

            var allEntitiesIds = await ServiceWith(options).ReadAllIds(AEntityName);

            allEntitiesIds.Should().Be($"[\"${AnEntityId}\"]");
        }


        [Test]
        public async Task use_provider_on_update() {
            var provider = Substitute.For<UpdateProvider>();
            options.For<UserEntity>().AddUpdate(provider);
            serializer.Deserialize(ASerializedEntity, typeof(UserEntity)).Returns(aUserEntity);

            await ServiceWith(options).Update(AEntityName, AnEntityId, ASerializedEntity);

            await provider.Received().Update(AnEntityId, aUserEntity);
        }

        [Test]
        public async Task use_provider_on_delete() {
            var provider = Substitute.For<DeleteProvider>();
            options.For<UserEntity>().AddDelete(provider);

            await ServiceWith(options).Delete(AEntityName, AnEntityId);

            await provider.Received().Delete(AnEntityId);
        }

        private BacklightService ServiceWith(ServiceOptions options) {
            return new BacklightService(options, serializer);
        }

        public interface ReadProviderForTest : ReadProvider, ReadAllIdsProvider {

        }

    }


}
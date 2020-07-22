using System;
using System.Linq;
using Backlight.Exceptions;
using Backlight.Providers;
using Backlight.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace Backlight.Test.Services {
    public class BacklightServiceCollectionExtensionsTests {
        private const string AnEntityId = "anEntityId";
        private ServiceCollection collection;
        private UserEntity AUserEntity = new UserEntity{ Name = "aName", Age = 23};

        [SetUp]
        public void SetUp() {
            collection = new ServiceCollection();
        }

        [Test]
        public void be_not_configured_when_there_is_no_options_configured() {

            collection.AddBacklight();

            var service = VerifyServiceOfType<BacklightProvidersService>();
            service.Options.ProvidersForType.Should().BeEmpty();
        }

        [Test]
        public void be_configured_with_a_create_provider() {
            var createProvider = Substitute.For<CreateProvider>();

            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCreate(createProvider);
            });

            var service = VerifyServiceOfType<BacklightProvidersService>();
            service.Options.ProvidersForType.Count.Should().Be(1);
            var providerForType = service.Options.ProvidersForType[typeof(UserEntity)];
            VerifyOnlyCan(providerForType, create: true);
        }

        [Test]
        public void be_configured_with_a_read_provider() {
            var readProvider = Substitute.For<ReadProvider>();
            readProvider.Read<UserEntity>(AnEntityId).Returns(AUserEntity);
            
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddRead(readProvider);
            });

            var service = VerifyServiceOfType<BacklightProvidersService>();
            service.Options.ProvidersForType.Count.Should().Be(1);
            var providerForType = service.Options.ProvidersForType[typeof(UserEntity)];
            VerifyOnlyCan(providerForType, read: true);
        }

        [Test]
        public void be_configured_with_a_update_provider() {
            var updateProvider = Substitute.For<UpdateProvider>();

            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddUpdate(updateProvider);
            });

            var service = VerifyServiceOfType<BacklightProvidersService>();
            service.Options.ProvidersForType.Count.Should().Be(1);
            var providerForType = service.Options.ProvidersForType[typeof(UserEntity)];
            VerifyOnlyCan(providerForType, update: true);
        }

        [Test]
        public void be_configured_with_a_delete_provider() {
            var deleteProvider = Substitute.For<DeleteProvider>();

            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddDelete(deleteProvider);
            });

            var service = VerifyServiceOfType<BacklightProvidersService>();
            service.Options.ProvidersForType.Count.Should().Be(1);
            var providerForType = service.Options.ProvidersForType[typeof(UserEntity)];
            VerifyOnlyCan(providerForType, delete: true);
        }

        [Test]
        public void be_configured_with_a_crud_provider() {
            var crudProvider = Substitute.For<CRUDProvider>();

            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCRUD(crudProvider);
            });

            var service = VerifyServiceOfType<BacklightProvidersService>();
            service.Options.ProvidersForType.Count.Should().Be(1);
            var providerForType = service.Options.ProvidersForType[typeof(UserEntity)];
            VerifyOnlyCan(providerForType, create: true, read: true, update: true, delete: true);
        }

        [Test]
        public void throw_an_exception_when_try_to_add_the_same_entity_twice() {
            var createProvider = Substitute.For<CreateProvider>();

            Action action = () => collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCreate(createProvider);
                configuration.For<UserEntity>()
                    .AddCreate(createProvider);
            });

            action.Should().Throw<CannotConfigureTheSameEntityTwiceException>();
        }

        private T VerifyServiceOfType<T>() {
            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(T));
            return (T) serviceDescriptor.ImplementationInstance;
        }

        private static void VerifyOnlyCan(ProviderForTypeForTypeOptions providerForType, bool create = false, bool read = false, bool update = false, bool delete = false) {
            providerForType.CanCreate().Should().Be(create);
            providerForType.CanRead().Should().Be(read);
            providerForType.CanUpdate().Should().Be(update);
            providerForType.CanDelete().Should().Be(delete);
        }

    }

}
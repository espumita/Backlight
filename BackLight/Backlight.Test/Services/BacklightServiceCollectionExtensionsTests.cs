using System;
using System.Linq;
using Backlight.Api;
using Backlight.Api.Serialization;
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

            var service = VerifyServiceOfType<BacklightService>();
            service.Options.ProvidersForType.Should().BeEmpty();
            VerifySingletonIsConfiguredForServiceOfType<StreamSerializer>();
            VerifySingletonIsConfiguredForServiceOfType<ApiRunner>();
        }

        [Test]
        public void be_configured_with_a_create_provider() {
            var createProvider = Substitute.For<CreateProvider>();

            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCreate(createProvider);
            });

            var service = VerifyServiceOfType<BacklightService>();
            service.Options.ProvidersForType.Count.Should().Be(1);
            var providerForType = service.Options.ProvidersForType[typeof(UserEntity)];
            VerifyOnlyCan(providerForType, create: true);
        }

        [Test]
        public void be_configured_with_a_read_provider() {
            var readProvider = Substitute.For<ReadProvider>();
            readProvider.Read(AnEntityId, typeof(UserEntity)).Returns(AUserEntity);
            
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddRead(readProvider);
            });

            var service = VerifyServiceOfType<BacklightService>();
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

            var service = VerifyServiceOfType<BacklightService>();
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

            var service = VerifyServiceOfType<BacklightService>();
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

            var service = VerifyServiceOfType<BacklightService>();
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


        [Test]
        public void throw_an_exception_when_try_to_add_a_creation_provider_twice() {
            var createProvider = Substitute.For<CreateProvider>();

            Action action = () => collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCreate(createProvider)
                    .AddCreate(createProvider);
            });

            action.Should().Throw<CannotConfigureTheSameProviderTwiceException>();
        }

        [Test]
        public void throw_an_exception_when_try_to_add_a_read_provider_twice() {
            var readProvider = Substitute.For<ReadProvider>();

            Action action = () => collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddRead(readProvider)
                    .AddRead(readProvider);
            });

            action.Should().Throw<CannotConfigureTheSameProviderTwiceException>();
        }

        [Test]
        public void throw_an_exception_when_try_to_add_a_update_provider_twice() {
            var updateProvider = Substitute.For<UpdateProvider>();

            Action action = () => collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddUpdate(updateProvider)
                    .AddUpdate(updateProvider);
            });

            action.Should().Throw<CannotConfigureTheSameProviderTwiceException>();
        }

        [Test]
        public void throw_an_exception_when_try_to_add_a_delete_provider_twice() {
            var deleteProvider = Substitute.For<DeleteProvider>();

            Action action = () => collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddDelete(deleteProvider)
                    .AddDelete(deleteProvider);
            });

            action.Should().Throw<CannotConfigureTheSameProviderTwiceException>();
        }

        [Test]
        public void throw_an_exception_when_try_to_add_a_crud_provider_twice() {
            var crudProvider = Substitute.For<CRUDProvider>();

            Action action = () => collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCRUD(crudProvider)
                    .AddCRUD(crudProvider);
            });

            action.Should().Throw<CannotConfigureTheSameProviderTwiceException>();
        }

        private T VerifyServiceOfType<T>() {
            var serviceDescriptor = collection.FirstOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == typeof(T));
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            return (T) serviceDescriptor.ImplementationInstance;
        }

        private void VerifySingletonIsConfiguredForServiceOfType<T>() {
            var serviceDescriptor = collection.FirstOrDefault(serviceDescriptor => serviceDescriptor.ServiceType == typeof(T));
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
        }

        private static void VerifyOnlyCan(ProviderForTypeOptions providerForType, bool create = false, bool read = false, bool update = false, bool delete = false) {
            providerForType.CanCreate().Should().Be(create);
            providerForType.CanRead().Should().Be(read);
            providerForType.CanUpdate().Should().Be(update);
            providerForType.CanDelete().Should().Be(delete);
        }

    }

}
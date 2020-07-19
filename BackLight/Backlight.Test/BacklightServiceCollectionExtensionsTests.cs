﻿using System.Linq;
using System.Text.Json;
using Backlight.Providers;
using Backlight.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace Backlight.Test {
    public class BacklightServiceCollectionExtensionsTests {
        private ServiceCollection collection;

        [SetUp]
        public void SetUp() {
            collection = new ServiceCollection();
        }

        [Test]
        public void be_not_configured_when_there_is_no_configuration() {
            collection.AddBacklight();

            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService)serviceDescriptor.ImplementationInstance;
            var configuration = backlightProvidersService.Configuration;
            configuration.Providers.Should().BeEmpty();
            configuration.CreateProvidersDelegates.Should().BeEmpty();
            configuration.ReadProvidersDelegates.Should().BeEmpty();
            configuration.UpdateProvidersDelegates.Should().BeEmpty();
            configuration.DeleteProvidersDelegates.Should().BeEmpty();
        }

        [Test]
        public void be_configued_with_a_create_provider() {
            var createProvider = Substitute.For<CreateProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddCreate(createProvider);
            });

            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService)serviceDescriptor.ImplementationInstance;
            var configuration = backlightProvidersService.Configuration;
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            keyValuePair.Value.CanCreate().Should().BeTrue();
            var valuePair = configuration.CreateProvidersDelegates.Single();
            valuePair.Key.Should().Be<UserEntity>();
            var userEntity = new UserEntity{ Name = "aName", Age = 23};
            valuePair.Value(JsonSerializer.Serialize(userEntity));
            createProvider.Received().Create(userEntity);
            //configuration.ReadProvidersDelegates.Should().BeEmpty();
            //configuration.UpdateProvidersDelegates.Should().BeEmpty();
            //configuration.DeleteProvidersDelegates.Should().BeEmpty();
        }

        [Test]
        public void be_configued_with_a_update_provider() {
            var updateProvider = Substitute.For<UpdateProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddUpdate(updateProvider);
            });

            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService)serviceDescriptor.ImplementationInstance;
            var configuration = backlightProvidersService.Configuration;
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            keyValuePair.Value.CanUpdate().Should().BeTrue();
            var valuePair = configuration.UpdateProvidersDelegates.Single();
            valuePair.Key.Should().Be<UserEntity>();
            var userEntity = new UserEntity { Name = "aName", Age = 23 };
            const string AnEntityId = "anEntityId";
            valuePair.Value(AnEntityId, JsonSerializer.Serialize(userEntity));
            updateProvider.Received().Update(AnEntityId, userEntity);
            //configuration.ReadProvidersDelegates.Should().BeEmpty();
            //configuration.CreateProvidersDelegates.Should().BeEmpty();
            //configuration.DeleteProvidersDelegates.Should().BeEmpty();
        }

        [Test]
        public void be_configued_with_a_delete_provider() {
            var deleteProvider = Substitute.For<DeleteProvider>();
            collection.AddBacklight(configuration => {
                configuration.For<UserEntity>()
                    .AddDelete(deleteProvider);
            });

            var serviceDescriptor = collection.Single();
            serviceDescriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
            serviceDescriptor.ServiceType.Should().Be(typeof(BacklightProvidersService));
            var backlightProvidersService = (BacklightProvidersService)serviceDescriptor.ImplementationInstance;
            var configuration = backlightProvidersService.Configuration;
            var keyValuePair = configuration.Providers.Single();
            keyValuePair.Key.Should().Be<UserEntity>();
            keyValuePair.Value.CanDelete().Should().BeTrue();
            var valuePair = configuration.DeleteProvidersDelegates.Single();
            valuePair.Key.Should().Be<UserEntity>();
            const string AnEntityId = "anEntityId";
            valuePair.Value(AnEntityId);
            deleteProvider.Received().Delete<UserEntity>(AnEntityId);
            //configuration.ReadProvidersDelegates.Should().BeEmpty();
            //configuration.UpdateProvidersDelegates.Should().BeEmpty();
            //configuration.CreateProvidersDelegates.Should().BeEmpty();
        }
    }
}
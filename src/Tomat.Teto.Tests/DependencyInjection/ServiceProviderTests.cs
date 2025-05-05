using NUnit.Framework;

using Tomat.Teto.Bot.DependencyInjection;
using Tomat.Teto.Bot.DependencyInjection.Models;

// ReSharper disable InconsistentNaming

namespace Tomat.Teto.Tests.DependencyInjection;

[TestFixture]
public static class ServiceProviderTests
{
    private class GenericService : IService;

#region Simple Tests
    [Test]
    public static void GetService_FalseWhenNotAdded()
    {
        var provider = MakeProvider();
        Assert.That(provider.TryGetService<GenericService>(out _), Is.False);
    }

    [Test]
    public static void GetService_TrueWhenAdded()
    {
        var provider = MakeProvider();
        provider.TryAddService<GenericService>();
        Assert.That(provider.TryGetService<GenericService>(out _), Is.True);
    }

    [Test]
    public static void AddService_SuccessfulWhenNotAddedAndOverwriteFalse()
    {
        var provider = MakeProvider();
        Assert.That(provider.TryAddService<GenericService>().Successful, Is.True);
    }

    [Test]
    public static void AddService_UnsuccessfulWhenAddedAndOverwriteFalse()
    {
        var provider = MakeProvider();
        provider.TryAddService<GenericService>();
        Assert.That(provider.TryAddService<GenericService>().Successful, Is.False);
    }

    [Test]
    public static void AddService_SuccessfulWhenAddedAndOverwriteTrue()
    {
        var provider = MakeProvider();
        provider.TryAddService<GenericService>();
        Assert.That(provider.TryAddService<GenericService>(true).Successful, Is.True);
    }

    [Test]
    public static void AddService_SuccessfulWhenNotAddedAndOverwriteTrue()
    {
        var provider = MakeProvider();
        Assert.That(provider.TryAddService<GenericService>(true).Successful, Is.True);
    }

    [Test]
    public static void RemoveService_SuccessfulWhenAdded()
    {
        var provider = MakeProvider();
        provider.TryAddService<GenericService>();
        Assert.That(provider.TryRemoveService<GenericService>().Successful, Is.True);
    }

    [Test]
    public static void RemoveService_SuccessfulWhenNotAdded()
    {
        var provider = MakeProvider();
        Assert.That(provider.TryRemoveService<GenericService>().Successful, Is.True);
    }

    [Test]
    public static void GetService_TrueWhenAddedToParent()
    {
        var parent = MakeProvider();
        parent.TryAddService<GenericService>();
        var provider = MakeProvider(parent);
        Assert.That(provider.TryGetService<GenericService>(out _), Is.True);
    }
#endregion

#region Advanced Tests
    // private class AdvancedService : IService { }

    private class ServiceWithPreferredConstructor
    {
        private readonly IServiceProvider? provider;

        public ServiceWithPreferredConstructor() { }

        [PreferredServiceConstructor]
        public ServiceWithPreferredConstructor([ServiceProvider] IServiceProvider provider)
        {
            this.provider = provider;
        }

        public bool Validate()
        {
            return provider is not null;
        }
    }

    private class ServiceWithHooks : IService
    {
        private bool ran1;
        private bool ran2;
        private bool ran3;
        private bool ran4;
        private bool ran5;
        private bool ran6;

        [ServiceHook(ServiceHook.PreConstruct)]
        private void Test1()
        {
            ran1 = true;
        }

        [ServiceHook(ServiceHook.PreConstruct)]
        private void Test2()
        {
            ran2 = true;
        }

        [ServiceHook(ServiceHook.PostConstruct)]
        private void Test3()
        {
            ran3 = true;
        }

        [ServiceHook(ServiceHook.PostConstruct)]
        private void Test4()
        {
            ran4 = true;
        }

        [ServiceHook(ServiceHook.OnRemoval)]
        private void Test5()
        {
            ran5 = true;
        }

        [ServiceHook(ServiceHook.OnRemoval)]
        private void Test6()
        {
            ran6 = true;
        }

        public bool Validate()
        {
            return ran1 && ran2 && ran3 && ran4 && ran5 && ran6;
        }
    }

    [Test]
    public static void TestServiceWithPreferredConstructor()
    {
        var provider = MakeProvider();
        provider.TryAddService<ServiceWithPreferredConstructor>();
        Assert.That(provider.ExpectService<ServiceWithPreferredConstructor>().Validate(), Is.True);
    }

    [Test]
    public static void TestServiceWithHooks()
    {
        var provider = MakeProvider();
        provider.TryAddService<ServiceWithHooks>();
        var service = provider.ExpectService<ServiceWithHooks>();
        provider.TryRemoveService<ServiceWithHooks>();
        Assert.That(service.Validate(), Is.True);
    }
#endregion

#pragma warning disable CA1859 - Imitating hidden implementation.
    private static IServiceProvider MakeProvider(IServiceProvider? parent = null)
    {
        return new ServiceProvider(parent);
    }
#pragma warning restore CA1859
}
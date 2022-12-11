using Blowin.ChainOfResponsibility.DependencyInjection;
using Blowin.ChainOfResponsibility.Finally;
using Blowin.ChainOfResponsibility.Middleware;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Blowin.ChainOfResponsibility.Tests;

public class ServiceCollectionTest
{
    [Fact]
    public void Should_Create_ChainOfResponsibility_With_Any_Generic_Values()
    {
        var provider = new ServiceCollection()
            .AddChainOfResponsibility(typeof(ServiceCollectionExt).Assembly, cnf => cnf.WithMiddlewareSingletonLifetime())
            .BuildServiceProvider();

        var service = provider.GetRequiredService<ChainOfResponsibility<string, bool>>();
        var service2 = provider.GetRequiredService<ChainOfResponsibility<int, string>>();
        var service3 = provider.GetRequiredService<ChainOfResponsibility<(int, int), long>>();
        var service4 = provider.GetRequiredService<ChainOfResponsibility<decimal, string>>();

        service.Should().NotBeNull();
        service2.Should().NotBeNull();
        service3.Should().NotBeNull();
        service4.Should().NotBeNull();
    }

    [Fact]
    public void EmptyAssembly_Should_Not_Add_Services()
    {
        var provider = new ServiceCollection().AddChainOfResponsibility(typeof(ServiceCollectionExt).Assembly, cnf => cnf.WithMiddlewareSingletonLifetime());

        provider.Count.Should().Be(1);
    }

    [Theory]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Transient)]
    public void Should_Register_Service_With_Right_Lifetime(ServiceLifetime lifetime)
    {
        var provider = new ServiceCollection()
            .AddChainOfResponsibility(typeof(ServiceCollectionExt).Assembly, cnf =>
            {
                cnf
                    .WithMiddlewareSingletonLifetime()
                    .WithChainOfResponsibilityLifetime(lifetime);
            });

        provider.Count.Should().Be(1);
        provider.Should().ContainSingle(descriptor => descriptor.ImplementationType == typeof(ChainOfResponsibility<,>) && descriptor.Lifetime == lifetime);
    }

    [Fact]
    public void One_Assembly_Should_Add_Services()
    {
        var provider = new ServiceCollection().AddChainOfResponsibility(typeof(FuncMiddleware<,>).Assembly, cnf => cnf.WithMiddlewareSingletonLifetime());

        provider.Count.Should().Be(3);
        provider.Should().ContainSingle(descriptor => descriptor.ImplementationType == typeof(FuncMiddleware<,>));
        provider.Should().ContainSingle(descriptor => descriptor.ImplementationType == typeof(FuncFinally<,>));
    }

    [Theory]
    [InlineData(ServiceLifetime.Scoped)]
    [InlineData(ServiceLifetime.Singleton)]
    [InlineData(ServiceLifetime.Transient)]
    public void Two_Assembly_Should_Add_Services(ServiceLifetime lifetime)
    {
        var provider = new ServiceCollection().AddChainOfResponsibility(new[] { typeof(FuncMiddleware<,>).Assembly, typeof(CustomMiddleware).Assembly }, cnf => cnf.WithMiddlewareLifetime(lifetime));

        provider.Count.Should().Be(5);
        provider.Should().ContainSingle(descriptor => descriptor.ImplementationType == typeof(FuncMiddleware<,>) && descriptor.Lifetime == lifetime);
        provider.Should().ContainSingle(descriptor => descriptor.ImplementationType == typeof(FuncFinally<,>) && descriptor.Lifetime == lifetime);
        provider.Should().ContainSingle(descriptor => descriptor.ImplementationType == typeof(CustomMiddleware) && descriptor.Lifetime == lifetime);
        provider.Should().ContainSingle(descriptor => descriptor.ImplementationType == typeof(CustomMiddleware2) && descriptor.Lifetime == lifetime);
    }

    public sealed class CustomMiddleware : IMiddleware<string, bool>
    {
        public bool Run(string parameter, Func<string, bool> next)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class CustomMiddleware2 : IMiddleware<string, string>
    {
        public string Run(string parameter, Func<string, string> next)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class CustomAbstractMiddleware : IMiddleware<string, string>
    {
        public string Run(string parameter, Func<string, string> next)
        {
            throw new NotImplementedException();
        }
    }
}

using Blowin.ChainOfResponsibility.Finally;
using Blowin.ChainOfResponsibility.Middleware;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Blowin.ChainOfResponsibility.Tests
{
    public class ChainOfResponsibilityTest
    {
        [Fact]
        public void Execute_Empty()
        {
            var provider = new ServiceCollection()
                .AddSingleton(typeof(ChainOfResponsibility<,>))
                .BuildServiceProvider();
            var chain = provider.GetRequiredService<ChainOfResponsibility<string, bool>>();

            Assert.Throws<InvalidOperationException>(() => chain.Execute(string.Empty));
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("12345", false)]
        [InlineData("123456", true)]
        public void Execute_Only_Finally(string inputValue, bool expectedResult)
        {
            var provider = new ServiceCollection()
                .AddSingleton<IFinally<string, bool>>(new FuncFinally<string, bool>(s => s.Length > 5))
                .AddSingleton(typeof(ChainOfResponsibility<,>))
                .BuildServiceProvider();
            var chain = provider.GetRequiredService<ChainOfResponsibility<string, bool>>();

            var result = chain.Execute(inputValue);

            result.Should().Be(expectedResult, $"'{inputValue}'.Length > 5");
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("Test")]
        [InlineData("qf qwf qwfq")]
        public void Execute_With_1_Middleware_Should_Be_Success(string inputData)
        {
            var provider = new ServiceCollection()
                .AddSingleton<IMiddleware<string, bool>>(new FuncMiddleware<string, bool>((s, _) => s == inputData))
                .AddSingleton(typeof(ChainOfResponsibility<,>))
                .BuildServiceProvider();
            var chain = provider.GetRequiredService<ChainOfResponsibility<string, bool>>();

            var result = chain.Execute(inputData);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("Test")]
        [InlineData("qf qwf qwfq")]
        public void Execute_With_2_Middleware_Should_Be_Success(string inputData)
        {
            var provider = new ServiceCollection()
                .AddSingleton(typeof(ChainOfResponsibility<,>))
                .AddSingleton<IMiddleware<string, bool>>(new FuncMiddleware<string, bool>((s, next) => next(s)))
                .AddSingleton<IMiddleware<string, bool>>(new FuncMiddleware<string, bool>((s, next) => s == inputData))
                .BuildServiceProvider();
            var chain = provider.GetRequiredService<ChainOfResponsibility<string, bool>>();

            var result = chain.Execute(inputData);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("Test")]
        [InlineData("qf qwf qwfq")]
        public void Execute_With_3_Middleware_Should_Be_Success(string inputData)
        {
            var provider = new ServiceCollection()
                .AddSingleton(typeof(ChainOfResponsibility<,>))
                .AddSingleton<IMiddleware<string, bool>>(new FuncMiddleware<string, bool>((s, next) => next(s)))
                .AddSingleton<IMiddleware<string, bool>>(new FuncMiddleware<string, bool>((s, next) => next(s)))
                .AddSingleton<IMiddleware<string, bool>>(new FuncMiddleware<string, bool>((s, next) => s == inputData))
                .BuildServiceProvider();
            var chain = provider.GetRequiredService<ChainOfResponsibility<string, bool>>();

            var result = chain.Execute(inputData);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("Test")]
        [InlineData("qf qwf qwfq")]
        public void Execute_With_1_Middleware_Should_Be_Fail_Without_Appropriate_Middleware_And_Finally(string inputData)
        {
            var provider = new ServiceCollection()
                .AddSingleton(typeof(ChainOfResponsibility<,>))
                .AddSingleton<IMiddleware<string, bool>>(new FuncMiddleware<string, bool>((s, next) => next(s)))
                .BuildServiceProvider();
            var chain = provider.GetRequiredService<ChainOfResponsibility<string, bool>>();

            Assert.Throws<InvalidOperationException>(() => chain.Execute(inputData));
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("Test")]
        [InlineData("qf qwf qwfq")]
        public void Execute_With_2_Middleware_Should_Be_Fail_Without_Appropriate_Middleware_And_Finally(string inputData)
        {
            var provider = new ServiceCollection()
                .AddSingleton(typeof(ChainOfResponsibility<,>))
                .AddSingleton<IMiddleware<string, bool>>(new FuncMiddleware<string, bool>((s, next) => next(s)))
                .AddSingleton<IMiddleware<string, bool>>(new FuncMiddleware<string, bool>((s, next) => next(s)))
                .BuildServiceProvider();
            var chain = provider.GetRequiredService<ChainOfResponsibility<string, bool>>();

            Assert.Throws<InvalidOperationException>(() => chain.Execute(inputData));
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("Test")]
        [InlineData("qf qwf qwfq")]
        public void Execute_With_3_Middleware_Should_Be_Fail_Without_Appropriate_Middleware_And_Finally(string inputData)
        {
            var provider = new ServiceCollection()
                .AddSingleton(typeof(ChainOfResponsibility<,>))
                .AddSingleton<IMiddleware<string, bool>>(new FuncMiddleware<string, bool>((s, next) => next(s)))
                .AddSingleton<IMiddleware<string, bool>>(new FuncMiddleware<string, bool>((s, next) => next(s)))
                .AddSingleton<IMiddleware<string, bool>>(new FuncMiddleware<string, bool>((s, next) => next(s)))
                .BuildServiceProvider();
            var chain = provider.GetRequiredService<ChainOfResponsibility<string, bool>>();

            Assert.Throws<InvalidOperationException>(() => chain.Execute(inputData));
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("Test")]
        [InlineData("qf qwf qwfq")]
        public void Execute_With_1_Middleware_Should_Be_Success_With_Finally(string inputData)
        {
            var provider = new ServiceCollection()
                .AddSingleton(typeof(ChainOfResponsibility<,>))
                .AddSingleton<IMiddleware<string, string>>(new FuncMiddleware<string, string>((s, next) => next(s)))
                .AddSingleton<IFinally<string, string>>(new FuncFinally<string, string>(s => s))
                .BuildServiceProvider();
            var chain = provider.GetRequiredService<ChainOfResponsibility<string, string>>();

            var result = chain.Execute(inputData);

            result.Should().Be(inputData);
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("Test")]
        [InlineData("qf qwf qwfq")]
        public void Execute_With_2_Middleware_Should_Be_Success_With_Finally(string inputData)
        {
            var provider = new ServiceCollection()
                .AddSingleton(typeof(ChainOfResponsibility<,>))
                .AddSingleton<IMiddleware<string, string>>(new FuncMiddleware<string, string>((s, next) => next(s)))
                .AddSingleton<IMiddleware<string, string>>(new FuncMiddleware<string, string>((s, next) => next(s)))
                .AddSingleton<IFinally<string, string>>(new FuncFinally<string, string>(s => s))
                .BuildServiceProvider();
            var chain = provider.GetRequiredService<ChainOfResponsibility<string, string>>();

            var result = chain.Execute(inputData);

            result.Should().Be(inputData);
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("Test")]
        [InlineData("qf qwf qwfq")]
        public void Execute_With_3_Middleware_Should_Be_Success_With_Finally(string inputData)
        {
            var provider = new ServiceCollection()
                .AddSingleton(typeof(ChainOfResponsibility<,>))
                .AddSingleton<IMiddleware<string, string>>(new FuncMiddleware<string, string>((s, next) => next(s)))
                .AddSingleton<IMiddleware<string, string>>(new FuncMiddleware<string, string>((s, next) => next(s)))
                .AddSingleton<IMiddleware<string, string>>(new FuncMiddleware<string, string>((s, next) => next(s)))
                .AddSingleton<IFinally<string, string>>(new FuncFinally<string, string>(s => s))
                .BuildServiceProvider();
            var chain = provider.GetRequiredService<ChainOfResponsibility<string, string>>();

            var result = chain.Execute(inputData);

            result.Should().Be(inputData);
        }

        [Theory]
        [InlineData("", "", "", "t", "t123")]
        [InlineData("a", "b", "c", "a", "a")]
        [InlineData("a", "b11", "c", "b1", "b11")]
        [InlineData("a", "b", "c12", "c", "c12")]
        [InlineData("a", "b", "c12", "d", "d123")]
        public void Execute_With_3_Middleware_Should_Call_Right_Number_Of_Times(string v1, string v2, string v3, string parameter, string expectedResult)
        {
            var provider = new ServiceCollection()
                .AddSingleton(typeof(ChainOfResponsibility<,>))
                .AddSingleton<IMiddleware<string, string>>(new FuncMiddleware<string, string>((s, next) => s == v1 ? s : next(s + "1")))
                .AddSingleton<IMiddleware<string, string>>(new FuncMiddleware<string, string>((s, next) => s == v2 ? s : next(s + "2")))
                .AddSingleton<IMiddleware<string, string>>(new FuncMiddleware<string, string>((s, next) => s == v3 ? s : next(s + "3")))
                .AddSingleton<IFinally<string, string>>(new FuncFinally<string, string>(s => s))
                .BuildServiceProvider();
            var chain = provider.GetRequiredService<ChainOfResponsibility<string, string>>();

            var result = chain.Execute(parameter);

            result.Should().Be(expectedResult);
        }
    }
}
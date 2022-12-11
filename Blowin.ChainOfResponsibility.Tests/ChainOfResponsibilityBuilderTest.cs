using FluentAssertions;

namespace Blowin.ChainOfResponsibility.Tests
{
    public class ChainOfResponsibilityBuilderTest
    {
        [Fact]
        public void Execute_Empty()
        {
            var chain = ChainOfResponsibilityBuilder.Create<string, bool>().Build();

            Assert.Throws<InvalidOperationException>(() => chain.Execute(string.Empty));
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("12345", false)]
        [InlineData("123456", true)]
        public void Execute_Only_Finally(string inputValue, bool expectedResult)
        {
            var chain = ChainOfResponsibilityBuilder.Create<string, bool>()
                .WithFinally(s => s.Length > 5)
                .Build();

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
            var chain = ChainOfResponsibilityBuilder.Create<string, bool>()
                .WithMiddleware((s, _) => s == inputData)
                .Build();

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
            var chain = ChainOfResponsibilityBuilder.Create<string, bool>()
                .WithMiddleware((s, next) => next(s))
                .WithMiddleware((s, _) => s == inputData)
                .Build();

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
            var chain = ChainOfResponsibilityBuilder.Create<string, bool>()
                .WithMiddleware((s, next) => next(s))
                .WithMiddleware((s, next) => next(s))
                .WithMiddleware((s, _) => s == inputData)
                .Build();

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
            var chain = ChainOfResponsibilityBuilder.Create<string, bool>()
                .WithMiddleware((s, next) => next(s))
                .Build();

            Assert.Throws<InvalidOperationException>(() => chain.Execute(inputData));
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("Test")]
        [InlineData("qf qwf qwfq")]
        public void Execute_With_2_Middleware_Should_Be_Fail_Without_Appropriate_Middleware_And_Finally(string inputData)
        {
            var chain = ChainOfResponsibilityBuilder.Create<string, bool>()
                .WithMiddleware((s, next) => next(s))
                .WithMiddleware((s, next) => next(s))
                .Build();

            Assert.Throws<InvalidOperationException>(() => chain.Execute(inputData));
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("Test")]
        [InlineData("qf qwf qwfq")]
        public void Execute_With_3_Middleware_Should_Be_Fail_Without_Appropriate_Middleware_And_Finally(string inputData)
        {
            var chain = ChainOfResponsibilityBuilder.Create<string, bool>()
                .WithMiddleware((s, next) => next(s))
                .WithMiddleware((s, next) => next(s))
                .WithMiddleware((s, next) => next(s))
                .Build();

            Assert.Throws<InvalidOperationException>(() => chain.Execute(inputData));
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("Test")]
        [InlineData("qf qwf qwfq")]
        public void Execute_With_1_Middleware_Should_Be_Success_With_Finally(string inputData)
        {
            var chain = ChainOfResponsibilityBuilder.Create<string, string>()
                .WithMiddleware((s, next) => next(s))
                .WithFinally((s) => s)
                .Build();

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
            var chain = ChainOfResponsibilityBuilder.Create<string, string>()
                .WithMiddleware((s, next) => next(s))
                .WithMiddleware((s, next) => next(s))
                .WithFinally((s) => s)
                .Build();

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
            var chain = ChainOfResponsibilityBuilder.Create<string, string>()
                .WithMiddleware((s, next) => next(s))
                .WithMiddleware((s, next) => next(s))
                .WithMiddleware((s, next) => next(s))
                .WithFinally((s) => s)
                .Build();

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
            var chain = ChainOfResponsibilityBuilder.Create<string, string>()
                .WithMiddleware((s, next) => s == v1 ? s : next(s + "1"))
                .WithMiddleware((s, next) => s == v2 ? s : next(s + "2"))
                .WithMiddleware((s, next) => s == v3 ? s : next(s + "3"))
                .WithFinally((s) => s)
                .Build();

            var result = chain.Execute(parameter);

            result.Should().Be(expectedResult);
        }
    }
}

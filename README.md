# Blowin.ChainOfResponsibility

![CI](https://img.shields.io/github/workflow/status/blowin/Blowin.ChainOfResponsibility/build)
[![NuGet](https://img.shields.io/nuget/v/Blowin.ChainOfResponsibility)](https://www.nuget.org/packages/Blowin.ChainOfResponsibility/)
[![NuGet (DI)](https://img.shields.io/nuget/v/Blowin.ChainOfResponsibility.DependencyInjection)](https://www.nuget.org/packages/Blowin.ChainOfResponsibility.DependencyInjection/)

Simple [chain of responsibility](https://en.wikipedia.org/wiki/Chain-of-responsibility_pattern) implementation in .NET

## There are 2 main concepts in the library

### IMiddleware

The link in the query processing, can interrupt the execution or pass the processing on, using Func<TIn, TOut> next.

```csharp
public interface IMiddleware<TIn, TOut>
{
    TOut Run(TIn parameter, Func<TIn, TOut> next);
}
```

### IFinally

Called if the call chain has reached the end and has not been interrupted by one of the IMiddleware. By default, if IFinally is not specified, an exception will be thrown

```csharp
public interface IFinally<in TIn, out TOut>
{
    TOut Run(TIn parameter);
}
```

There are also extensions for asynchronous versions

```csharp
public interface IAsyncFinally<TIn, TOut> : IFinally<AsyncRequest<TIn>, Task<TOut>>
{
}

public interface IAsyncMiddleware<TIn, TOut> : IMiddleware<AsyncRequest<TIn>, Task<TOut>>
{
}

public sealed class AsyncRequest<T> : IEquatable<AsyncRequest<T>>
{
    public T Parameter { get; }

    public CancellationToken CancellationToken { get; }

    public AsyncRequest(T parameter);

    public AsyncRequest(T parameter, CancellationToken cancellationToken);
}
```

# Samples

Thanks to these 2 interfaces, you can create a flexible pipeline.

## ChainOfResponsibilityBuilder

There are several methods to create ChainOfResponsibility

```csharp
public sealed class ChainOfResponsibilityBuilder<T, TRes>
{
  public ChainOfResponsibilityBuilder<T, TRes> WithFinally(IFinally<T, TRes> @finally);

  public ChainOfResponsibilityBuilder<T, TRes> WithMiddleware(IMiddleware<T, TRes> middleware);

  public ChainOfResponsibilityBuilder<T, TRes> WithFinally(Func<T, TRes> @finally);
  
  public ChainOfResponsibilityBuilder<T, TRes> WithMiddleware(Func<T, Func<T, TRes>, TRes> middleware);
  
  public ChainOfResponsibility<T, TRes> Build();
}
```

There are factory methods to create a synchronous and asynchronous version of ChainOfResponsibility

```csharp
public static class ChainOfResponsibilityBuilder
{
    public static ChainOfResponsibilityBuilder<T, TRes> Create<T, TRes>();

    public static ChainOfResponsibilityBuilder<AsyncRequest<T>, Task<TRes>> CreateAsync<T, TRes>();
}
```

Usage

```csharp
var chain = ChainOfResponsibilityBuilder.Create<string, string>()
    .WithMiddleware((s, func) => func(s + " World"))
    .WithFinally(s => s + " from ChainOfResponsibility")
    .Build();

var result = chain.Execute("Hello");

Console.WriteLine(result); // Hello World from ChainOfResponsibility
```

## Dependency injection

```csharp
var provider = new ServiceCollection()
    .AddChainOfResponsibility(typeof(ServiceCollectionExt).Assembly) // Register
    .BuildServiceProvider();

var service = provider.GetRequiredService<ChainOfResponsibility<string, bool>>();
```

You can also specify a delegate as the second parameter, which will configure the service.

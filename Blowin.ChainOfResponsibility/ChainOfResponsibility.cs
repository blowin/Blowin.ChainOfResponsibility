using System;
using System.Collections.Generic;
using System.Linq;
using Blowin.ChainOfResponsibility.Finally;
using Blowin.ChainOfResponsibility.Middleware;

namespace Blowin.ChainOfResponsibility
{
    public class ChainOfResponsibility<T, TRes>
    {
        private readonly Lazy<Func<T, TRes>> _lazyHandler;

        internal ChainOfResponsibility(IEnumerable<IMiddleware<T, TRes>> middlewares, IFinally<T, TRes> finallyBlock)
        {
            _lazyHandler = new Lazy<Func<T, TRes>>(() => MergeChain(middlewares, finallyBlock));
        }
        
        public ChainOfResponsibility(IEnumerable<IMiddleware<T, TRes>> middlewares, IEnumerable<IFinally<T, TRes>> finallyBlocks)
            : this(middlewares, finallyBlocks?.FirstOrDefault())
        {
        }
        
        public TRes Execute(T parameter) => _lazyHandler.Value(parameter);

        private static Func<T, TRes> MergeChain(IEnumerable<IMiddleware<T, TRes>> middlewares, IFinally<T, TRes> finallyBlock)
        {
            var @finally = finallyBlock ?? new FallbackFinally();
            Func<T, TRes> handler = arg => @finally.Run(arg);
            var middlewareList = middlewares ?? Array.Empty<IMiddleware<T, TRes>>();
            foreach (var middleware in middlewareList.Reverse())
            {
                var handler1 = handler;
                handler = arg => middleware.Run(arg, handler1);
            }

            return handler;
        }

        private sealed class FallbackFinally : IFinally<T, TRes>
        {
            public TRes Run(T parameter) => throw new InvalidOperationException($"Unhandled parameter '{parameter}'");
        }
    }
}

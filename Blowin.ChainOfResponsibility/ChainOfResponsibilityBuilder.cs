using System;
using System.Collections.Generic;
using Blowin.ChainOfResponsibility.Finally;
using Blowin.ChainOfResponsibility.Middleware;

namespace Blowin.ChainOfResponsibility
{
    public sealed class ChainOfResponsibilityBuilder<T, TRes>
    {
        private readonly List<IMiddleware<T, TRes>> _middlewares = new List<IMiddleware<T, TRes>>();
        private IFinally<T, TRes> _finally;

        public ChainOfResponsibilityBuilder<T, TRes> WithFinally(IFinally<T, TRes> @finally)
        {
            _finally = @finally;
            return this;
        }

        public ChainOfResponsibilityBuilder<T, TRes> WithMiddleware(IMiddleware<T, TRes> middleware)
        {
            _middlewares.Add(middleware);
            return this;
        }

        public ChainOfResponsibilityBuilder<T, TRes> WithFinally(Func<T, TRes> @finally)
        {
            return WithFinally(new FuncFinally<T, TRes>(@finally));
        }

        public ChainOfResponsibilityBuilder<T, TRes> WithMiddleware(Func<T, Func<T, TRes>, TRes> middleware)
        {
            return WithMiddleware(new FuncMiddleware<T, TRes>(middleware));
        }

        public ChainOfResponsibility<T, TRes> Build()
        {
            return new ChainOfResponsibility<T, TRes>(_middlewares, _finally);
        }
    }
}

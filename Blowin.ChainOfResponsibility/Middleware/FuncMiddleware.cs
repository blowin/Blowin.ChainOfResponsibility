using System;

namespace Blowin.ChainOfResponsibility.Middleware
{
    public sealed class FuncMiddleware<TIn, TOut> : IMiddleware<TIn, TOut>
    {
        private readonly Func<TIn, Func<TIn, TOut>, TOut> _handler;

        public FuncMiddleware(Func<TIn, Func<TIn, TOut>, TOut> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public TOut Run(TIn parameter, Func<TIn, TOut> next) => _handler(parameter, next);
    }
}
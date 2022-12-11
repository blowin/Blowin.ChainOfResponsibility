using System;

namespace Blowin.ChainOfResponsibility.Middleware
{
    /// <inheritdoc />
    public sealed class FuncMiddleware<TIn, TOut> : IMiddleware<TIn, TOut>
    {
        private readonly Func<TIn, Func<TIn, TOut>, TOut> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncMiddleware{TIn, TOut}"/> class.
        /// </summary>
        /// <param name="handler">Handler as delegate.</param>
        public FuncMiddleware(Func<TIn, Func<TIn, TOut>, TOut> handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <inheritdoc/>
        public TOut Run(TIn parameter, Func<TIn, TOut> next) => _handler(parameter, next);
    }
}

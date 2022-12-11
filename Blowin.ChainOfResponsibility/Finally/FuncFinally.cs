using System;

namespace Blowin.ChainOfResponsibility.Finally
{
    /// <inheritdoc />
    public sealed class FuncFinally<TIn, TOut> : IFinally<TIn, TOut>
    {
        /// <summary>
        /// Stored delegate.
        /// </summary>
        internal readonly Func<TIn, TOut> Func;

        /// <summary>
        /// Initializes a new instance of the <see cref="FuncFinally{TIn, TOut}"/> class.
        /// </summary>
        /// <param name="func">Handler as delegate.</param>
        public FuncFinally(Func<TIn, TOut> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        /// <inheritdoc/>
        public TOut Run(TIn parameter) => Func(parameter);
    }
}

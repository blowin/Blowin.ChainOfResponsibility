using System;

namespace Blowin.ChainOfResponsibility.Finally
{
    public sealed class FuncFinally<TIn, TOut> : IFinally<TIn, TOut>
    {
        private readonly Func<TIn, TOut> _func;

        public FuncFinally(Func<TIn, TOut> func)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public TOut Run(TIn parameter) => _func(parameter);
    }
}

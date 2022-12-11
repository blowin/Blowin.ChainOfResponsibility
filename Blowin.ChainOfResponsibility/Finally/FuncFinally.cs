using System;

namespace Blowin.ChainOfResponsibility.Finally
{
    public sealed class FuncFinally<TIn, TOut> : IFinally<TIn, TOut>
    {
        internal readonly Func<TIn, TOut> Func;

        public FuncFinally(Func<TIn, TOut> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public TOut Run(TIn parameter) => Func(parameter);
    }
}

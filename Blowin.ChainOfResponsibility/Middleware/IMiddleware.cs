using System;

namespace Blowin.ChainOfResponsibility.Middleware
{
    public interface IMiddleware<TIn, TOut>
    {
        TOut Run(TIn parameter, Func<TIn, TOut> next);
    }
}
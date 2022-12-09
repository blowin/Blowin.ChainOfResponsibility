namespace Blowin.ChainOfResponsibility.Finally
{
    public interface IFinally<in TIn, out TOut>
    {
        TOut Run(TIn parameter);
    }
}

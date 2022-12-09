using System;

namespace Blowin.ChainOfResponsibility
{
    internal static class ServiceProviderExt
    {
        public static TService GetService<TService>(this IServiceProvider self)
            where TService : class
        {
            return self.GetService(typeof(TService)) as TService;
        }
    }
}
using Slavapp.Extensions.EF.Filtering;
using Slavapp.Extensions.EF.Sorting;
using System;
using System.Collections.Generic;

namespace Slavapp.Extensions.EF
{
    internal static class FactoryCache
    {
        private static Dictionary<Type, Func<IBaseSorting>> sortCache = new Dictionary<Type, Func<IBaseSorting>>();
        private static Dictionary<Type, Func<IBaseFilter>> filterCache = new Dictionary<Type, Func<IBaseFilter>>();

        internal static void RegisterSort<TModel, TSort>(this IServiceProvider serviceProvider) where TSort : BaseSorting<TModel>
        {
            sortCache.Add(typeof(TModel), () => (IBaseSorting)serviceProvider.GetService(typeof(TSort)));
        }

        internal static void RegisterFilter<TModel, TFilter>(this IServiceProvider serviceProvider) where TFilter : BaseFilter<TModel>
        {
            filterCache.Add(typeof(TModel), () => (IBaseFilter)serviceProvider.GetService(typeof(TFilter)));
        }

        internal static IBaseSorting GetSort<TModel>()
        {
            return sortCache[typeof(TModel)]();
        }

        internal static IBaseFilter GetFilter<TModel>()
        {
            return filterCache[typeof(TModel)]();
        }
    }
}

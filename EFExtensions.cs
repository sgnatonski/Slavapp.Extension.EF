using Slavapp.Extensions.EF.Filtering;
using Slavapp.Extensions.EF.Sorting;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Slavapp.Extensions.EF
{
    public static class EFExtensions
    {
        public static void RegisterSort<TModel, TSort>(this IServiceProvider serviceProvider) where TSort : BaseSorting<TModel>
        {
            FactoryCache.RegisterSort<TModel, TSort>(serviceProvider);
        }

        public static void RegisterFilter<TModel, TFilter>(this IServiceProvider serviceProvider) where TFilter : BaseFilter<TModel>
        {
            FactoryCache.RegisterFilter<TModel, TFilter>(serviceProvider);
        }

        public static IQueryable<T> WithSort<T>(this IQueryable<T> source, string sort)
        {
            return FactoryCache.GetSort<T>().Sort(source, sort);
        }

        public static IQueryable<T> WithFilter<T>(this IQueryable<T> source, string filter)
        {
            return FactoryCache.GetFilter<T>().Filter(source, filter);
        }

        public static IQueryable<T> WithPaging<T>(this IQueryable<T> source, int start, int count)
        {
            var query = start > 0 ? source.Skip(start) : source;
            return count > 0 ? query.Take(count) : query;
        }
    }
}

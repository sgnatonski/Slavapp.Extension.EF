using Slavapp.Extensions.EF.Filtering;
using Slavapp.Extensions.EF.Sorting;
using System;
using System.Linq;

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

        public static IQueryable<T> WithPaging<T>(this IQueryable<T> source, Func<dynamic> param) where T : class
        {
            var input = param();
            var start = input.start != null ? int.Parse((string)input.start) : (int?)null;
            var count = input.count != null ? int.Parse((string)input.count) : (int?)null;
            var query = start.HasValue ? source.Skip(start.Value) : source;
            return count.HasValue ? query.Take(count.Value) : query;
        }
    }
}

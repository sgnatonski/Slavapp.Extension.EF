using System.Linq;
namespace Slavapp.Extensions.EF.Sorting
{
    internal interface IBaseSorting
    {
        IQueryable<TModel> Sort<TModel>(IQueryable<TModel> set, string sort);
    }
}

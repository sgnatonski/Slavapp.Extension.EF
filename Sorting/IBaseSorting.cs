using System.Linq;
namespace Slavapp.Extensions.EF.Sorting
{
    public interface IBaseSorting
    {
        IQueryable<TModel> Sort<TModel>(IQueryable<TModel> set, string sort);
    }
}

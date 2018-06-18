using System.Linq;
namespace Slavapp.Extensions.EF.Filtering
{
    internal interface IBaseFilter
    {
        IQueryable<TModel> Filter<TModel>(IQueryable<TModel> set, string sort);
    }
}

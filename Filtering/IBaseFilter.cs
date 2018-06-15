using System.Linq;
namespace Slavapp.Extensions.EF.Filtering
{
    public interface IBaseFilter
    {
        IQueryable<TModel> Filter<TModel>(IQueryable<TModel> set, string sort);
    }
}

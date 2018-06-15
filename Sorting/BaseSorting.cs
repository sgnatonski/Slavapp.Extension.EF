using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Slavapp.Extensions.EF.Sorting
{
    public abstract class BaseSorting<TModel> : IBaseSorting
    {
        private Hashtable expressions = new Hashtable();
        private ISymbols symbols = new DefaultSymbols();

        private IEnumerable<SortCommand> ParseCommands(string sortString)
        {
            var cols = sortString.Split(new[] { this.symbols.ColumnSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var col in cols)
            {
                var sort = col.Split(new[] { this.symbols.ValueSeparator }, StringSplitOptions.None);
                yield return new SortCommand() { Column = sort[0], Ascending = sort[1] == this.symbols.SortAscending };
            }
        }

        private IOrderedQueryable<TModel> Order<TKey>(IQueryable<TModel> source, Expression<Func<TModel, TKey>> selector, bool ascending)
        {
            if (ascending)
            {
                return source.OrderBy(selector);
            }
            else
            {
                return source.OrderByDescending(selector);
            }
        }

        public IQueryable<T> Sort<T>(IQueryable<T> set, string sort)
        {
            var internalSet = set.Cast<TModel>();
            foreach (var command in ParseCommands(sort))
            {
                var expr = expressions[command.Column];                
                if (expr != null)
                {
                    internalSet = Order(internalSet, (dynamic)expr, command.Ascending);
                }
            }
            return internalSet.Cast<T>();
        }

        protected void SetSorting<TReturn>(string name, Expression<Func<TModel, TReturn>> property)
        {
            expressions.Add(name, property);
        }

        protected void SetSymbols(ISymbols symbols)
        {
            this.symbols = symbols ?? throw new ArgumentNullException(nameof(symbols));
        }
    }
}

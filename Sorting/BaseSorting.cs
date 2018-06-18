using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Slavapp.Extensions.EF.Sorting
{
    public abstract class BaseSorting<TModel> : IBaseSorting
    {
        private Hashtable expressions = new Hashtable();
        private Hashtable complexExpressions = new Hashtable();
        private ISymbols symbols = new DefaultSymbols();

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

        protected void SetSorting<TReturn>(string name, Expression<Func<TModel, TReturn>> propertyExpression)
        {
            expressions.Add(name, propertyExpression);
        }

        protected void SetComplexSorting<TReturn>(string name, Func<string, TReturn> valueExtractor, Func<TReturn, Expression<Func<TModel, bool>>> propertyExpression)
        {
            complexExpressions.Add(name, Tuple.Create(valueExtractor, propertyExpression));
        }

        protected void SetSymbols(ISymbols symbols)
        {
            this.symbols = symbols ?? throw new ArgumentNullException(nameof(symbols));
        }

        public IQueryable<T> Sort<T>(IQueryable<T> set, string sort)
        {
            var internalSet = set.Cast<TModel>();
            var sorted = false;
            foreach (var command in SortingParser.ParseCommands(sort, this.symbols))
            {
                var expr = expressions[command.Column];
                if (expr != null)
                {
                    internalSet = Order(internalSet, (dynamic)expr, command.Ascending);
                    sorted = true;
                }
                else
                {
                    foreach(var key in complexExpressions.Keys)
                    {
                        if (command.Column.StartsWith(key.ToString()))
                        {
                            var func = (dynamic)complexExpressions[key];
                            var val = func.Item1(command.Column);
                            var expr2 = func.Item2(val);
                            if (expr2 != null)
                            {
                                internalSet = Order(internalSet, expr2, command.Ascending);
                                sorted = true;
                                break;
                            }
                        }                        
                    }
                }
            }
            if (!sorted)
            {
                internalSet = Order(internalSet, x => x != null, true);
            }
            return internalSet.Cast<T>();
        }
    }
}
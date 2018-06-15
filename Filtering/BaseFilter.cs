using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Slavapp.Extensions.EF.Filtering
{
    public abstract class BaseFilter<TModel> : IBaseFilter
    {
        private Hashtable expressions = new Hashtable();
        private ISymbols symbols = new DefaultSymbols();
        
        private IEnumerable<FilterCommand> ParseCommands(string filterString)
        {
            if (string.IsNullOrEmpty(filterString) || filterString == "null")
            {
                yield break;
            }
            var cols = filterString.Split(new[] { this.symbols.ColumnSeparator }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var col in cols)
            {
                var sort = col.Split(new[] { this.symbols.ValueSeparator }, StringSplitOptions.None);
                yield return new FilterCommand()
                {
                    Column = sort[0],
                    Filter = sort[1].Substring(1),
                    Operation = GetOperation(sort[1][0].ToString())
                };
            }
        }

        private FilterOperation GetOperation(string o)
        {
            if (this.symbols.StartsWith == o) return FilterOperation.StartsWith;
            if (this.symbols.Equal == o) return FilterOperation.Equal;
            if (this.symbols.GreaterThan == o) return FilterOperation.GreaterThan;
            if (this.symbols.LesserThan == o) return FilterOperation.LesserThan;
            if (this.symbols.NotEqual == o) return FilterOperation.NotEqual;
            if (this.symbols.Contains == o) return FilterOperation.Contains;
            return FilterOperation.StartsWith;
        }

        private Expression<Func<T, bool>> GetPredicate<T, TType>(Expression<Func<T, TType>> expr, FilterCommand command)
        {
            var argParam = expr.Parameters[0];
            var val1 = Expression.Constant(Convert.ChangeType(command.Filter, typeof(TType)));

            Expression e1 = null;
            switch (command.Operation)
            {
               case FilterOperation.Equal:
                    e1 = Expression.Equal(expr.Body, val1); break;
                case FilterOperation.NotEqual:
                    e1 = Expression.NotEqual(expr.Body, val1); break;
                case FilterOperation.GreaterThan:
                    e1 = Expression.GreaterThan(expr.Body, val1); break;
                case FilterOperation.LesserThan:
                    e1 = Expression.LessThan(expr.Body, val1); break;
                case FilterOperation.StartsWith:
                    e1 = Expression.Call(expr.Body, typeof(TType).GetMethod("ToString", new Type[] { }));
                    e1 = Expression.Call(e1, typeof(string).GetMethod("ToLower", new Type[] { }));
                    e1 = Expression.Call(e1, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), Expression.Constant(command.Filter.ToLower()));
                    break;
                case FilterOperation.Contains:
                    e1 = Expression.Call(expr.Body, typeof(TType).GetMethod("ToString", new Type[] { }));
                    e1 = Expression.Call(e1, typeof(string).GetMethod("ToLower", new Type[] { }));
                    e1 = Expression.Call(e1, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), Expression.Constant(command.Filter.ToLower()));
                    break;
            }

            var lambda = Expression.Lambda<Func<T, bool>>(e1, argParam);
            return lambda;
        }

        private IQueryable<TModel> ApplyOperation<TType>(IQueryable<TModel> set, Expression<Func<TModel, TType>> expr, FilterCommand command)
        {
            return set.Where(GetPredicate(expr, command));
        }

        public IQueryable<T> Filter<T>(IQueryable<T> set, string filter)
        {
            var internalSet = set.Cast<TModel>();
            foreach (var command in this.ParseCommands(filter))
            {
                var expr = expressions[command.Column];
                if (expr != null)
                {
                    internalSet = ApplyOperation(internalSet, (dynamic)expressions[command.Column], command);
                }
            }
            return internalSet.Cast<T>();
        }

        protected void SetFilter<TReturn>(string name, Expression<Func<TModel, TReturn>> property)
        {
            expressions.Add(name, property);
        }

        protected void SetSymbols(ISymbols symbols)
        {
            this.symbols = symbols ?? throw new ArgumentNullException(nameof(symbols));
        }
    }
}

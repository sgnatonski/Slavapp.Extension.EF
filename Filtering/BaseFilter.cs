using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Slavapp.Extensions.EF.Filtering
{
    public abstract class BaseFilter<TModel> : IBaseFilter
    {
        private Hashtable expressions = new Hashtable();
        private ISymbols symbols = new DefaultSymbols();

        private Expression GetConvertedParam<T>(string val)
        {
            return Expression.Constant((T)Convert.ChangeType(val, typeof(T)));
        }

        private Expression<Func<T, bool>> GetPredicate<T, TType>(Expression<Func<T, TType>> expr, FilterCommand command)
        {
            Expression expression = null;
            try
            {
                switch (command.Operation)
                {
                    case FilterOperation.Equal:
                        expression = Expression.Equal(expr.Body, GetConvertedParam<TType>(command.Filter));
                        break;
                    case FilterOperation.NotEqual:
                        expression = Expression.NotEqual(expr.Body, GetConvertedParam<TType>(command.Filter));
                        break;
                    case FilterOperation.GreaterThan:
                        expression = Expression.GreaterThan(expr.Body, GetConvertedParam<TType>(command.Filter));
                        break;
                    case FilterOperation.LesserThan:
                        expression = Expression.LessThan(expr.Body, GetConvertedParam<TType>(command.Filter));
                        break;
                    case FilterOperation.StartsWith:
                        expression = Expression.Call(expr.Body, typeof(TType).GetMethod("ToString", new Type[] { }));
                        expression = Expression.Call(expression, typeof(string).GetMethod("ToLower", new Type[] { }));
                        expression = Expression.Call(expression, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), Expression.Constant(command.Filter.ToLower()));
                        break;
                    case FilterOperation.Contains:
                        expression = Expression.Call(expr.Body, typeof(TType).GetMethod("ToString", new Type[] { }));
                        expression = Expression.Call(expression, typeof(string).GetMethod("ToLower", new Type[] { }));
                        expression = Expression.Call(expression, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), Expression.Constant(command.Filter.ToLower()));
                        break;
                }
            }
            catch (FormatException)
            {
                expression = Expression.Call(expr.Body, typeof(TType).GetMethod("ToString", new Type[] { }));
                expression = Expression.Call(expression, typeof(string).GetMethod("ToLower", new Type[] { }));
                expression = Expression.Call(expression, typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), Expression.Constant(command.Filter.ToLower()));
            }
            
            var argParam = expr.Parameters[0];
            var lambda = Expression.Lambda<Func<T, bool>>(expression, argParam);
            return lambda;
        }

        private IQueryable<TModel> ApplyOperation<TType>(IQueryable<TModel> set, Expression<Func<TModel, TType>> expr, FilterCommand command)
        {
            return set.Where(GetPredicate(expr, command));
        }

        protected void SetFilter<TReturn>(string name, Expression<Func<TModel, TReturn>> property)
        {
            expressions.Add(name, property);
        }

        protected void SetSymbols(ISymbols symbols)
        {
            this.symbols = symbols ?? throw new ArgumentNullException(nameof(symbols));
        }

        public IQueryable<T> Filter<T>(IQueryable<T> set, string filter)
        {
            var internalSet = set.Cast<TModel>();
            foreach (var command in FilterParser.ParseCommands(filter, this.symbols))
            {
                var expr = expressions[command.Column];
                if (expr != null)
                {
                    internalSet = ApplyOperation(internalSet, (dynamic)expressions[command.Column], command);
                }
            }
            return internalSet.Cast<T>();
        }
    }
}

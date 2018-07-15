using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Slavapp.Extensions.EF.Filtering
{
    internal class FilterParser
    {
        private static ConcurrentDictionary<int, List<FilterCommand>> commandCache = new ConcurrentDictionary<int, List<FilterCommand>>();

        private static FilterOperation GetOperation(string o, ISymbols symbols)
        {
            if (symbols.StartsWith == o) return FilterOperation.StartsWith;
            if (symbols.Equal == o) return FilterOperation.Equal;
            if (symbols.GreaterThan == o) return FilterOperation.GreaterThan;
            if (symbols.LesserThan == o) return FilterOperation.LesserThan;
            if (symbols.NotEqual == o) return FilterOperation.NotEqual;
            if (symbols.Contains == o) return FilterOperation.Contains;
            return FilterOperation.StartsWith;
        }

        internal static IEnumerable<FilterCommand> ParseCommands(string filterString, ISymbols symbols)
        {
            if (string.IsNullOrEmpty(filterString))
            {
                yield break;
            }
            if (commandCache.TryGetValue(filterString.GetHashCode(), out var cached))
            {
                foreach (var command in cached)
                {
                    yield return command;
                }
            }
            else
            {
                var cols = filterString.Split(new[] { symbols.ColumnSeparator }, StringSplitOptions.RemoveEmptyEntries);
                var list = new List<FilterCommand>();
                foreach (var col in cols)
                {
                    var filter = col.Split(new[] { symbols.ValueSeparator }, StringSplitOptions.None);
                    if (filter.Length == 2)
                    {
                        var command = new FilterCommand()
                        {
                            Column = filter[0],
                            Filter = filter[1].Substring(1),
                            Operation = GetOperation(filter[1][0].ToString(), symbols)
                        };
                        list.Add(command);
                        yield return command;
                    }
                }
                commandCache.TryAdd(filterString.GetHashCode(), list);
            }
        }
    }
}

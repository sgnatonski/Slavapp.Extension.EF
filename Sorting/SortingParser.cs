using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Slavapp.Extensions.EF.Sorting
{
    internal class SortingParser
    {
        private static ConcurrentDictionary<int, List<SortCommand>> commandCache = new ConcurrentDictionary<int, List<SortCommand>>();

        internal static IEnumerable<SortCommand> ParseCommands(string sortString, ISymbols symbols)
        {
            if (string.IsNullOrEmpty(sortString))
            {
                yield break;
            }
            if (commandCache.TryGetValue(sortString.GetHashCode(), out var cached))
            {
                foreach (var command in cached)
                {
                    yield return command;
                }
            }
            else
            {
                var cols = sortString.Split(new[] { symbols.ColumnSeparator }, StringSplitOptions.RemoveEmptyEntries);
                var list = new List<SortCommand>();
                foreach (var col in cols)
                {
                    var sort = col.Split(new[] { symbols.ValueSeparator }, StringSplitOptions.None);
                    if (sort.Length == 2)
                    {
                        var command = new SortCommand() { Column = sort[0], Ascending = sort[1] == symbols.SortAscending };                        
                        list.Add(command);
                        yield return command;
                    }
                }
                commandCache.TryAdd(sortString.GetHashCode(), list);
            }
        }
    }
}

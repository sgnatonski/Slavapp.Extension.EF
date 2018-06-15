namespace Slavapp.Extensions.EF
{
    internal class DefaultSymbols : ISymbols
    {
        public string StartsWith { get; } = "^";
        public string Equal { get; } = "*";
        public string GreaterThan { get; } = ">";
        public string LesserThan { get; } = "<";
        public string NotEqual { get; } = "!";
        public string Contains { get; } = "~";
        public string SortAscending { get; } = "asc";
        public string ColumnSeparator { get; } = "|";
        public string ValueSeparator { get; } = ":";
    }
}

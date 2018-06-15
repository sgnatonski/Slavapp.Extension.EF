namespace Slavapp.Extensions.EF
{
    public interface ISymbols
    {
        string Contains { get; }
        string Equal { get; }
        string GreaterThan { get; }
        string LesserThan { get; }
        string NotEqual { get; }
        string StartsWith { get; }
        string SortAscending { get; }
        string ColumnSeparator { get; }
        string ValueSeparator { get; }
    }
}
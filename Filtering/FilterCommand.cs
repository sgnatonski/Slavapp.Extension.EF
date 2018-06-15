namespace Slavapp.Extensions.EF.Filtering
{
    internal class FilterCommand
    {
        public string Column { get; set; }
        public string Filter { get; set; }
        public FilterOperation Operation { get; set; }
    }

    public enum FilterOperation 
    {
        StartsWith,
        Equal,
        GreaterThan,
        LesserThan,
        NotEqual,
        Contains,
    }
}
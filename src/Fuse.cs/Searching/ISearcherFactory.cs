namespace FuseCs.Searching
{
    public interface ISearcherFactory
    {
        ISearcher Create(string pattern, FuseOptions options);
    }
}
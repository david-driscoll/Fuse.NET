using FuseCs.Matching;

namespace FuseCs.Searching
{
    public class SearcherFactory : ISearcherFactory
    {
        public ISearcher Create(string pattern, FuseOptions options)
        {
            return new BitapSearcher(pattern, new MatcherOptions()
            {
                Threshold = options.Threshold,
                IgnoreCase = options.IgnoreCase,
                Distance = options.Distance,
                MaxLength= options.MaxLength
            });
        }
    }
}
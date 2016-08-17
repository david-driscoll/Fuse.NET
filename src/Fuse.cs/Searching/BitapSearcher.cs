using System;
using FuseCs.Matching;

namespace FuseCs.Searching
{
    public class BitapSearcher : ISearcher
    {
        private readonly Matcher _matcher;

        public BitapSearcher(string pattern, MatcherOptions options)
        {
            _matcher = new Matcher(pattern, options);
        }

        public SearcherResult Search(string word)
        {
            var match = _matcher.Search(word);
            return new SearcherResult(match.IsMatch, match.Score, match.MatchedIndices);
        }
    }
}

using System.Collections.Generic;
using FuseCs.Matching;

namespace FuseCs.Searching
{
    public class SearcherResult
    {
        internal SearcherResult(bool isMatch, double score, IEnumerable<MatchedIndex> matchedIndices)
        {
            IsMatch = isMatch;
            Score = score;
            MatchedIndices = matchedIndices;
        }
        public bool IsMatch { get; }
        public double Score { get; }
        public IEnumerable<MatchedIndex> MatchedIndices { get; }
    }
}
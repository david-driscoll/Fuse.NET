using System.Collections.Generic;

namespace FuseCs.Matching
{
    public class Match
    {
        internal Match(bool isMatch, double score, IEnumerable<MatchedIndex> matchedIndices)
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
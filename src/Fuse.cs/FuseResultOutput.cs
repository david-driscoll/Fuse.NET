using System.Collections.Generic;
using FuseCs.Matching;

namespace FuseCs
{
    public class FuseResultOutput<T>
    {
        internal FuseResultOutput(FuseKey<T> key, double score, IEnumerable<MatchedIndex> matchedIndices)
        {
            Key = key;
            Score = score;
            MatchedIndices = matchedIndices;
        }

        public FuseKey<T> Key { get; }
        public string Name => Key.Name;
        public double Score { get; }
        public IEnumerable<MatchedIndex> MatchedIndices { get; }
    }
}
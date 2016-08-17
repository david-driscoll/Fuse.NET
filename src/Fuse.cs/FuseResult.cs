using System.Collections.Generic;
using System.Linq;
using FuseCs.Matching;

namespace FuseCs
{
    public class FuseResult<T>
    {
        internal FuseResult(T item, ICollection<FuseResultOutput<T>> output)
        {
            Item = item;
            Output = output;
        }

        public T Item { get; }
        internal ICollection<FuseResultOutput<T>> Output { get; }
        public double Score { get; internal set; }

        private IDictionary<string, IEnumerable<MatchedIndex>> _matches;
        public IDictionary<string, IEnumerable<MatchedIndex>> Matches
        {
            get
            {
                if (this._matches == null)
                {
                    _matches = Output.ToDictionary(x => x.Name, x => x.MatchedIndices);
                }
                return _matches;
            }
        }
    }
}
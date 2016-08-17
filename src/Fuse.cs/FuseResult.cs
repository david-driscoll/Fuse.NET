using System.Collections.Generic;

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
        public ICollection<FuseResultOutput<T>> Output { get; }
        public double Score { get; internal set; }
    }
}
using System.Collections.Generic;
using System.Linq;
using FuseCs.Searching;

namespace FuseCs
{
    public static class Fuse
    {
        public static Fuse<T> Create<T>() => new Fuse<T>(new FuseOptions<T>());
        public static Fuse<T> Create<T>(FuseOptions<T> options) => new Fuse<T>(options);
    }

    public class Fuse<T>
    {
        private readonly FuseOptions<T> _options;

        public Fuse(FuseOptions<T> options)
        {
            _options = options;
        }

        /**
         * Sets a new list for Fuse to match against.
         * @param {Array} list
         * @return {Array} The newly set list
         * @public
         */
        public FuseSearch<T> Search(IEnumerable<T> items, string pattern)
        {
            var options = _options;
            var tokens = _options.TokenSeparator.Split(pattern);

            var tokenSearchers = Enumerable.Empty<ISearcher>();
            if (_options.Tokenize)
            {
                tokenSearchers = tokens.Select(token => _options.Factory.Create(token, _options)).ToArray();
            }

            var fullSeacher = _options.Factory.Create(pattern, options);

            return new FuseSearch<T>(items, fullSeacher, tokenSearchers, _options);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FuseCs.Searching;

namespace FuseCs
{
    public class FuseSearch<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _items;
        private readonly ISearcher _fullSeacher;
        private readonly IEnumerable<ISearcher> _tokenSearchers;
        private readonly FuseOptions<T> _options;
        private readonly IDictionary<int, FuseResult<T>> _cache = new Dictionary<int, FuseResult<T>>();
        private IEnumerable<FuseResult<T>> _results;

        internal FuseSearch(IEnumerable<T> items, ISearcher fullSeacher, IEnumerable<ISearcher> tokenSearchers,
            FuseOptions<T> options)
        {
            _items = items;
            _fullSeacher = fullSeacher;
            _tokenSearchers = tokenSearchers;
            _options = options;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Results.Select(x => x.Item).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<FuseResult<T>> Results
        {
            get
            {
                if (_results != null) return _results;
                var index = 0;
                foreach (var item in _items)
                {
                    foreach (var key in _options.Keys)
                    {
                        Analyze(key, key.LookupFunc(item), item, index);
                    }
                    FuseResult<T> result;
                    if (_cache.TryGetValue(index, out result))
                    {
                        PrepareScore(result);
                    }
                    index += 1;
                }

                return _results = _cache.Values
                    .OrderByDescending(x => x.Score);
            }
        }

        public void Analyze(FuseKey<T> key, string[] values, T entity, int index)
        {
            if (values == null || values.Length == 0)
            {
                return;
            }

            foreach (var text in values)
            {
                double? averageScore = null;
                var exists = false;

                if (_options.Tokenize)
                {
                    var words = _options.TokenSeparator.Split(text);
                    var scores = new List<double>();
                    foreach (var tokenSearcher in _tokenSearchers)
                    {
                        foreach (var word in words)
                        {
                            var result = tokenSearcher.Search(word);
                            if (result.IsMatch)
                            {
                                scores.Add(result.Score);
                                exists = true;
                            }
                            else
                            {
                                scores.Add(1);
                            }
                        }
                    }

                    averageScore = scores.Average();
                }

                var fullResult = _fullSeacher.Search(text);
                var finalScore = fullResult.Score;
                if (averageScore.HasValue)
                {
                    finalScore = (finalScore + averageScore.Value) / 2;
                }

                if (exists || fullResult.IsMatch)
                {
                    FuseResult<T> result;
                    if (!_cache.TryGetValue(index, out result))
                    {
                        result = new FuseResult<T>(entity, new List<FuseResultOutput<T>>());
                        _cache.Add(index, result);
                    }
                    result.Output.Add(new FuseResultOutput<T>(key, finalScore, fullResult.MatchedIndices));
                }
            }
        }

        private static void PrepareScore(FuseResult<T> result)
        {
            double totalScore = 0;
            double bestScore = 1;
            foreach (var output in result.Output)
            {
                var weight = output.Key.Weight;
                var nScore = output.Score * weight;
                if (Math.Abs(weight - 1) > 0.0001)
                {
                    bestScore = Math.Min(bestScore, nScore);
                }
                else
                {
                    totalScore += nScore;
                    // output.NScore = nScore;
                }
            }
            if (Math.Abs(bestScore - 1) > 0.0001)
            {
                result.Score = totalScore / result.Output.Count;
            }
            else
            {
                result.Score = bestScore;
            }
        }
    }
}
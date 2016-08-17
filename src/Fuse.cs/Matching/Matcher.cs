using System;
using System.Collections.Generic;
using System.Linq;

namespace FuseCs.Matching
{
    public class Matcher
    {
        private readonly string _pattern;
        private readonly Dictionary<char, int> _matchAlphabet;
        private readonly int _location;
        private readonly MatcherOptions _matcherOptions;
        private readonly int _matchmask;

        public Matcher(string pattern) : this(pattern, 0, new MatcherOptions()) { }
        public Matcher(string pattern, MatcherOptions matcherOptions) : this(pattern, 0, matcherOptions) { }
        public Matcher(string pattern, int location) : this(pattern, location, new MatcherOptions()) { }
        public Matcher(string pattern, int location, MatcherOptions matcherOptions)
        {
            if (matcherOptions.IgnoreCase)
            {
                _pattern = pattern.ToLowerInvariant();
            }
            else
            {
                _pattern = pattern;
            }

            _location = location;
            _matcherOptions = matcherOptions;
            _matchAlphabet = MatchAlphabet(_pattern);
            _matchmask = 1 << (_pattern.Length - 1);
        }

        /**
         * Locate the best instance of 'pattern' in 'text' near 'loc'.
         * Returns -1 if no match found.
         * @param text The text to search.
         * @param pattern The pattern to search for.
         * @param loc The location to search around.
         * @return Best match index or -1.
         */
        public Match Search(string text)
        {
            if (this._matcherOptions.IgnoreCase)
            {
                text = text.ToLowerInvariant();
            }

            var location = Math.Max(0, Math.Min(_location, text.Length));
            if (text == _pattern)
            {
                // Shortcut (potentially not guaranteed by the algorithm)
                return new Match(true, 0d, new[] { new MatchedIndex(0, text.Length - 1) });
            }

            // Highest score beyond which we give up.
            var scoreThreshold = _matcherOptions.Threshold;
            // Is there a nearby exact match? (speedup)
            var bestLoc = text.IndexOf(_pattern, _location, StringComparison.OrdinalIgnoreCase);
            if (bestLoc != -1)
            {
                scoreThreshold = Math.Min(MatchBitapScore(0, bestLoc), scoreThreshold);
                // What about in the other direction? (speedup)
                bestLoc = text.LastIndexOf(_pattern,
                    Math.Min(_location + _pattern.Length, text.Length),
                    StringComparison.OrdinalIgnoreCase);
                if (bestLoc != -1)
                {
                    scoreThreshold = Math.Min(MatchBitapScore(0, bestLoc), scoreThreshold);
                }
            }


            // a mask of the matches
            var matchMask = new bool[text.Length];

            // Initialise the bit arrays.
            bestLoc = -1;

            var binMax = _pattern.Length + text.Length;
            // Empty initialization added to appease C# compiler.
            var lastRd = new int[0];
            var score = 1.0d;

            for (var i = 0; i < _pattern.Length; i++)
            {
                // Scan for the best match; each iteration allows for one more error.
                // Run a binary search to determine how far from 'loc' we can stray at
                // this error level.
                var binMin = 0;
                var binMid = binMax;
                while (binMin < binMid)
                {
                    if (MatchBitapScore(i, _location + binMid)
                        <= scoreThreshold)
                    {
                        binMin = binMid;
                    }
                    else
                    {
                        binMax = binMid;
                    }
                    binMid = (binMax - binMin) / 2 + binMin;
                }
                // Use the result from this iteration as the maximum for the next.
                binMax = binMid;
                var start = Math.Max(1, _location - binMid + 1);
                var finish = Math.Min(_location + binMid, text.Length) + _pattern.Length;

                var bitArr = new int[finish + 2];
                bitArr [finish + 1] = (1 << i) - 1;
                for (var j = finish; j >= start; j--)
                {
                    int charMatch;
                    if (text.Length <= j - 1 || !_matchAlphabet.ContainsKey(text[j - 1]))
                    {
                        // Out of range.
                        charMatch = 0;
                    }
                    else
                    {
                        charMatch = _matchAlphabet[text[j - 1]];
                        matchMask[j - 1] = true;
                    }
                    if (i == 0)
                    {
                        // First pass: exact match.
                        bitArr [j] = ((bitArr [j + 1] << 1) | 1) & charMatch;
                    }
                    else
                    {
                        // Subsequent passes: fuzzy match.
                        bitArr [j] = (((bitArr [j + 1] << 1) | 1) & charMatch)
                                | (((lastRd[j + 1] | lastRd[j]) << 1) | 1) | (lastRd[j + 1]);
                    }
                    if ((bitArr [j] & _matchmask) != 0)
                    {
                        score = MatchBitapScore(i, j - 1);
                        // This match will almost certainly be better than any existing
                        // match.  But check anyway.
                        if (score <= scoreThreshold)
                        {
                            // Told you so.
                            scoreThreshold = score;
                            bestLoc = j - 1;
                            if (bestLoc > _location)
                            {
                                // When passing loc, don't exceed our current distance from loc.
                                start = Math.Max(1, 2 * _location - bestLoc);
                            }
                            else
                            {
                                // Already passed loc, downhill from here on in.
                                break;
                            }
                        }
                    }
                }
                if (MatchBitapScore(i + 1, _location) > scoreThreshold)
                {
                    // No hope for a (better) match at greater error levels.
                    break;
                }
                lastRd = bitArr ;
            }

            return new Match(bestLoc >= 0, score, GetMatchedIndices(matchMask));
        }

        private static IEnumerable<MatchedIndex> GetMatchedIndices(IReadOnlyList<bool> matchMask)
        {
            var matchedIndices = new List<MatchedIndex>();
            var start = -1;
            var i = 0;
            // var match
            for (; i < matchMask.Count; i++)
            {
                var match = matchMask[i];
                if (match && start == -1)
                {
                    start = i;
                }
                else if (!match && start != -1)
                {
                    var end = i - 1;
                    matchedIndices.Add(new MatchedIndex(start, end));
                    start = -1;
                }
            }
            if (matchMask[i - 1])
            {
                matchedIndices.Add(new MatchedIndex(start, i - 1));
            }
            return matchedIndices;
        }


        /**
         * Compute and return the score for a match with e errors and x location.
         * @param e Number of errors in match.
         * @param x Location of match.
         * @param loc Expected location of match.
         * @param pattern Pattern being sought.
         * @return Overall score for match (0.0 = good, 1.0 = bad).
         */
        private double MatchBitapScore(int errors, int location)
        {
            var accuracy = (double)errors / _pattern.Length;
            var proximity = Math.Abs(_location - location);
            if (_matcherOptions.Distance == 0)
            {
                // Dodge divide by zero error.
                return proximity != 0 ? 1.0d : accuracy;
            }
            return accuracy + (proximity / (double)_matcherOptions.Distance);
        }

        /**
         * Initialise the alphabet for the Bitap algorithm.
         * @param pattern The text to encode.
         * @return Hash of character locations.
         */

        private static Dictionary<char, int> MatchAlphabet(string pattern)
        {
            var s = new Dictionary<char, int>();
            var charPattern = pattern.ToCharArray();
            foreach (var c in charPattern)
            {
                if (!s.ContainsKey(c))
                {
                    s.Add(c, 0);
                }
            }
            var i = 0;
            foreach (var c in charPattern)
            {
                var value = s[c] | (1 << (pattern.Length - i - 1));
                s[c] = value;
                i++;
            }
            return s;
        }
    }
}
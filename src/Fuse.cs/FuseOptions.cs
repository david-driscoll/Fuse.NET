using System.Collections.Generic;
using System.Text.RegularExpressions;
using FuseCs.Searching;

namespace FuseCs
{
    public abstract class FuseOptions
    {
        public string Id { get; set; } = null;
        public bool IgnoreCase { get; set; } = true;
        public ISearcherFactory Factory { get; set; } = new SearcherFactory();
        public bool Tokenize { get; set; } = false;
        public Regex TokenSeparator { get; set; } = new Regex(" +", RegexOptions.Multiline | RegexOptions.Compiled);
        public double Threshold { get; set; } = 0.6d;
        public int Distance { get; set; } = 100;
        public int MaxLength { get; set; } = 32;
    }

    public class FuseOptions<T> : FuseOptions
    {
        public IEnumerable<FuseKey<T>> Keys { get; set; }
    }
}
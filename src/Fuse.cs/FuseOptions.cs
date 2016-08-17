using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private IEnumerable<FuseKey<T>> _keys;

        public IEnumerable<FuseKey<T>> Keys
        {
            get
            {
                if (_keys != null) return _keys;
                var properties = typeof(T)
                    .GetTypeInfo()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var stringProperties = typeof(T)
                    .GetTypeInfo()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.PropertyType == typeof(string));
                var enumerableStringProperties = typeof(T)
                    .GetTypeInfo()
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => typeof(IEnumerable<string>).GetTypeInfo().IsAssignableFrom(x.PropertyType));

                return _keys = enumerableStringProperties
                    .Union(stringProperties)
                    .Select(x => FuseKey.Create(typeof(T), x))
                    .Cast<FuseKey<T>>()
                    .ToArray();
            }
            set { _keys = value; }
        }
    }
}
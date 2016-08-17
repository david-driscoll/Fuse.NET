using System;

namespace FuseCs
{
    public static class FuseKey
    {
        public static FuseKey<T> Create<T>(string name, Func<T, string> lookupFunc)
        {
            return new FuseKey<T>(name, (x) => new[] { lookupFunc(x) }, 1.0d);
        }
        public static FuseKey<T> Create<T>(string name, Func<T, string> lookupFunc, double weight)
        {
            return new FuseKey<T>(name, (x) => new[] { lookupFunc(x) }, weight);
        }
        public static FuseKey<T> Create<T>(string name, Func<T, string[]> lookupFunc)
        {
            return new FuseKey<T>(name, lookupFunc, 1.0d);
        }
        public static FuseKey<T> Create<T>(string name, Func<T, string[]> lookupFunc, double weight)
        {
            return new FuseKey<T>(name, lookupFunc, weight);
        }
    }
    public class FuseKey<T>
    {
        public FuseKey(string name, Func<T, string[]> lookupFunc, double weight)
        {
            Name = name;
            LookupFunc = lookupFunc;
            Weight = weight;
        }

        public string Name { get; }
        internal Func<T, string[]> LookupFunc { get; }
        public double Weight { get; }
    }
}
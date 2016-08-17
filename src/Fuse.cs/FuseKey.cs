using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
        internal static object Create(Type type, PropertyInfo property)
        {
            var funcType = typeof(Func<,>)
                .MakeGenericType(type, typeof(string[]));

            var param = Expression.Parameter(type, "x");
            if (property.PropertyType == typeof(string[]))
            {
                var prop = Expression.Property(param, property);
                var funcValue = Expression.Lambda(prop, param).Compile();
                var fuseType = typeof(FuseKey<>).MakeGenericType(type);
                return Activator.CreateInstance(fuseType, property.Name, funcValue, 1.0d);
            } else if (property.PropertyType == typeof(string))
            {
                var prop = Expression.NewArrayInit(typeof(string), Expression.Property(param, property));
                var funcValue = Expression.Lambda(prop, param).Compile();
                var fuseType = typeof(FuseKey<>).MakeGenericType(type);
                return Activator.CreateInstance(fuseType, property.Name, funcValue, 1.0d);
            }
            else if (typeof(IEnumerable<string>).GetTypeInfo().IsAssignableFrom(property.PropertyType))
            {
                var toArrayMethod = typeof(Enumerable).GetTypeInfo()
                    .GetMethod(nameof(Enumerable.ToArray), BindingFlags.Public | BindingFlags.Static)
                    .MakeGenericMethod(typeof(string));
                var prop = Expression.Property(param, property);
                var result = Expression.Call(toArrayMethod, prop);
                var funcValue = Expression.Lambda(result, param).Compile();
                var fuseType = typeof(FuseKey<>).MakeGenericType(type);
                return Activator.CreateInstance(fuseType, property.Name, funcValue, 1.0d);
            }

            throw new NotSupportedException(property.Name);
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
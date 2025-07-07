#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;

namespace Polymorphism4Unity.Editor
{
     public class Cache<TArg, TResult> : IReadOnlyCollection<TResult>
     {
          private readonly Dictionary<object, TResult> values = new();
          private readonly Func<TArg, TResult> factory;
          private readonly Func<TArg, object> keySelector;

          private static TArg Unit(TArg arg)
          {
               return arg;
          }

          public Cache(Func<TArg, TResult> factory) : this(factory, static arg => arg!)
          {

          }

          public Cache(Func<TArg, TResult> factory, Func<TArg, object> keySelector)
          {
               this.factory = factory;
               this.keySelector = keySelector;
          }

          public TResult this[TArg arg]
          {
               get
               {
                    object key = keySelector(arg);
                    if (values.TryGetValue(key, out TResult result))
                    {
                         return result;
                    }
                    result = factory(arg);
                    values[key] = result;
                    return result;
               }
          }

          public IEnumerable<TResult> Values => values.Values;
          public int Count => values.Count;

          public bool ContainsKey(TArg arg)
          {
               object key = keySelector(arg);
               return values.ContainsKey(key);
          }

          public bool TryGetValue(TArg arg, out TResult? value)
          {
               object key = keySelector(arg);
               return values.TryGetValue(key, out value);
          }

          public IEnumerator<TResult> GetEnumerator()
          {
               return values.Values.GetEnumerator();
          }

          IEnumerator IEnumerable.GetEnumerator()
          {
               return GetEnumerator();
          }
     }
}

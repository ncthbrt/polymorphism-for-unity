#nullable enable
using System;
using System.Collections.Generic;

namespace Polymorphism4Unity.Editor.Collections
{
     public class Cache<TArg, TResult>
     {
          private readonly Dictionary<object, TResult> values = new();
          private readonly Func<TArg, TResult> factory;
          private readonly Func<TArg, object> keySelector;

          public Cache(Func<TArg, TResult> factory) : this(factory, static arg => arg!)
          {
          }

          public Cache(Func<TArg, TResult> factory, Func<TArg, object> keySelector)
          {
               this.factory = factory;
               this.keySelector = keySelector;
          }

          public TResult this[TArg arg] => GetValue(arg);

          public TResult GetValue(TArg arg)
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

          public bool AlreadyContainsCachedValue(TArg arg)
          {
               object key = keySelector(arg);
               return values.ContainsKey(key);
          }

          public IEnumerable<TResult> Values => values.Values;
          public int Count => values.Count;

          public static implicit operator Func<TArg, TResult>(Cache<TArg, TResult> thisCache) => thisCache.GetValue;

          public void Clear()
          {
               values.Clear();
          }
     }
}

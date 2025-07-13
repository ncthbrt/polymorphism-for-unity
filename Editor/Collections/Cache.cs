#nullable enable
using System;
using System.Collections.Generic;

namespace Polymorphism4Unity.Editor.Collections
{
     public class Cache<TArg, TResult>
     {
          private readonly Dictionary<object, TResult> _values = new();
          private readonly Func<TArg, TResult> _factory;
          private readonly Func<TArg, object> _keySelector;

          public Cache(Func<TArg, TResult> factory) : this(factory, static arg => arg!)
          {
          }

          public Cache(Func<TArg, TResult> factory, Func<TArg, object> keySelector)
          {
               this._factory = factory;
               this._keySelector = keySelector;
          }

          public TResult this[TArg arg] => GetValue(arg);

          public TResult GetValue(TArg arg)
          {
               object key = _keySelector(arg);
               if (_values.TryGetValue(key, out TResult result))
               {
                    return result;
               }
               result = _factory(arg);
               _values[key] = result;
               return result;
          }

          public bool AlreadyContainsCachedValue(TArg arg)
          {
               object key = _keySelector(arg);
               return _values.ContainsKey(key);
          }

          public IEnumerable<TResult> Values => _values.Values;
          public int Count => _values.Count;

          public static implicit operator Func<TArg, TResult>(Cache<TArg, TResult> thisCache) => thisCache.GetValue;

          public void Clear()
          {
               _values.Clear();
          }
     }
}

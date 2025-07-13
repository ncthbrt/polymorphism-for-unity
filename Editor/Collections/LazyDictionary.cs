#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;

namespace Polymorphism4Unity.Editor.Collections
{
    internal interface ILazyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IDisposable
    {
    }

    internal class LazyDictionary<TInput, TKey, TValue> : ILazyDictionary<TKey, TValue>
    {
        private interface IEnumerationState : IEnumerable<IEnumerationState>, IDisposable
        {
            void IDisposable.Dispose() { }
        }

        private sealed class NotStartedEnumerationState : IEnumerationState
        {
            private readonly IEnumerable<TInput> _enumerable;
            private readonly Func<TInput, TKey> _keyMapper;
            private readonly Func<TInput, TValue> _valueMapper;

            public NotStartedEnumerationState(IEnumerable<TInput> enumerable, Func<TInput, TKey> keyMapper, Func<TInput, TValue> valueMapper)
            {
                this._enumerable = enumerable;
                this._keyMapper = keyMapper;
                this._valueMapper = valueMapper;
            }

            public IEnumerationState Start()
            {
                try
                {
                    IEnumerator<TInput> enumerator = _enumerable.GetEnumerator();
                    return new EnumeratingEnumerationState(enumerator, _keyMapper, _valueMapper);
                }
                catch (Exception e)
                {
                    return new ErrorEnumerationState(e);
                }
            }

            public IEnumerator<IEnumerationState> GetEnumerator()
            {
                try
                {
                    IEnumerator<TInput> enumerator = _enumerable.GetEnumerator();
                    return new EnumeratingEnumerationState(enumerator, _keyMapper, _valueMapper).GetEnumerator();
                }
                catch (Exception e)
                {
                    return EnumerableUtils.FromSingleItem<IEnumerationState>(new ErrorEnumerationState(e)).GetEnumerator();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class EnumeratingEnumerationState : IEnumerationState
        {
            private readonly IEnumerator<TInput> _enumerator;
            private readonly Func<TInput, TKey> _keyMapper;
            private readonly Func<TInput, TValue> _valueMapper;
            public Dictionary<TKey, TValue> Dictionary { get; } = new();
            private bool _disposed;

            public EnumeratingEnumerationState(IEnumerator<TInput> enumerator, Func<TInput, TKey> keyMapper, Func<TInput, TValue> valueMapper)
            {
                this._enumerator = enumerator;
                this._keyMapper = keyMapper;
                this._valueMapper = valueMapper;
            }

            public (IEnumerationState nextState, (TKey key, TValue value)? keyValue) TryPullNextElement()
            {
                try
                {
                    TInput element = _enumerator.Current;
                    TKey key = _keyMapper(element);
                    (TKey key, TValue value)? keyValue = null;
                    if (!Dictionary.ContainsKey(key))
                    {
                        TValue value = _valueMapper(element);
                        Dictionary[key] = value;
                        keyValue = (key, value);
                    }
                    if (_enumerator.MoveNext())
                    {
                        return (this, keyValue);
                    }
                    else
                    {
                        Dispose();
                        return (new CompletedEnumerationState(Dictionary), default);
                    }
                }
                catch (Exception e)
                {
                    Dispose();
                    return (new ErrorEnumerationState(e), null);
                }
            }


            public void Dispose()
            {
                if (!_disposed)
                {
                    try
                    {
                        _enumerator.Dispose();
                    }
                    finally
                    {
                        _disposed = true;
                    }
                }
            }


            public IEnumerator<IEnumerationState> GetEnumerator()
            {
                IEnumerationState currentState = this;
                while (currentState is EnumeratingEnumerationState enumerating)
                {
                    (currentState, _) = enumerating.TryPullNextElement();
                    yield return currentState;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class CompletedEnumerationState : IEnumerationState
        {
            public Dictionary<TKey, TValue> Dictionary { get; }

            public CompletedEnumerationState(Dictionary<TKey, TValue> values)
            {
                Dictionary = values;
            }

            public IEnumerator<IEnumerationState> GetEnumerator()
            {
                return Enumerable.Empty<IEnumerationState>().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class ErrorEnumerationState : IEnumerationState
        {
            public Exception Exception { get; }

            public ErrorEnumerationState(Exception exception)
            {
                Exception = exception;
            }

            public IEnumerator<IEnumerationState> GetEnumerator()
            {
                throw Exception;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private sealed class DisposedEnumerationState : IEnumerationState
        {
            public IEnumerator<IEnumerationState> GetEnumerator()
            {
                throw new ObjectDisposedException(nameof(LazyDictionary<TInput, TKey, TValue>));
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private IEnumerationState _state;

        public LazyDictionary(IEnumerable<TInput> enumerable, Func<TInput, TKey> keyMapper, Func<TInput, TValue> valueMapper)
        {
            _state = new NotStartedEnumerationState(enumerable, keyMapper, valueMapper);
        }

        public TValue this[TKey key]
        {
            get
            {
                EnsurePreconditions();
                if (_state is CompletedEnumerationState completed)
                {
                    return completed.Dictionary[key];
                }
                if (_state is EnumeratingEnumerationState enumerating)
                {
                    if (enumerating.Dictionary.TryGetValue(key, out TValue? value))
                    {
                        return value;
                    }
                    else
                    {
                        foreach ((TKey thisKey, TValue thisValue) in GetEnumerableFromEnumeratingState(enumerating))
                        {
                            if (Equals(thisKey, key))
                            {
                                return thisValue;
                            }
                        }
                    }
                }
                throw StateFailsPostCondition();
            }
        }

        public ICollection<TKey> Keys =>
            Force().Dictionary.Keys;

        public ICollection<TValue> Values =>
            Force().Dictionary.Values;

        public int Count => Force().Dictionary.Count;

        public bool IsReadOnly => true;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public void Clear()
        {
            _state.Dispose();
            _state = new CompletedEnumerationState(new Dictionary<TKey, TValue>());
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) =>
            TryGetValue(item.Key, out TValue? value) && Equals(item.Value, value);

        public bool ContainsKey(TKey key) =>
            TryGetValue(key, out _);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) =>
            ((ICollection<KeyValuePair<TKey, TValue>>)Force().Dictionary).CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            EnsurePreconditions();
            if (_state is CompletedEnumerationState completed)
            {
                return completed.Dictionary.GetEnumerator();
            }
            else if (_state is EnumeratingEnumerationState enumerating)
            {
                return GetEnumerableFromEnumeratingState(enumerating).GetEnumerator();
            }
            throw StateFailsPostCondition();
        }

        private IEnumerable<KeyValuePair<TKey, TValue>> GetEnumerableFromEnumeratingState(EnumeratingEnumerationState enumerating)
        {
            while (true)
            {
                (IEnumerationState nextState, (TKey key, TValue value)? maybeKeyValue) = enumerating.TryPullNextElement();
                _state = nextState;
                ThrowIfErrorOrDisposedState();
                if (maybeKeyValue is { } keyValue)
                {
                    yield return new KeyValuePair<TKey, TValue>(keyValue.key, keyValue.value);
                }
                if (_state is not EnumeratingEnumerationState nextEnumerating)
                {
                    yield break;
                }
                enumerating = nextEnumerating;
            }
        }

#pragma warning disable CS8767 // out value needs to be nullable (TValue?) for this contract to make sense
        public bool TryGetValue(TKey key, out TValue? value)
        {
            EnsurePreconditions();
            if (_state is CompletedEnumerationState completed)
            {
                return completed.Dictionary.TryGetValue(key, out value);
            }
            else if (_state is EnumeratingEnumerationState enumerating)
            {
                if (enumerating.Dictionary.TryGetValue(key, out value))
                {
                    return true;
                }
                else
                {
                    foreach ((TKey thisKey, TValue thisValue) in GetEnumerableFromEnumeratingState(enumerating))
                    {
                        if (Equals(thisKey, key))
                        {
                            value = thisValue;
                            return true;
                        }
                    }
                    return false;
                }
            }
            throw StateFailsPostCondition();
        }
#pragma warning restore CS8767

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            try
            {
                _state.Dispose();
            }
            finally
            {
                _state = new DisposedEnumerationState();
            }
        }

        private void StartEnumerationIfNecessary()
        {
            if (_state is NotStartedEnumerationState notStarted)
            {
                _state = notStarted.Start();
                ThrowIfErrorOrDisposedState();
            }
        }

        private void ThrowIfErrorOrDisposedState()
        {
            if (_state is ErrorEnumerationState error)
            {
                throw error.Exception;
            }
            if (_state is DisposedEnumerationState)
            {
                throw new ObjectDisposedException(nameof(LazyDictionary<TInput, TKey, TValue>));
            }
        }

        private void EnsurePreconditions()
        {
            ThrowIfErrorOrDisposedState();
            StartEnumerationIfNecessary();
        }

        public Exception StateFailsPostCondition()
        {
            throw Asserts.Fail($"{nameof(_state)} is {nameof(EnumeratingEnumerationState)} or ${nameof(CompletedEnumerationState)}");
        }

        private CompletedEnumerationState Force()
        {
            EnsurePreconditions();
            IEnumerationState initialState = _state;
            foreach (IEnumerationState newState in initialState)
            {
                _state = newState;
                ThrowIfErrorOrDisposedState();
            }
            return Asserts.IsType<CompletedEnumerationState>(_state);
        }
    }
}

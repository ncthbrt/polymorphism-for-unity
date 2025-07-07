#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Polymorphism4Unity.Editor
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
            private readonly IEnumerable<TInput> enumerable;
            private readonly Func<TInput, TKey> keyMapper;
            private readonly Func<TInput, TValue> valueMapper;

            public NotStartedEnumerationState(IEnumerable<TInput> enumerable, Func<TInput, TKey> keyMapper, Func<TInput, TValue> valueMapper)
            {
                this.enumerable = enumerable;
                this.keyMapper = keyMapper;
                this.valueMapper = valueMapper;
            }

            public IEnumerationState Start()
            {
                try
                {
                    IEnumerator<TInput> enumerator = enumerable.GetEnumerator();
                    return new EnumeratingEnumerationState(enumerator, keyMapper, valueMapper);
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
                    IEnumerator<TInput> enumerator = enumerable.GetEnumerator();
                    return new EnumeratingEnumerationState(enumerator, keyMapper, valueMapper).GetEnumerator();
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

        private sealed class EnumeratingEnumerationState : IEnumerationState, IDisposable
        {
            private readonly IEnumerator<TInput> enumerator;
            private readonly Func<TInput, TKey> keyMapper;
            private readonly Func<TInput, TValue> valueMapper;
            public Dictionary<TKey, TValue> Dictionary { get; } = new();
            public bool Disposed { get; private set; } = false;

            public EnumeratingEnumerationState(IEnumerator<TInput> enumerator, Func<TInput, TKey> keyMapper, Func<TInput, TValue> valueMapper)
            {
                this.enumerator = enumerator;
                this.keyMapper = keyMapper;
                this.valueMapper = valueMapper;
            }

            public (IEnumerationState nextState, (TKey key, TValue value)? keyValue) TryPullNextElement()
            {
                try
                {
                    TInput element = enumerator.Current;
                    TKey key = keyMapper(element);
                    (TKey key, TValue value)? keyValue = null;
                    if (!Dictionary.ContainsKey(key))
                    {
                        TValue value = valueMapper(element);
                        Dictionary[key] = value;
                        keyValue = (key, value);
                    }
                    if (enumerator.MoveNext())
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
                if (!Disposed)
                {
                    try
                    {
                        enumerator.Dispose();
                    }
                    finally
                    {
                        Disposed = true;
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
            public Dictionary<TKey, TValue> Dictionary { get; } = new();

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

        private IEnumerationState state;

        public LazyDictionary(IEnumerable<TInput> enumerable, Func<TInput, TKey> keyMapper, Func<TInput, TValue> valueMapper)
        {
            state = new NotStartedEnumerationState(enumerable, keyMapper, valueMapper);
        }

        public TValue this[TKey key]
        {
            get
            {
                EnsurePreconditions();
                if (state is CompletedEnumerationState completed)
                {
                    return completed.Dictionary[key];
                }
                if (state is EnumeratingEnumerationState enumerating)
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
            state.Dispose();
            state = new CompletedEnumerationState(new Dictionary<TKey, TValue>());
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
            if (state is CompletedEnumerationState completed)
            {
                return completed.Dictionary.GetEnumerator();
            }
            else if (state is EnumeratingEnumerationState enumerating)
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
                state = nextState;
                ThrowIfErrorOrDisposedState();
                if (maybeKeyValue is { } keyValue)
                {
                    yield return new KeyValuePair<TKey, TValue>(keyValue.key, keyValue.value);
                }
                if (state is not EnumeratingEnumerationState nextEnumerating)
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
            if (state is CompletedEnumerationState completed)
            {
                return completed.Dictionary.TryGetValue(key, out value);
            }
            else if (state is EnumeratingEnumerationState enumerating)
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
                state.Dispose();
            }
            finally
            {
                state = new DisposedEnumerationState();
            }
        }

        private void StartEnumerationIfNecessary()
        {
            if (state is NotStartedEnumerationState notStarted)
            {
                state = notStarted.Start();
                ThrowIfErrorOrDisposedState();
            }
        }

        private void ThrowIfErrorOrDisposedState()
        {
            if (state is ErrorEnumerationState error)
            {
                throw error.Exception;
            }
            if (state is DisposedEnumerationState)
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
            throw Asserts.Fail($"{nameof(state)} is {nameof(EnumeratingEnumerationState)} or ${nameof(CompletedEnumerationState)}");
        }

        private CompletedEnumerationState Force()
        {
            EnsurePreconditions();
            IEnumerationState initialState = state;
            foreach (IEnumerationState newState in initialState)
            {
                state = newState;
                ThrowIfErrorOrDisposedState();
            }
            return Asserts.IsType<IEnumerationState, CompletedEnumerationState>(state);
        }
    }
}

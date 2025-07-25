#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Polymorphism4Unity.Safety;

namespace Polymorphism4Unity.Editor.Collections
{
    internal class CachedEnumerable<TElement> : IEnumerable<TElement>
    {
        private interface ICachedEnumerableState
        {
            public ICachedEnumerableState? Next { get; }
            public TElement Current { get; }
        }

        private class NotStartedCachedEnumerableState : ICachedEnumerableState
        {
            public IEnumerable<TElement> Enumerable { get; private set; }

            public ICachedEnumerableState Next
            {
                get
                {
                    try
                    {
                        return new EnumeratingCachedEnumerableState(Enumerable.GetEnumerator());
                    }
                    catch (Exception e)
                    {
                        return new ErrorCachedEnumerableState(e);
                    }
                }
            }


            public TElement Current
            {
                get
                {
                    throw new NotImplementedException("Cannot pull value from NotStartedCachedEnumerable");
                }
            }

            public NotStartedCachedEnumerableState(IEnumerable<TElement> enumerable)
            {
                Enumerable = enumerable;
            }
        }

        private class EnumeratingCachedEnumerableState : ICachedEnumerableState
        {
            public IEnumerator<TElement> Enumerator { get; }
            public EnumeratingCachedEnumerableState(IEnumerator<TElement> enumerator)
            {
                Enumerator = enumerator;
            }
            public TElement Current => Enumerator.Current;
            public ICachedEnumerableState? Next =>
                Enumerator.MoveNext() ? this : null;
        }

        private class ErrorCachedEnumerableState : ICachedEnumerableState
        {
            public Exception Exception { get; private set; }

            public ErrorCachedEnumerableState(Exception exception)
            {
                Exception = exception;
            }

            public ICachedEnumerableState Next => throw Asserts.Never($"{nameof(Next)} should never be called");
            public TElement Current => throw Asserts.Never($"{nameof(Current)} should never be called");
        }

        private readonly List<TElement> _cache = new();
        private ICachedEnumerableState? _state;

        public CachedEnumerable(IEnumerable<TElement> enumerable)
        {
            _state = new NotStartedCachedEnumerableState(enumerable);
        }

        private void EnsureAndValidatePreconditions()
        {
            if (_state is NotStartedCachedEnumerableState notStarted)
            {
                _state = Asserts.IsNotNull(notStarted.Next);
                if (_state is ErrorCachedEnumerableState err)
                {
                    throw err.Exception;
                }
            }
            Asserts.IsTrue(_state is null or EnumeratingCachedEnumerableState or ErrorCachedEnumerableState);
        }

        private void ValidatePostConditions()
        {
            Asserts.IsNull(_state);
        }

        private void ValidationEnumerationInvariant()
        {
            if (_state is ErrorCachedEnumerableState err)
            {
                throw err.Exception;
            }
            Asserts.IsNotNull(_state);
            Asserts.IsType<EnumeratingCachedEnumerableState>(_state!);
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            EnsureAndValidatePreconditions();
            foreach (TElement item in _cache)
            {
                yield return item;
            }
            while (_state is EnumeratingCachedEnumerableState enumerating)
            {
                ValidationEnumerationInvariant();
                try
                {
                    TElement item = enumerating.Current;
                    _cache.Add(item);
                    _state = _state.Next;
                }
                catch (Exception e)
                {
                    _state = new ErrorCachedEnumerableState(e);
                }
            }
            Asserts.IsNull(_state);
            ValidatePostConditions();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
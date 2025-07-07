#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using static Polymorphism4Unity.Editor.Asserts;

namespace Polymorphism4Unity.Editor
{
    public interface ICachedEnumerable<TElement> : IEnumerable<TElement>
    {
        public ICachedEnumerable<TElement> Head { get; }
        public ICachedEnumerable<TElement>? Next { get; }
        public TElement Value { get; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<TElement> IEnumerable<TElement>.GetEnumerator()
        {
            ICachedEnumerable<TElement>? current = this;
            do
            {
                yield return current.Value;
                current = current.Next;
                if (current is null)
                {
                    yield break;
                }
            }
            while (true);
        }
    }

    class WrappedCachedEnumerable<TElement> : ICachedEnumerable<TElement>
    {
        public ICachedEnumerable<TElement> Head { get; protected set; }
        private ICachedEnumerable<TElement> inner;

        public WrappedCachedEnumerable(ICachedEnumerable<TElement> inner)
        {
            Head = this;
            this.inner = inner;
        }

        public WrappedCachedEnumerable(ICachedEnumerable<TElement> inner, ICachedEnumerable<TElement> head)
        {
            Head = head;
            this.inner = inner;
        }

        public TElement Value
        {
            get
            {

                MaybeCacheValue();
                return inner.Value;
            }
        }

        public ICachedEnumerable<TElement>? Next
        {
            get
            {
                MaybeCacheValue();
                return inner.Next;
            }
        }

        private void MaybeCacheValue()
        {
            if (inner is NotStartedCachedEnumerable<TElement> notStarted)
            {
                inner = IsNotNull(notStarted.Next);
            }
            if (inner is not CachedCachedEnumerable<TElement>)
            {
                TElement value = inner.Value;
                ICachedEnumerable<TElement>? next = Next;
                if (next is not null)
                {
                    next = new WrappedCachedEnumerable<TElement>(next);
                }
                inner = new CachedCachedEnumerable<TElement>(value, next, Head);
            }
        }
    }

    class NotStartedCachedEnumerable<TElement> : ICachedEnumerable<TElement>
    {
        public IEnumerable<TElement> Enumerable { get; private set; }
        public ICachedEnumerable<TElement> Head { get; }

        public ICachedEnumerable<TElement>? Next =>
            new EnumeratingCachedEnumerable<TElement>(Enumerable.GetEnumerator(), Head);

        public TElement Value
        {
            get
            {
                throw new NotImplementedException("Cannot pull value from NotStartedCachedEnumerable");
            }
        }

        public NotStartedCachedEnumerable(IEnumerable<TElement> enumerable)
        {
            Enumerable = enumerable;
            Head = this;
        }
    }

    class CachedCachedEnumerable<TElement> : ICachedEnumerable<TElement>
    {
        public ICachedEnumerable<TElement> Head { get; protected set; }

        public TElement Value { get; }
        public ICachedEnumerable<TElement>? Next { get; set; }

        public CachedCachedEnumerable(TElement value, ICachedEnumerable<TElement>? next, ICachedEnumerable<TElement> head)
        {
            Value = value;
            Next = next;
            Head = head;
        }
    }

    class EnumeratingCachedEnumerable<TElement> : ICachedEnumerable<TElement>
    {
        public ICachedEnumerable<TElement> Head { get; protected set; }
        public IEnumerator<TElement> Enumerator { get; private set; }
        private TElement? value = default;

        public TElement Value => value ??= Enumerator.Current;

        public ICachedEnumerable<TElement>? Next
        {
            get
            {
                if (!Enumerator.MoveNext())
                {
                    return null;
                }
                else
                {
                    return new EnumeratingCachedEnumerable<TElement>(Enumerator, Head);
                }
            }
        }

        public EnumeratingCachedEnumerable(IEnumerator<TElement> enumerator, ICachedEnumerable<TElement> head)
        {
            Enumerator = enumerator;
            Head = head;
        }
    }

    class ErrorCachedEnumerable<TElement> : ICachedEnumerable<TElement>
    {
        public ICachedEnumerable<TElement> Head { get; protected set; }

        public Exception Exception { get; private set; }

        public ErrorCachedEnumerable(Exception exception, ICachedEnumerable<TElement> head)
        {
            Exception = exception;
            Head = head;
        }

        public ICachedEnumerable<TElement>? Next
        {
            get
            {
                throw Exception;
            }
        }

        public TElement Value
        {
            get
            {
                throw Exception;
            }
        }
    }



}
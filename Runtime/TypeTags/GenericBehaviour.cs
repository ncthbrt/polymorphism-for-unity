#nullable enable
using System;

namespace Polymorphism4Unity.TypeTags
{
    [Flags]
    public enum GenericBehaviour : uint
    {
        Generic = 1 << 0,
        Constructed = 1 << 1,
        GenericOrConstructed = Generic | Constructed
    }
}
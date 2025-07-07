#nullable enable
using System;
using UnityEngine;

namespace Polymorphism4Unity.Enums
{
    [Flags, Serializable]
    public enum TypesFilter : uint
    {
        Nulls = 1 << 0,
        Abstracts = 1 << 1,
        Generics = 1 << 2,
        Interfaces = 1 << 3,
        ValueTypes = 1 << 4,
        Classes = 1 << 5,
        HasDefaultPublicConstructor = 1 << 16,
        IsPublic = 1 << 17,
        Concretes = ValueTypes | Classes | HasDefaultPublicConstructor | IsPublic,
        ConcretesAndNulls = Concretes | Nulls,
        All = Nulls
                  | Abstracts
                  | Generics
                  | Interfaces
                  | Classes
                  | ValueTypes
    }
}
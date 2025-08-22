#nullable enable
using System;
using JetBrains.Annotations;

namespace Polymorphism4Unity.Enums
{
    [Flags, Serializable, PublicAPI]
    public enum TypesFilter : uint
    {
        Nulls = 1 << 0,
        Abstracts = 1 << 1,
        Generics = 1 << 2,
        Interfaces = 1 << 3,
        ValueTypes = 1 << 4,
        Classes = 1 << 5,
        MonoBehaviours = 1<< 6,
        ScriptableObjects = 1<< 7,
        HasDefaultPublicConstructor = 1 << 16,
        IsPublic = 1 << 17,
        Concretes = ValueTypes | Classes | HasDefaultPublicConstructor | IsPublic,
        ConcretesAndNulls = Concretes | Nulls,
        UnityObjects = MonoBehaviours | ScriptableObjects,
        All = Nulls
                  | Abstracts
                  | Generics
                  | Interfaces
                  | Classes
                  | ValueTypes
                  | UnityObjects
    }
}
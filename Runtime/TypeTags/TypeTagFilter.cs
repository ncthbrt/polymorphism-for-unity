#nullable enable
using System;

namespace Polymorphism4Unity.TypeTags
{
    [Flags]
    public enum TypeTagFilter : uint
    {
        ShowNoneType = 1 << 0,
        ShowAbstractTypes = 1 << 1,
        ShowGenericTypeDefinitions = 1 << 2,
        ShowTypesWithoutParameterlessConstructor = 1 << 3,
        ShowInterfaces = 1 << 4,
        ShowStructs = 1 << 5,
        ShowClasses = 1 << 6,
        ShowOnlyConcreteTypes = ShowStructs | ShowClasses,
        ShowOnlyConcreteAndNoneTypes = ShowStructs | ShowClasses | ShowNoneType,
        ShowAll = ShowNoneType
                  | ShowAbstractTypes
                  | ShowGenericTypeDefinitions
                  | ShowTypesWithoutParameterlessConstructor
                  | ShowInterfaces
                  | ShowClasses
    }
}
#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Polymorphism4Unity.Editor
{
    internal interface IDynamicReadonlyInstance
    {
        object? this[string memberName] { get; }
        bool TryGetValue(string memberName, Type expectedType, out object? result);
        bool TryGetValue<TExpectedResult>(string memberName, out TExpectedResult? result);
        TExpectedResult? GetValue<TExpectedResult>(string memberName);
        object? GetValue(string memberName, Type expectedType);
    }

    internal class DynamicReadonlyInstance<TBaseType> : IDynamicReadonlyInstance
    {
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance;
        const MemberTypes memberTypes = MemberTypes.Property | MemberTypes.Field;
        private static readonly Cache<string, MemberInfo[]> memberInfoCache = new(GetMemberInfo);

        private static MemberInfo[] GetMemberInfo(string memberName)
        {
            List<MemberInfo> result = new();
            foreach (MemberInfo memberInfo in typeof(TBaseType).GetMember(memberName, memberTypes, bindingFlags))
            {
                if (memberInfo is PropertyInfo or FieldInfo)
                {
                    result.Add(memberInfo);
                }
            }
            return result.ToArray();
        }

        private readonly TBaseType value;
        public DynamicReadonlyInstance(TBaseType value)
        {
            this.value = value;
        }

        public object? this[string memberName]
        {
            get
            {
                foreach (MemberInfo memberInfo in memberInfoCache[memberName])
                {
                    try
                    {
                        switch (memberInfo)
                        {
                            case PropertyInfo propertyInfo:
                                return propertyInfo.GetValue(value);
                            case FieldInfo fieldInfo:
                                return fieldInfo.GetValue(value);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                throw MemberNotFound(memberName);
            }
        }

        public bool TryGetValue(string memberName, Type expectedType, out object? result)
        {
            foreach (MemberInfo memberInfo in memberInfoCache[memberName])
            {
                try
                {
                    switch (memberInfo)
                    {

                        case PropertyInfo propertyInfo:
                            {
                                object? propertyValue = propertyInfo.GetValue(value);
                                if (propertyValue.GetType().Is(expectedType))
                                {
                                    result = propertyValue;
                                    return true;
                                }
                                break;
                            }
                        case FieldInfo fieldInfo:
                            {
                                object? fieldValue = fieldInfo.GetValue(value);
                                if (fieldValue.GetType().Is(expectedType))
                                {
                                    result = fieldValue;
                                    return true;
                                }
                                break;
                            }
                    }
                }
                catch
                {
                    continue;
                }
            }
            result = null;
            return false;
        }

        public bool TryGetValue<TExpectedResult>(string memberName, out TExpectedResult? result)
        {
            foreach (MemberInfo memberInfo in memberInfoCache[memberName])
            {
                try
                {
                    switch (memberInfo)
                    {
                        case PropertyInfo propertyInfo:
                            {
                                object? propertyValue = propertyInfo.GetValue(value);
                                if (propertyValue is null)
                                {
                                    result = default;
                                    return true;
                                }
                                if (propertyValue is TExpectedResult expectedResult)
                                {
                                    result = expectedResult;
                                    return true;
                                }
                                break;
                            }
                        case FieldInfo fieldInfo:
                            {
                                object? fieldValue = fieldInfo.GetValue(value);
                                if (fieldValue is null)
                                {
                                    result = default;
                                    return true;
                                }
                                if (fieldValue is TExpectedResult expectedResult)
                                {
                                    result = expectedResult;
                                    return true;
                                }
                                break;
                            }
                    }
                }
                catch
                {
                    continue;
                }
            }
            result = default;
            return false;
        }

        private Exception MemberNotFound(string memberName, Type expectedType) =>
            new KeyNotFoundException($"Could not find value of member with name {memberName} and assignable to type {expectedType.Name} inside {typeof(TBaseType).FullName}.");

        private Exception MemberNotFound(string memberName) =>
            new KeyNotFoundException($"Could not find value of member with name {memberName} inside type {typeof(TBaseType).FullName}.");

        public TExpectedResult? GetValue<TExpectedResult>(string memberName) =>
            TryGetValue(memberName, out TExpectedResult? result)
                ? result!
                : throw MemberNotFound(memberName, typeof(TExpectedResult));

        public object? GetValue(string memberName, Type expectedType) =>
             TryGetValue(memberName, expectedType, out object? result)
                ? result
                : throw MemberNotFound(memberName, expectedType);
    }


}
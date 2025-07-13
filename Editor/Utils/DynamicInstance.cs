#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using Polymorphism4Unity.Editor.Collections;

namespace Polymorphism4Unity.Editor.Utils
{
    internal class DynamicReadonlyInstance<TBaseType>
    {
        private const BindingFlags DeclaredInstanceFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance;
        private const MemberTypes MemberTypes = System.Reflection.MemberTypes.Property | System.Reflection.MemberTypes.Field;
        
        // ReSharper disable once StaticMemberInGenericType
        // This is correct as we want a separate cache for each member of the TBaseType
        private static readonly Cache<string, MemberInfo[]> _memberInfoCache = new(GetMemberInfo);

        private static MemberInfo[] GetMemberInfo(string memberName)
        {
            List<MemberInfo> result = new();
            foreach (MemberInfo memberInfo in typeof(TBaseType).GetMember(memberName, MemberTypes, DeclaredInstanceFlag))
            {
                if (memberInfo is PropertyInfo or FieldInfo)
                {
                    result.Add(memberInfo);
                }
            }
            return result.ToArray();
        }

        private readonly TBaseType _value;
        public DynamicReadonlyInstance(TBaseType value)
        {
            this._value = value;
        }

        public object? this[string memberName]
        {
            get
            {
                foreach (MemberInfo memberInfo in _memberInfoCache[memberName])
                {
                    try
                    {
                        switch (memberInfo)
                        {
                            case PropertyInfo propertyInfo:
                                return propertyInfo.GetValue(_value);
                            case FieldInfo fieldInfo:
                                return fieldInfo.GetValue(_value);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
                throw MemberNotFound(memberName);
            }
        }

        public bool TryGetValue(string memberName, Type expectedType, out object? result)
        {
            foreach (MemberInfo memberInfo in _memberInfoCache[memberName])
            { 
                switch (memberInfo)
                {
                    case PropertyInfo propertyInfo:
                        {
                            object? propertyValue;
                            try
                            {
                                propertyValue = propertyInfo.GetValue(_value);
                            }
                            catch
                            {
                                continue;
                            }
                            if (propertyValue is null || propertyValue.GetType().Is(expectedType))
                            {
                                result = propertyValue;
                                return true;
                            }
                            break;
                        }
                    case FieldInfo fieldInfo:
                        {
                            object? fieldValue;
                            try
                            {
                                fieldValue = fieldInfo.GetValue(_value);
                            }
                            catch
                            {
                                continue;
                            }
                            if (fieldValue is null || fieldValue.GetType().Is(expectedType))
                            {
                                result = fieldValue;
                                return true;
                            }
                            break;
                        }
                }
            }
            result = null;
            return false;
        }

        public bool TryGetValue<TExpectedResult>(string memberName, out TExpectedResult? result)
        {
            foreach (MemberInfo memberInfo in _memberInfoCache[memberName])
            {
                switch (memberInfo)
                {
                    case PropertyInfo propertyInfo:
                        {
                            object? propertyValue;
                            try
                            {
                                propertyValue = propertyInfo.GetValue(_value);
                            }
                            catch
                            {
                                continue;
                            }
                            switch (propertyValue)
                            {
                                case null:
                                    result = default;
                                    return true;
                                case TExpectedResult expectedResult:
                                    result = expectedResult;
                                    return true;
                            }
                            break;
                        }
                    case FieldInfo fieldInfo:
                        {
                            object? fieldValue;
                            try
                            {
                                fieldValue = fieldInfo.GetValue(_value);
                            }
                            catch
                            {
                                continue;
                            }
                            switch (fieldValue)
                            {
                                case null:
                                    result = default;
                                    return true;
                                case TExpectedResult expectedResult:
                                    result = expectedResult;
                                    return true;
                            }
                            break;
                        }
                    default:
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

        public TExpectedResult GetValue<TExpectedResult>(string memberName) =>
            TryGetValue(memberName, out TExpectedResult? result)
                ? result!
                : throw MemberNotFound(memberName, typeof(TExpectedResult));

        public object? GetValue(string memberName, Type expectedType) =>
            TryGetValue(memberName, expectedType, out object? result)
                ? result
                : throw MemberNotFound(memberName, expectedType);
    }


}
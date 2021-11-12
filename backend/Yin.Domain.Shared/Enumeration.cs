using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Yin.Domain.Shared
{
    /// <summary>
    /// 枚举抽象基类
    /// </summary>
    public abstract class Enumeration : IComparable
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 枚举值
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// construct
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;
        /// <summary>
        /// 获取当前所有类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }
        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Id.GetHashCode();

        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        {
            var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absoluteDifference;
        }
        /// <summary>
        /// 通过值 获取当前类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T FromValue<T>(int value) where T : Enumeration
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }
        /// <summary>
        ///  通过名称 获取当前 类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static T FromDisplayName<T>(string displayName) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }
        /// <summary>
        /// CompareTo
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
    }

    public abstract class Enumeration<TValue> : IComparable
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 枚举值
        /// </summary>
        public TValue Id { get; private set; }
        /// <summary>
        /// construct
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        protected Enumeration(TValue id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString() => Name;
        /// <summary>
        /// 获取当前所有类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }
        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var otherValue = obj as Enumeration;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = Id.Equals(otherValue.Id);

            return typeMatches && valueMatches;
        }
        /// <summary>
        /// GetHashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Id.GetHashCode();

        public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        {
            var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
            return absoluteDifference;
        }
        /// <summary>
        /// 通过值 获取当前类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T FromValue<T>(int value) where T : Enumeration
        {
            var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
            return matchingItem;
        }
        /// <summary>
        ///  通过名称 获取当前 类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public static T FromDisplayName<T>(string displayName) where T : Enumeration
        {
            var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
            return matchingItem;
        }

        private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
        {
            var matchingItem = GetAll<T>().FirstOrDefault(predicate);

            if (matchingItem == null)
                throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

            return matchingItem;
        }
        /// <summary>
        /// CompareTo
        /// 排序 相关 ？
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(object other) => -1;
    }
}

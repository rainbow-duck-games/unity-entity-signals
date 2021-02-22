using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

/********************************
 * This is part developed for TACT.Net
 * Source code available here - https://github.com/tdupont750/tact.net/blob/9d73a912dfd64bbd7fa88d3d1460c23c848af61a/framework/src/Tact/Extensions/TypeExtensions.cs
 * Author: Tom DuPont / tdupont750@gmail.com
 ********************************/
namespace RainbowDuckGames.UnityEntitySignals.Utility.Tact {
    public static class TypeExtensions {
        internal static readonly ConcurrentDictionary<MethodBase, IReadOnlyList<Type>> ParameterMap =
            new ConcurrentDictionary<MethodBase, IReadOnlyList<Type>>();

        public static EfficientInvoker GetMethodInvoker(this Type type, string methodName) {
            return EfficientInvoker.ForMethod(type, methodName);
        }
        
        public static EfficientInvoker GetMethodInvoker(this Type type, MethodInfo method) {
            return EfficientInvoker.ForMethod(type, method);
        }

        public static EfficientInvoker GetPropertyInvoker(this Type type, string propertyName) {
            return EfficientInvoker.ForProperty(type, propertyName);
        }
    }
}
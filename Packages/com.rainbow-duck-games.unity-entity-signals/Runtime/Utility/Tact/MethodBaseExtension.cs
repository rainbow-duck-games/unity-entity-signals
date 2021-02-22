using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/********************************
 * This is part developed for TACT.Net
 * Source code available here - https://github.com/tdupont750/tact.net/blob/9d73a912dfd64bbd7fa88d3d1460c23c848af61a/framework/src/Tact/Extensions/MethodBaseExtensions.cs
 * Author: Tom DuPont / tdupont750@gmail.com
 ********************************/
namespace RainbowDuckGames.UnityEntitySignals.Utility.Tact {
    public static class MethodBaseExtensions {
        public static IReadOnlyList<Type> GetParameterTypes(this MethodBase method) {
            return TypeExtensions.ParameterMap.GetOrAdd(method,
                c => c.GetParameters().Select(p => p.ParameterType).ToArray());
        }
    }
}
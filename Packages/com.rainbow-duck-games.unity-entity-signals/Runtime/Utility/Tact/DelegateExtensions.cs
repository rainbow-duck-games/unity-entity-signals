using System;

/********************************
 * This is part developed for TACT.Net
 * Source code available here - https://github.com/tdupont750/tact.net/blob/9d73a912dfd64bbd7fa88d3d1460c23c848af61a/framework/src/Tact/Extensions/DelegateExtensions.cs
 * Author: Tom DuPont / tdupont750@gmail.com
 ********************************/
namespace RainbowDuckGames.UnityEntitySignals.Utility.Tact {
    public static class DelegateExtensions {
        public static EfficientInvoker GetInvoker(this Delegate del) {
            return EfficientInvoker.ForDelegate(del);
        }
    }
}
using System;

namespace App.Utilities {
    public static class ToHtmlLocalTimeStringExtension {
        // example from https://developer.mozilla.org/en-US/docs/Web/HTML/Element/time
        // 2011-11-18 14:54:39.929
        public static string ToHtmlLocalTimeString(this DateTime datetime) => datetime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss.fff");
    }
}

using System;

namespace SDProject.Extensions {
    public static class DateTimeExtension {
        public static long ToUnixTime(this DateTime dateTime) {
            return (long)dateTime.Subtract(DateTime.UnixEpoch).TotalSeconds;
        }
    }
}

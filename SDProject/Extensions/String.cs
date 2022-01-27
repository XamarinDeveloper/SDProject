using System;

namespace SDProject.Extensions {
    public static class StringExtension {
        public static int ToInt32(this string str) {
            return Convert.ToInt32(str);
        }
        public static string PadBoth(this string str, int totalWidth, char paddingChar, int shiftLeft = 0) {
            int padWidth = totalWidth - str.Length;
            int padLeft = (padWidth + 1 - shiftLeft) / 2;
            return str.PadLeft(padLeft + str.Length, paddingChar).PadRight(totalWidth, paddingChar);
        }
        public static string PadBoth(this string str, int totalWidth, int shiftLeft = 0) {
            return str.PadBoth(totalWidth, ' ', shiftLeft);
        }

        public static bool IsEmpty(this string str) {
            return str == null || str.Length == 0;
        }
    }
}

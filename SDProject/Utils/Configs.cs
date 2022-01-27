using AndroidX.Annotations;
using System.Collections.Generic;

namespace SDProject.Utils {
    internal static class Configs {
        // Server
        internal const string ServerIPAddress = "185.226.116.49";
        internal const int ServerPort = 50051;
        internal const int ServerTimeout = 8;

        // Coockie
        internal const int DurationForever = -1;
        internal const int MessageDuration = 2000;
        internal const int ErrorDuration = MessageDuration;
        internal const int SuccessDuration = MessageDuration;

        [FontRes]
        internal const int AppNormalFontRes = Resource.Font.iransans_fanum_regular;
        [FontRes]
        internal const int AppBoldFontRes = Resource.Font.iransans_fanum_bold;

        internal static readonly List<string> DangerTypes = new List<string>{
            "خشونت خانگی",
            "تهدید",
            "درگیری",
            "موارد دیگر",
        };
    }
}
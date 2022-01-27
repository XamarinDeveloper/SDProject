using Android.Content;
using GrpcServer.User;
using Newtonsoft.Json;
using User = SDProject.Types.User;

namespace SDProject.Utils {
    internal class Database {
        private static ISharedPreferences SharedPreferences => Publics.SharedPreferences;

        //public static readonly string NOTIFICATION_CHANNEL_ID = "notification_channel";
        //public static readonly int NOTIFICATION_ID = 100;

        public static int UserId => User.Id;

        public static User User {
            get => JsonConvert.DeserializeObject<User>(SharedPreferences.GetString("User", JsonConvert.SerializeObject(new User {
                Id = -1
            })));
            set => SharedPreferences.Edit().PutString("User", JsonConvert.SerializeObject(value)).Apply();
        }

        public static UserStatus UserStatus {
            get => User.Status;
            set {
                var user = User;
                user.Status = value;
                User = user;
            }
        }

        public static bool ShowNotifications {
            get => SharedPreferences.GetBoolean("ShowNotifications", true);
            set => SharedPreferences.Edit().PutBoolean("ShowNotifications", value).Apply();
        }
        public static bool UseBiometric {
            get => SharedPreferences.GetBoolean("UseBiometric", false);
            set => SharedPreferences.Edit().PutBoolean("UseBiometric", value).Apply();
        }

        //public static TimeSpan BirthdayUtcOffset {
        //    get => JsonConvert.DeserializeObject<TimeSpan>(sharedPreferences.GetString("BirthdayUtcOffset", JsonConvert.SerializeObject(new TimeSpan(0, 0, 0))));
        //    set => sharedPreferences.Edit().PutString("BirthdayUtcOffset", JsonConvert.SerializeObject(value)).Apply();
        //}

        //public static string FirebaseToken {
        //    get => SharedPreferences.GetString("FirebaseToken", null);
        //    set => SharedPreferences.Edit().PutString("FirebaseToken", value).Apply();
        //}

        //public static bool TokenIsStored {
        //    get => SharedPreferences.GetBoolean("TokenIsStored", false);
        //    set => SharedPreferences.Edit().PutBoolean("TokenIsStored", value).Apply();
        //}
    }
}
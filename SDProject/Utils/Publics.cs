using Android.Content;

namespace SDProject.Utils {
    public static class Publics {
        private static Context applicationContext;
        public static Context ApplicationContext { get => applicationContext; set => applicationContext = value; }
        public static ISharedPreferences SharedPreferences => applicationContext.GetSharedPreferences("_", FileCreationMode.Private);

        private static bool isFirstLaunch;
        public static bool IsFirstLaunch { 
            get {
                if (isFirstLaunch) {
                    return true;
                }
                var result = SharedPreferences.GetBoolean("IsFirstLaunch", true);
                SharedPreferences.Edit().PutBoolean("IsFirstLaunch", false).Apply();
                return isFirstLaunch = result;
            }
        }
    }
}
using Android;
using Android.Graphics;
using Android.Widget;
using AndroidX.Annotations;
using AndroidX.Biometric;
using AndroidX.Core.Content.Resources;
using Java.Lang;
using Java.Lang.Reflect;
using Org.Aviran.CookieBar2;
using SDProject.Extensions;
using SDProject.Types;
using System;
using System.Diagnostics;
using System.Reflection;
using Activity = Android.App.Activity;

namespace SDProject.Utils {
    public static class Tools {
        private static XActivity CallerActivity(int frameCount = 1) {
            var methodInfo = new StackTrace().GetFrame(frameCount + 1).GetMethod();
            var type = methodInfo.ReflectedType;
            var instanceInfo = type.GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            return instanceInfo.GetValue(null) as XActivity;
        }

        public static void ShowToast(string text, ToastLength length) {
            Toast.MakeText(Publics.ApplicationContext, text, length).Show();
        }
        public static void ShowCookie(Activity activity, string title, string message, string action, Action<string, string> onClick, long duration, int backColorResId, int textColorResId, int actionColorResId) {
            activity.RunOnUiThread(() => {
                var cookieBar = CookieBar.Build(activity)
                    .SetTitle(title)
                    .SetMessage(message)
                    .SetBackgroundColor(backColorResId)
                    .SetTitleColor(textColorResId)
                    .SetMessageColor(textColorResId)
                    .SetActionColor(actionColorResId)
                    .SetAnimationIn(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_in_bottom)
                    .SetAnimationOut(Resource.Animation.abc_slide_out_top, Resource.Animation.abc_slide_out_bottom)
                    .SetAction(action, () => { onClick(title, message); });
                if (duration == -1) {
                    cookieBar.SetEnableAutoDismiss(false);
                }
                else {
                    cookieBar.SetDuration(duration);
                }
                cookieBar.Show();
            });
        }
        public static void ShowMessage(string message, long duration, string action = null, Action<string, string> onClick = null, Activity activity = null) {
            ShowCookie(activity ?? CallerActivity(), null, message, action?.PadBoth(action.Length + 10), onClick, duration, Resource.Color.Message, Resource.Color.OnMessage, Resource.Color.OnMessageVariant);
        }
        public static void ShowError(string message, long duration, Activity activity = null) {
            ShowCookie(activity ?? CallerActivity(), "خطا", message, null, null, duration, Resource.Color.Error, Resource.Color.OnError, Resource.Color.OnErrorVariant);
        }
        public static void ShowSuccess(string message, long duration, Activity activity = null) {
            ShowCookie(activity ?? CallerActivity(), null, message, null, null, duration, Resource.Color.Success, Resource.Color.OnSuccess, Resource.Color.OnSuccessVariant);
        }
        internal static bool HasPermissions(Activity activity, params string[] permissions) {
            bool result = true;
            foreach (string permission in permissions) {
                result &= activity.CheckSelfPermission(permission) == Android.Content.PM.Permission.Granted;
            }
            return result;
        }
        internal static bool HasPermissions(params string[] permissions) {
            bool result = true;
            foreach (string permission in permissions) {
                result &= Publics.ApplicationContext.CheckSelfPermission(permission) == Android.Content.PM.Permission.Granted;
            }
            return result;
        }
        internal static bool HasBiometricPermission() {
            return HasPermissions(Manifest.Permission.UseBiometric);
        }
        internal static bool HasLocationPermission() {
            return HasPermissions(Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation);
        }
        internal static bool HasCallPermission() {
            return HasPermissions(Manifest.Permission.CallPhone);
        }

        internal static bool ShouldShowPermissionError(params string[] permissions) {
            bool result = true;
            foreach (string permission in permissions) {
                result &= CallerActivity().ShouldShowRequestPermissionRationale(permission);
            }
            return !result & !HasPermissions(CallerActivity(), permissions);
        }

        internal static bool HasBiometricAccess() {
            BiometricManager biometricManager = BiometricManager.From(CallerActivity());
            return HasBiometricPermission() && biometricManager.CanAuthenticate(BiometricManager.Authenticators.BiometricWeak) == BiometricManager.BiometricSuccess;
        }

        public static void OverrideDefaultFont([FontRes] int noramlFontRes, [FontRes] int boldFontRes) {
            Typeface normalFontTypeface = ResourcesCompat.GetFont(CallerActivity(), noramlFontRes);
            Typeface boldFontTypeface = ResourcesCompat.GetFont(CallerActivity(), boldFontRes);

            //Get Fontface.Default Field by reflection
            Class typeFaceClass = Class.ForName("android.graphics.Typeface");
            string x = "";
            foreach (var sss in typeFaceClass.GetFields()) {
                x += $"{sss.Name} {sss.Type}\n";
            }

            Field defaultFontTypefaceField = typeFaceClass.GetField("DEFAULT");
            defaultFontTypefaceField.Accessible = true;
            defaultFontTypefaceField.Set(null, normalFontTypeface);

            Field sansSerifFontTypefaceField = typeFaceClass.GetField("SANS_SERIF");
            sansSerifFontTypefaceField.Accessible = true;
            sansSerifFontTypefaceField.Set(null, normalFontTypeface);

            Field serifFontTypefaceField = typeFaceClass.GetField("SERIF");
            serifFontTypefaceField.Accessible = true;
            serifFontTypefaceField.Set(null, normalFontTypeface);

            Field boldFontTypefaceField = typeFaceClass.GetField("DEFAULT_BOLD");
            boldFontTypefaceField.Accessible = true;
            boldFontTypefaceField.Set(null, boldFontTypeface);
        }
    }
}
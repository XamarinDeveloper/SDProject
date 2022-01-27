using Android.Widget;
using AndroidX.Annotations;
using AndroidX.Lifecycle;
using Ir.XamarinDev.Android.ProgressButton;

namespace SDProject.Extensions {
    internal static class ButtonExtension {
        public static void MakeProgressButton(this Button button, ILifecycleOwner lifecycleOwner) {
            ProgressButtonHolder.BindProgressButton(lifecycleOwner, button);
            button.AttachTextChangeAnimator((textChangeAnimatorParams) => {
                textChangeAnimatorParams.FadeInMills = 300;
                textChangeAnimatorParams.FadeOutMills = 300;
            });
        }

        public static void ShowProgress(this Button button) {
            button.ShowProgress((progressParams) => {
                progressParams.ProgressColorRes = Resource.Color.Surface;
                progressParams.Gravity = DrawableButton.Gravity.Center;
            });
            button.Clickable = false;
        }

        public static void HideProgress(this Button button, [StringRes] int textRes) {
            button.Clickable = true;
            DrawableButtonExtensions.HideProgress(button, textRes);
        }

    }
}
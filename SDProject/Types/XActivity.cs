using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Biometric;
using AndroidX.Core.Content;
using SDProject.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SDProject.Types {
    public class XActivity : AppCompatActivity {
        protected static XActivity Instance;

        override protected void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);
            GetType().GetField("Instance", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, this);
            Publics.ApplicationContext = ApplicationContext;
        }
        protected override void OnResume() {
            base.OnResume();
            GetType().GetField("Instance", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, this);
            Publics.ApplicationContext = ApplicationContext;
        }
        protected override void OnDestroy() {
            base.OnDestroy();
            GetType().GetField("Instance", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, null);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected void SetStatusbarHeight(View view) {
            int id = Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (id > 0) {
                int height = Resources.GetDimensionPixelSize(id);
                view.LayoutParameters.Height = height;
            }
        }

        protected void BiometricAuth(BiometricPrompt.AuthenticationCallback callback) {
            var executor = ContextCompat.GetMainExecutor(this);
            var biometricPrompt = new BiometricPrompt(this, executor, callback);
            var promptInfo = new BiometricPrompt.PromptInfo.Builder()
                .SetTitle("لایف لاین")
                .SetSubtitle("")
                .SetAllowedAuthenticators(BiometricManager.Authenticators.BiometricWeak)
                .SetNegativeButtonText("ورود با رمز")
                .Build();
            biometricPrompt.Authenticate(promptInfo);
        }

        public void RequestPermissions(IEnumerable<string> permissions, int requestCode = 1339) {
            base.RequestPermissions(permissions.ToArray(), requestCode);
        }

    }
}
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SDProject.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDProject {
    [Activity(Label = "AboutUsActivity", Theme = "@style/AppTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.ScreenLayout, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AboutUsActivity : XActivity {
        protected new static XActivity Instance;

        #region Header
        private ImageButton backButton;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_about_us);

            SetStatusbarHeight(FindViewById(Resource.Id.statusbarPlaceholder));

            #region Header
            backButton = FindViewById<ImageButton>(Resource.Id.backButton);

            backButton.Click += delegate { OnBackPressed(); };
            #endregion
        }
    }
}
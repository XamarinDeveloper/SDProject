using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Grpc.Core;
using SDProject.Extensions;
using SDProject.Types;
using SDProject.Utils;
using SDProject.Views;
using System;
using System.Threading;
using Server = SDProject.Utils.Server;

namespace SDProject {
    [Activity(Label = "CounsellingCentersActivity", Theme = "@style/AppTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.ScreenLayout, ScreenOrientation = ScreenOrientation.Portrait)]
    public class CounsellingCentersActivity : XActivity {
        protected new static XActivity Instance;

        #region Header
        private ConstraintLayout container;

        private ImageButton backButton;
        #endregion
        #region Body
        private SwipeRefreshLayout contentRefreshLayout;
        private RecyclerView contentRecyclerView;
        private CounsellingCentersViewAdapter counsellingCentersViewAdapter;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_reading);

            SetStatusbarHeight(FindViewById(Resource.Id.statusbarPlaceholder));

            #region Header
            container = FindViewById<ConstraintLayout>(Resource.Id.container);

            backButton = FindViewById<ImageButton>(Resource.Id.backButton);

            backButton.Click += delegate { OnBackPressed(); };
            #endregion

            #region Body
            contentRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.contentRefreshLayout);
            contentRecyclerView = FindViewById<RecyclerView>(Resource.Id.contentRecyclerView);

            int wavesHeight = Resources.GetDimensionPixelSize(Resource.Dimension.waves_height);
            int contentTopMargin = Resources.GetDimensionPixelSize(Resource.Dimension.content_top_margin);
            contentRefreshLayout.SetProgressViewOffset(false, contentRefreshLayout.ProgressViewStartOffset, wavesHeight + contentTopMargin);
            contentRefreshLayout.Refresh += Refresh;

            counsellingCentersViewAdapter = new CounsellingCentersViewAdapter(this);
            contentRecyclerView.SetAdapter(counsellingCentersViewAdapter);
            contentRecyclerView.SetLayoutManager(new LinearLayoutManager(this) {
                ReverseLayout = false
            });

            contentRefreshLayout.Refreshing = true;
            Refresh();
            #endregion
        }

        private void Refresh(object sender = null, EventArgs e = null) {
            new Thread(() => {
                try {
                    counsellingCentersViewAdapter.Items = Server.CounsellingCenter.GetCenters();
                    RunOnUiThread(() => {
                        counsellingCentersViewAdapter.NotifyDataSetChanged();
                    });
                }
                catch (RpcException ex) {
                    Tools.ShowError(ex.GetMessage(), Configs.ErrorDuration, this);
                }
                catch {
                    Tools.ShowError("خطای داخلی برنامه", Configs.ErrorDuration, this);
                }
                RunOnUiThread(() => {
                    contentRefreshLayout.Refreshing = false;
                });
            }).Start();
        }
    }
}

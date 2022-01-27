using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Text;
using AndroidX.SwipeRefreshLayout.Widget;
using Google.Android.Material.ImageView;
using Grpc.Core;
using SDProject.Extensions;
using SDProject.Types;
using SDProject.Utils;
using Square.Picasso;
using System;
using System.Threading;
using Server = SDProject.Utils.Server;

namespace SDProject {
    [Activity(Label = "ReadingPostActivity", Theme = "@style/AppTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.ScreenLayout, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ReadingPostActivity : XActivity {
        protected new static XActivity Instance;

        #region Header
        private ConstraintLayout container;

        private ImageButton backButton;
        #endregion
        #region Body
        private SwipeRefreshLayout contentRefreshLayout;
        private ShapeableImageView postImageView;
        private TextView postTitleTextView;
        private TextView postBodyTextView;

        private int postId;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_reading_post);

            SetStatusbarHeight(FindViewById(Resource.Id.statusbarPlaceholder));

            #region Header
            container = FindViewById<ConstraintLayout>(Resource.Id.container);

            backButton = FindViewById<ImageButton>(Resource.Id.backButton);

            backButton.Click += delegate { OnBackPressed(); };
            #endregion

            if (Intent?.Extras == null || !Intent.Extras.ContainsKey("PostId")) {
                OnBackPressed();
                return;
            }

            #region Body
            contentRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.contentRefreshLayout);
            postImageView = FindViewById<ShapeableImageView>(Resource.Id.postImageView);
            postTitleTextView = FindViewById<TextView>(Resource.Id.postTitleTextView);
            postBodyTextView = FindViewById<TextView>(Resource.Id.postBodyTextView);

            int wavesHeight = Resources.GetDimensionPixelSize(Resource.Dimension.waves_height);
            int contentTopMargin = Resources.GetDimensionPixelSize(Resource.Dimension.content_top_margin);
            contentRefreshLayout.SetProgressViewOffset(false, contentRefreshLayout.ProgressViewStartOffset, wavesHeight + contentTopMargin);
            contentRefreshLayout.Refresh += Refresh;

            postId = Intent.Extras.GetInt("PostId");
            
            contentRefreshLayout.Refreshing = true;
            Refresh();
            #endregion
        }

        private void Refresh(object sender = null, EventArgs e = null) {
            new Thread(() => {
                try {
                    var post = Server.Blog.GetPost(postId);
                    RunOnUiThread(() => {
                        if (post.ImageUrl.IsEmpty()) {
                            postImageView.SetImageDrawable(null);
                        }
                        else {
                            Picasso.Get().Load(post.ImageUrl).Into(postImageView);
                        }
                        if (post.Title.IsEmpty()) {
                            postTitleTextView.Visibility = ViewStates.Gone;
                        }
                        else {
                            postTitleTextView.Text = post.Title;
                        }
                        if (post.Body.IsEmpty()) {
                            postBodyTextView.Visibility = ViewStates.Gone;
                        }
                        else {
                            postBodyTextView.TextFormatted = new SpannableString(HtmlCompat.FromHtml(post.Body, HtmlCompat.FromHtmlModeLegacy));
                            //postBodyTextView.MovementMethod = LinkMovementMethod.Instance;
                        }
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
using Android.App;
using Android.Content;
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
using static IR.Blue_saffron.Persianmaterialdatetimepicker.Date.DatePickerDialog;
using DatePickerDialog = IR.Blue_saffron.Persianmaterialdatetimepicker.Date.DatePickerDialog;
using Server = SDProject.Utils.Server;

namespace SDProject {
    [Activity(Label = "OnlineCounsellerActivity", Theme = "@style/AppTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.ScreenLayout, ScreenOrientation = ScreenOrientation.Portrait)]
    public class OnlineCounsellerActivity : XActivity, IOnDateSetListener {
        protected new static XActivity Instance;

        #region Header
        private ConstraintLayout container;

        private ImageButton backButton;
        #endregion
        #region Body
        private SwipeRefreshLayout contentRefreshLayout;
        private ImageButton prevButton;
        private TextView dateTextView;
        private ImageButton nextButton;
        //private TextView postBodyTextView;
        private RecyclerView contentRecyclerView;
        private SchedulesViewAdapter schedulesViewAdapter;

        private int counsellerId;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_online_counseller);

            SetStatusbarHeight(FindViewById(Resource.Id.statusbarPlaceholder));

            #region Header
            container = FindViewById<ConstraintLayout>(Resource.Id.container);

            backButton = FindViewById<ImageButton>(Resource.Id.backButton);

            backButton.Click += delegate { OnBackPressed(); };
            #endregion

            if (Intent?.Extras == null || !Intent.Extras.ContainsKey("CounsellerId")) {
                OnBackPressed();
                return;
            }

            #region Body
            contentRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.contentRefreshLayout);
            prevButton = FindViewById<ImageButton>(Resource.Id.prevButton);
            dateTextView = FindViewById<TextView>(Resource.Id.dateTextView);
            nextButton = FindViewById<ImageButton>(Resource.Id.nextButton);
            contentRecyclerView = FindViewById<RecyclerView>(Resource.Id.contentRecyclerView);

            //int wavesHeight = Resources.GetDimensionPixelSize(Resource.Dimension.waves_height);
            int contentTopMargin = Resources.GetDimensionPixelSize(Resource.Dimension.content_top_margin);
            contentRefreshLayout.SetProgressViewOffset(false, contentRefreshLayout.ProgressViewStartOffset, contentTopMargin);
            contentRefreshLayout.Refresh += Refresh;

            counsellerId = Intent.Extras.GetInt("CounsellerId");

            prevButton.Click += GoToPreviousDay;
            nextButton.Click += GoToNextDay;

            dateTextView.Click += PickDate;

            schedulesViewAdapter = new SchedulesViewAdapter(this);
            contentRecyclerView.SetAdapter(schedulesViewAdapter);
            contentRecyclerView.SetLayoutManager(new LinearLayoutManager(this) {
                ReverseLayout = false
            });

            SetDay(JDateTime.Now);
            #endregion
        }

        private void PickDate(object sender, EventArgs e) {
            DatePickerDialog datePickerDialog;
            if (JDateTime.TryParse(dateTextView.Text, out var date)) {
                datePickerDialog = DatePickerDialog.NewInstance(this, date.Year, date.Month - 1, date.Day);
            }
            else {
                datePickerDialog = DatePickerDialog.NewInstance(this, JDateTime.Now.Year, JDateTime.Now.Month - 1, JDateTime.Now.Day);
            }
            var minDate = new IR.Blue_saffron.Persianmaterialdatetimepicker.Utils.PersianCalendar();
            minDate.Parse($"{JDateTime.Now:yyyy/MM/dd}");
            datePickerDialog.SetYearRange(JDateTime.Now.Year, JDateTime.Now.Year + 1);
            datePickerDialog.MinDate = minDate;
#pragma warning disable CS0618 // Type or member is obsolete
            datePickerDialog.Show(FragmentManager, "DatePickerDialog");
#pragma warning restore CS0618 // Type or member is obsolete
        }
        public void OnDateSet(DatePickerDialog p0, int p1, int p2, int p3) {
            SetDay(new JDateTime(p1, p2 + 1, p3));
        }
        private void GoToNextDay(object sender, EventArgs e) {
            ChangeDay(1);
        }

        private void GoToPreviousDay(object sender, EventArgs e) {
            ChangeDay(-1);
        }

        private void ChangeDay(int days) {
            SetDay(JDateTime.Parse(dateTextView.Text).AddDays(days));
        }

        private void SetDay(JDateTime date) {
            date = date.Date;
            dateTextView.Text = date.ToString("yyyy/MM/dd");
            prevButton.Enabled = date != JDateTime.Now.Date;

            contentRefreshLayout.Refreshing = true;
            Refresh();
        }

        Thread refreshThread;
        private void Refresh(object sender = null, EventArgs e = null) {
            refreshThread?.Abort();
            refreshThread = new Thread(() => {
                try {
                    var date = JDateTime.Parse(dateTextView.Text);
                    var items = Server.Counseller.GetSchedules(counsellerId, date);
                    if (date == JDateTime.Parse(dateTextView.Text)) {
                        schedulesViewAdapter.Items = items;
                        RunOnUiThread(() => {
                            schedulesViewAdapter.NotifyDataSetChanged();
                        });
                    }
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
            });
            refreshThread.Start();
        }
    }
}
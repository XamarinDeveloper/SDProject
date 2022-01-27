using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.ConstraintLayout.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Internal;
using Google.Android.Material.Tabs;
using Google.Android.Material.TextField;
using Grpc.Core;
using IR.Blue_saffron.Persianmaterialdatetimepicker.Time;
using SDProject.Extensions;
using SDProject.Types;
using SDProject.Utils;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using static Android.Views.View;
using static Google.Android.Material.Tabs.TabLayout;
using static IR.Blue_saffron.Persianmaterialdatetimepicker.Date.DatePickerDialog;
using static IR.Blue_saffron.Persianmaterialdatetimepicker.Time.TimePickerDialog;
using DatePickerDialog = IR.Blue_saffron.Persianmaterialdatetimepicker.Date.DatePickerDialog;
using Server = SDProject.Utils.Server;
using TimePickerDialog = IR.Blue_saffron.Persianmaterialdatetimepicker.Time.TimePickerDialog;

namespace SDProject {
    [Activity(Label = "WatcherActivity", Theme = "@style/AppTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.ScreenLayout, ScreenOrientation = ScreenOrientation.Portrait)]
    public class WatcherActivity : XActivity, IOnDateSetListener, IOnTimeSetListener, ILocationListener {
        protected new static XActivity Instance;

        #region Header
        private ConstraintLayout container;

        private ImageButton backButton;
        #endregion
        #region Body
        private TabLayout nowFutureTabLayout;
        private TabItem nowTab;
        private TabItem futureTab;

        private TextInputLayout dangerTypeLayout;
        private TextInputLayout addressLayout;
        private TextInputLayout waitUntilLayout;
        private MaterialAutoCompleteTextView dangerType;
        private TextInputEditText address;
        private TextInputEditText waitUntil;

        private CheckableImageButton waitUntilPickDateTimeButton;

        private MaterialButton dangerNowButton;
        private MaterialButton dangerFutureButton;
        #endregion

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_watcher);

            SetStatusbarHeight(FindViewById(Resource.Id.statusbarPlaceholder));

            #region Header
            container = FindViewById<ConstraintLayout>(Resource.Id.container);

            backButton = FindViewById<ImageButton>(Resource.Id.backButton);

            backButton.Click += delegate { OnBackPressed(); };
            #endregion

            #region Body
            nowFutureTabLayout = FindViewById<TabLayout>(Resource.Id.nowFutureTabLayout);
            nowTab = FindViewById<TabItem>(Resource.Id.nowTab);
            futureTab = FindViewById<TabItem>(Resource.Id.futureTab);

            dangerTypeLayout = FindViewById<TextInputLayout>(Resource.Id.dangerTypeLayout);
            addressLayout = FindViewById<TextInputLayout>(Resource.Id.addressLayout);
            waitUntilLayout = FindViewById<TextInputLayout>(Resource.Id.waitUntilLayout);
            dangerType = FindViewById<MaterialAutoCompleteTextView>(Resource.Id.dangerType);
            address = FindViewById<TextInputEditText>(Resource.Id.address);
            waitUntil = FindViewById<TextInputEditText>(Resource.Id.waitUntil);

            dangerNowButton = FindViewById<MaterialButton>(Resource.Id.dangerNowButton);
            dangerFutureButton = FindViewById<MaterialButton>(Resource.Id.dangerFutureButton);

            waitUntilPickDateTimeButton = waitUntilLayout.FindViewById<CheckableImageButton>(Resource.Id.text_input_end_icon);

            waitUntilPickDateTimeButton.Pressable = true;
            waitUntilPickDateTimeButton.Clickable = true;

            nowFutureTabLayout.TabSelected += RemoveTabsTooltips;
            RemoveTabsTooltips(null, null);
            nowFutureTabLayout.TabSelected += LoginRegisterTabSelected;

            dangerType.Adapter = new ArrayAdapter(this, Resource.Layout.list_item_danger, Configs.DangerTypes);

            waitUntilPickDateTimeButton.Click += PickUntilDateTime;

            dangerType.TextChanged += RemoveError;
            address.TextChanged += RemoveError;
            waitUntil.TextChanged += CheckDateTime;

            dangerType.Click += RemoveError;
            address.Click += RemoveError;
            waitUntil.Click += RemoveError;

            dangerType.FocusChange += RemoveError;
            address.FocusChange += RemoveError;
            waitUntil.FocusChange += RemoveError;

            dangerNowButton.Click += SetNowDanger;
            dangerFutureButton.Click += SetFutureDanger;

            dangerNowButton.MakeProgressButton(this);
            dangerFutureButton.MakeProgressButton(this);

            if (!Tools.HasLocationPermission()) {
                addressLayout.Visibility = ViewStates.Visible;
            }
            #endregion
        }

        #region EndIcons
        private void PickUntilDateTime(object sender, EventArgs e) {
            DatePickerDialog datePickerDialog;
            //Regex dateRegex = new Regex(@"^(\d{4})/(\d{1,2})/(\d{1,2})");
            if (JDateTime.TryParse(waitUntil.Text, out var date)) {
                //var date = JDateTime.Parse(waitUntil.Text);
                datePickerDialog = DatePickerDialog.NewInstance(this, date.Year, date.Month - 1, date.Day);
            }
            else {
                datePickerDialog = DatePickerDialog.NewInstance(this, JDateTime.Now.Year, JDateTime.Now.Month - 1, JDateTime.Now.Day);
            }
            var minDate = new IR.Blue_saffron.Persianmaterialdatetimepicker.Utils.PersianCalendar();
            minDate.Parse($"{JDateTime.Now:yyyy/MM/dd}");
            datePickerDialog.SetYearRange(JDateTime.Now.Year, JDateTime.Now.Year + 100);
            datePickerDialog.MinDate = minDate;
#pragma warning disable CS0618 // Type or member is obsolete
            datePickerDialog.Show(FragmentManager, "DatePickerDialog");
#pragma warning restore CS0618 // Type or member is obsolete
        }
        private int year, month, day;
        public void OnDateSet(DatePickerDialog p0, int p1, int p2, int p3) {
            year = p1;
            month = p2 + 1;
            day = p3;

            TimePickerDialog timePickerDialog;
            //Regex timeRegex = new Regex(@" (\d{1,2}):(\d{1,2})(:(\d{1,2}))?$");
            if (JDateTime.TryParse(waitUntil.Text, out var time)) {
                //var time = JDateTime.Parse(waitUntil.Text);
                timePickerDialog = TimePickerDialog.NewInstance(this, time.Hour, time.Minute, true);
            }
            else {
                timePickerDialog = TimePickerDialog.NewInstance(this, JDateTime.Now.Hour, JDateTime.Now.Minute, true);
            }
#pragma warning disable CS0618 // Type or member is obsolete
            timePickerDialog.Show(FragmentManager, "DatePickerDialog");
#pragma warning restore CS0618 // Type or member is obsolete
        }
        public void OnTimeSet(RadialPickerLayout p0, int p1, int p2) {
            waitUntil.Text = $"{year:0000}/{month:00}/{day:00} {p1:00}:{p2:00}";
        }
        #endregion
        #region Inputs
        private void RemoveError(object sender, EventArgs e) {
            var editText = sender as TextView;
            var editTextLayout = (TextInputLayout)editText.Parent.Parent ?? (TextInputLayout)editText.Parent;
            editTextLayout.ErrorEnabled = false;
        }

        private void RemoveError(object sender, FocusChangeEventArgs e) {
            if (e.HasFocus) {
                var editText = sender as TextView;
                var editTextLayout = (TextInputLayout)editText.Parent.Parent ?? (TextInputLayout)editText.Parent;
                editTextLayout.ErrorEnabled = false;
            }
        }

        private void RemoveError(object sender, TextChangedEventArgs e) {
            var editText = sender as TextView;
            var editTextLayout = (TextInputLayout)editText.Parent.Parent ?? (TextInputLayout)editText.Parent;
            editTextLayout.ErrorEnabled = false;
        }

        private void CheckIsEditTextEmpty(params TextView[] editTexts) {
            foreach (var editText in editTexts) {
                var editTextLayout = (TextInputLayout)editText.Parent.Parent ?? (TextInputLayout)editText.Parent;
                if (editText.Text == "") {
                    RunOnUiThread(() => {
                        editTextLayout.Error = "فیلد الزامی است";
                    });
                }
            }
        }

        private void CheckDateTime(object sender, TextChangedEventArgs e = null) {
            var editText = sender as TextInputEditText;
            var editTextLayout = (TextInputLayout)editText.Parent.Parent;
            if (editText.Text == "") {
                editTextLayout.ErrorEnabled = false;
                return;
            }
            var dateTimeRegex = new Regex(@"^(\d{4})/(\d{1,2})/(\d{1,2}) (\d{1,2}):(\d{1,2})(:(\d{1,2}))?$");
            if (dateTimeRegex.IsMatch(editText.Text)) {
                try {
                    var date = JDateTime.Parse(editText.Text);
                    if (date <= JDateTime.Now) {
                        editTextLayout.Error = "زمان باید بعد از الان باشد";
                    }
                    else {
                        editTextLayout.ErrorEnabled = false;
                    }
                }
                catch {
                    editTextLayout.Error = "زمان نامعتبر";
                }
            }
            else {
                editTextLayout.Error = "زمان نامعتبر، زمان باید به صورت دقیقه:ساعت روز/ماه/سال باشد";
            }
        }

        private Location location;
        private void SetNowDanger(object sender, EventArgs e) {
            CheckIsEditTextEmpty(dangerType);
            if (addressLayout.Visibility == ViewStates.Visible) {
                CheckIsEditTextEmpty(address);
            }
            if (dangerTypeLayout.ErrorEnabled || addressLayout.ErrorEnabled) {
                return;
            }
            new Thread(() => {
                RunOnUiThread(() => {
                    dangerNowButton.ShowProgress();
                });
                try {
                    if (addressLayout.Visibility == ViewStates.Visible) {
                        Server.Report.ReportNow(dangerType.Text, address.Text);
                    }
                    else {
#pragma warning disable CS0618 // Type or member is obsolete
                        var locationManager = (LocationManager)GetSystemService(LocationService);
                        location = locationManager.GetLastKnownLocation(LocationManager.FusedProvider);
                        var locationTime = location?.Time ?? 0;
                        if (locationTime < DateTime.Now.AddMinutes(-2).ToUnixTime()) {
                            locationManager.RequestSingleUpdate(LocationManager.FusedProvider, this, MainLooper);
                        }
                        var startTime = DateTime.Now;
                        while (locationTime < DateTime.Now.AddMinutes(-2).ToUnixTime() && startTime > DateTime.Now.AddSeconds(-10)) {
                            locationTime = location?.Time ?? 0;
                        }
                        if (locationTime < DateTime.Now.AddMinutes(-2).ToUnixTime()) {
                            Tools.ShowError("امکان دریافت مکان وجود نداشت، لطفا آدرس را دستی وارد کنید", Configs.ErrorDuration);
                            RunOnUiThread(() => {
                                addressLayout.Visibility = ViewStates.Visible;
                                dangerNowButton.HideProgress(Resource.String.danger_now_button);
                            });
                            return;
                        }
                        else {
                            Server.Report.ReportNow(dangerType.Text, address.Text, location.Latitude, location.Longitude);
                        }
#pragma warning restore CS0618 // Type or member is obsolete
                    }
                    //Tools.ShowSuccess("گزارش با موفقیت ثبت شد", Configs.SuccessDuration);
                    SetResult(Result.Ok);
                    Finish();
                }
                catch (RpcException ex) {
                    Tools.ShowError(ex.GetMessage(), Configs.ErrorDuration, this);
                }
                catch {
                    Tools.ShowError("خطای داخلی برنامه", Configs.ErrorDuration, this);
                }
                RunOnUiThread(() => {
                    dangerNowButton.HideProgress(Resource.String.danger_now_button);
                });
            }).Start();
        }
        private void SetFutureDanger(object sender, EventArgs e) {
            CheckDateTime(waitUntil);
            CheckIsEditTextEmpty(dangerType, address, waitUntil);
            if (dangerTypeLayout.ErrorEnabled || addressLayout.ErrorEnabled || waitUntilLayout.ErrorEnabled) {
                return;
            }
            new Thread(() => {
                RunOnUiThread(() => {
                    dangerFutureButton.ShowProgress();
                });
                try {
                    Server.Report.ReportForFuture(dangerType.Text, address.Text, JDateTime.Parse(waitUntil.Text));
                    //Tools.ShowSuccess("گزارش با موفقیت ثبت شد", Configs.SuccessDuration);
                    SetResult(Result.Ok);
                    Finish();
                }
                catch (RpcException ex) {
                    Tools.ShowError(ex.GetMessage(), Configs.ErrorDuration, this);
                }
                catch {
                    Tools.ShowError("خطای داخلی برنامه", Configs.ErrorDuration, this);
                }
                RunOnUiThread(() => {
                    dangerFutureButton.HideProgress(Resource.String.danger_future_button);
                });
            }).Start();
        }
        #endregion

        #region Location
        public void OnLocationChanged(Location location) {
            this.location = location;
        }

        public void OnProviderDisabled(string provider) {
            Tools.ShowToast("لطفا دسترسی مکان را فعال کنید", ToastLength.Long);
            Intent onGPS = new Intent(Settings.ActionLocationSourceSettings);
            StartActivityForResult(onGPS, 963);
        }

        public void OnProviderEnabled(string provider) {
            FinishActivity(963);
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }
        #endregion

        #region Tabs
        private void LoginRegisterTabSelected(object sender, TabSelectedEventArgs e) {
            switch (e.Tab.Position) {
                case 0:
                    waitUntilLayout.Visibility = ViewStates.Gone;

                    dangerTypeLayout.ErrorEnabled = false;
                    addressLayout.ErrorEnabled = false;
                    waitUntilLayout.ErrorEnabled = false;

                    waitUntil.Text = "";
                    address.Text = "";

                    if (Tools.HasLocationPermission()) {
                        addressLayout.Visibility = ViewStates.Gone;
                    }

                    dangerNowButton.Visibility = ViewStates.Visible;
                    dangerFutureButton.Visibility = ViewStates.Invisible;
                    return;
                case 1:
                    waitUntilLayout.Visibility = ViewStates.Visible;
                    addressLayout.Visibility = ViewStates.Visible;

                    dangerTypeLayout.ErrorEnabled = false;
                    addressLayout.ErrorEnabled = false;
                    waitUntilLayout.ErrorEnabled = false;

                    dangerFutureButton.Visibility = ViewStates.Visible;
                    dangerNowButton.Visibility = ViewStates.Invisible;
                    return;
            }
        }
        #endregion

        #region Design
        private void RemoveTabsTooltips(object sender, TabSelectedEventArgs e) {
            for (int i = 0; i < nowFutureTabLayout.TabCount; i++) {
                TooltipCompat.SetTooltipText(nowFutureTabLayout.GetTabAt(i).View, (string)null);
            }
        }
        #endregion
    }
}
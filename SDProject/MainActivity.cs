using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.Core.Widget;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Internal;
using Google.Android.Material.Navigation;
using Google.Android.Material.SwitchMaterial;
using Google.Android.Material.TextField;
using Grpc.Core;
using GrpcServer.User;
using Org.Aviran.CookieBar2;
using SDProject.Extensions;
using SDProject.Types;
using SDProject.Utils;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using static Android.Views.View;
using static IR.Blue_saffron.Persianmaterialdatetimepicker.Date.DatePickerDialog;
using DatePickerDialog = IR.Blue_saffron.Persianmaterialdatetimepicker.Date.DatePickerDialog;
using Server = SDProject.Utils.Server;
using Timer = System.Timers.Timer;
using User = SDProject.Types.User;

namespace SDProject {
    [Activity(Label = "MainActivity", Theme = "@style/AppTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.ScreenLayout, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : XActivity, NavigationBarView.IOnItemSelectedListener, IOnDateSetListener {
        protected new static XActivity Instance;

        #region Common
        private CoordinatorLayout container;

        private CheckableImageButton status;
        private ImageButton backButton;

        private BottomNavigationView navigationView;
        private FloatingActionButton navigationHome;
        private IMenuItem navigationHomePlaceholder;
        private IMenuItem navigationProfile;
        private IMenuItem navigationSettings;

        private readonly Timer statusTimer = new Timer(1000);
        private readonly Timer userTimer = new Timer(10000);
        #endregion

        #region Home page
        private View homePageLayout;
        private NestedScrollView homePageScroll;

        private MaterialButton watcherButton;
        private MaterialButton onlineCounsellingButton;
        private MaterialButton counsellingCentersButton;
        private MaterialButton readingButton;
        #endregion

        #region Profile page
        private View profilePageLayout;
        private NestedScrollView profilePageScroll;

        private TextInputLayout phoneNumberLayout;
        private TextInputLayout firstNameLayout;
        private TextInputLayout lastNameLayout;
        private TextInputLayout nationalIdLayout;
        private TextInputLayout birthdayLayout;
        private TextInputLayout passwordLayout;
        private TextInputLayout passwordConfirmLayout;
        private TextInputEditText phoneNumber;
        private TextInputEditText firstName;
        private TextInputEditText lastName;
        private TextInputEditText nationalId;
        private TextInputEditText birthday;
        private TextInputEditText password;
        private TextInputEditText passwordConfirm;

        private CheckableImageButton birthdayPickDateButton;
        private CheckableImageButton passwordHideShowButton;
        private CheckableImageButton passwordConfirmHideShowButton;

        private MaterialButton modifyButton;
        private MaterialButton logoutButton;
        #endregion

        #region Settings page
        private View settingsPageLayout;
        private NestedScrollView settingsPageScroll;

        private SwitchMaterial notificationSwitch;
        private SwitchMaterial twoFactorAuthSwitch;
        private SwitchMaterial locationAccessSwitch;
        private SwitchMaterial useBiometricSwitch;

        private MaterialButton faqButton;
        private MaterialButton aboutButton;
        #endregion
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            SetStatusbarHeight(FindViewById(Resource.Id.statusbarPlaceholder));

            #region Common
            container = FindViewById<CoordinatorLayout>(Resource.Id.container);

            status = FindViewById<CheckableImageButton>(Resource.Id.status);
            backButton = FindViewById<ImageButton>(Resource.Id.backButton);

            navigationView = FindViewById<BottomNavigationView>(Resource.Id.navigationView);
            navigationView.Background = null;
            navigationView.SetOnItemSelectedListener(this);
            navigationHome = FindViewById<FloatingActionButton>(Resource.Id.navigationHome);
            navigationHomePlaceholder = navigationView.Menu.FindItem(Resource.Id.navigationHomePlaceholder);
            navigationProfile = navigationView.Menu.FindItem(Resource.Id.navigationProfile);
            navigationSettings = navigationView.Menu.FindItem(Resource.Id.navigationSettings);

            status.Visibility = ViewStates.Visible;
            status.Checked = Database.User.Status != UserStatus.Green;
            status.SetImageLevel((int)Database.User.Status);
            status.Click += ChangeStatus;

            backButton.Visibility = ViewStates.Invisible;

            navigationHome.Click += NavigateHome;
            navigationHome.ImageTintList = ColorStateList.ValueOf(Color.ParseColor(GetString(Resource.Color.Primary)));
            navigationHomePlaceholder.SetChecked(true);
            #endregion

            #region Home page
            homePageLayout = FindViewById<View>(Resource.Id.homePageLayout);

            homePageScroll = FindViewById<NestedScrollView>(Resource.Id.homePageScroll);

            watcherButton = FindViewById<MaterialButton>(Resource.Id.watcherButton);
            onlineCounsellingButton = FindViewById<MaterialButton>(Resource.Id.onlineCounsellingButton);
            counsellingCentersButton = FindViewById<MaterialButton>(Resource.Id.counsellingCentersButton);
            readingButton = FindViewById<MaterialButton>(Resource.Id.readingButton);

            watcherButton.Click += OpenWathcerActivity;
            onlineCounsellingButton.Click += OpenOnlineCounsellingActivity;
            counsellingCentersButton.Click += OpenCounsellingCentersActivity;
            readingButton.Click += OpenReadingActivity;
            #endregion

            #region Profile page
            profilePageLayout = FindViewById<View>(Resource.Id.profilePageLayout);

            profilePageScroll = FindViewById<NestedScrollView>(Resource.Id.profilePageScroll);

            phoneNumberLayout = FindViewById<TextInputLayout>(Resource.Id.phoneNumberLayout);
            firstNameLayout = FindViewById<TextInputLayout>(Resource.Id.firstNameLayout);
            lastNameLayout = FindViewById<TextInputLayout>(Resource.Id.lastNameLayout);
            nationalIdLayout = FindViewById<TextInputLayout>(Resource.Id.nationalIdLayout);
            birthdayLayout = FindViewById<TextInputLayout>(Resource.Id.birthdayLayout);
            passwordLayout = FindViewById<TextInputLayout>(Resource.Id.passwordLayout);
            passwordConfirmLayout = FindViewById<TextInputLayout>(Resource.Id.passwordConfirmLayout);
            phoneNumber = FindViewById<TextInputEditText>(Resource.Id.phoneNumber);
            firstName = FindViewById<TextInputEditText>(Resource.Id.firstName);
            lastName = FindViewById<TextInputEditText>(Resource.Id.lastName);
            nationalId = FindViewById<TextInputEditText>(Resource.Id.nationalId);
            birthday = FindViewById<TextInputEditText>(Resource.Id.birthday);
            password = FindViewById<TextInputEditText>(Resource.Id.password);
            passwordConfirm = FindViewById<TextInputEditText>(Resource.Id.passwordConfirm);

            modifyButton = FindViewById<MaterialButton>(Resource.Id.modifyButton);
            logoutButton = FindViewById<MaterialButton>(Resource.Id.logoutButton);

            birthdayPickDateButton = birthdayLayout.FindViewById<CheckableImageButton>(Resource.Id.text_input_end_icon);
            passwordHideShowButton = passwordLayout.FindViewById<CheckableImageButton>(Resource.Id.text_input_end_icon);
            passwordConfirmHideShowButton = passwordConfirmLayout.FindViewById<CheckableImageButton>(Resource.Id.text_input_end_icon);

            birthdayPickDateButton.Pressable = true;
            birthdayPickDateButton.Clickable = true;

            phoneNumber.Text = Database.User.PhoneNumber;
            firstName.Text = Database.User.FirstName;
            lastName.Text = Database.User.LastName;
            nationalId.Text = Database.User.NationalId;
            birthday.Text = Database.User.Birthday.ToString("yyyy/MM/dd");

            modifyButton.Click += ModifyUser;
            logoutButton.Click += Logout;

            birthdayPickDateButton.Click += PickBirthdayDate;
            passwordHideShowButton.Click += PasswordVisibilityChanged;
            passwordConfirmHideShowButton.Click += PasswordVisibilityChanged;

            firstName.TextChanged += RemoveError;
            lastName.TextChanged += RemoveError;
            nationalId.TextChanged += CheckNationalId;
            birthday.TextChanged += CheckBirthday;
            password.TextChanged += CheckPasswords;
            passwordConfirm.TextChanged += CheckPasswords;

            firstName.TextChanged += CheckModify;
            lastName.TextChanged += CheckModify;
            nationalId.TextChanged += CheckModify;
            birthday.TextChanged += CheckModify;
            password.TextChanged += CheckModify;
            passwordConfirm.TextChanged += CheckModify;

            firstName.Click += RemoveError;
            lastName.Click += RemoveError;
            nationalId.Click += RemoveError;
            birthday.Click += RemoveError;
            password.Click += RemoveError;
            passwordConfirm.Click += RemoveError;

            firstName.FocusChange += RemoveError;
            lastName.FocusChange += RemoveError;
            nationalId.FocusChange += RemoveError;
            birthday.FocusChange += RemoveError;
            password.FocusChange += RemoveError;
            passwordConfirm.FocusChange += RemoveError;
            #endregion

            #region Settings page
            settingsPageLayout = FindViewById<View>(Resource.Id.settingsPageLayout);

            settingsPageScroll = FindViewById<NestedScrollView>(Resource.Id.settingsPageScroll);

            notificationSwitch = FindViewById<SwitchMaterial>(Resource.Id.notificationSwitch);
            twoFactorAuthSwitch = FindViewById<SwitchMaterial>(Resource.Id.twoFactorAuthSwitch);
            locationAccessSwitch = FindViewById<SwitchMaterial>(Resource.Id.locationAccessSwitch);
            useBiometricSwitch = FindViewById<SwitchMaterial>(Resource.Id.useBiometricSwitch);

            faqButton = FindViewById<MaterialButton>(Resource.Id.faqButton);
            aboutButton = FindViewById<MaterialButton>(Resource.Id.aboutButton);

            notificationSwitch.Checked = Database.ShowNotifications;
            locationAccessSwitch.Checked = Tools.HasLocationPermission();
            locationAccessSwitch.Clickable = !Tools.HasLocationPermission();
            useBiometricSwitch.Enabled = Tools.HasBiometricAccess();
            useBiometricSwitch.Checked = Tools.HasBiometricAccess() & Database.UseBiometric;

            notificationSwitch.CheckedChange += ShowNotificationSwitch;
            twoFactorAuthSwitch.CheckedChange += TwoFactorAuthSwitch;
            locationAccessSwitch.CheckedChange += LocationAccessSwitch;
            useBiometricSwitch.CheckedChange += UseBiometricSwitch;

            aboutButton.Click += ShowAboutUs;
            #endregion

            faqButton.Click += delegate {
                //for (int i = 0; i < 30; i++) {
                //    if (JDateTime.Today.AddDays(i).DayOfWeek == DayOfWeek.Friday) {
                //        continue;
                //    }
                //    Server.Counseller.Create(1, JDateTime.Today.AddDays(i).AddHours(8), JDateTime.Today.AddDays(i).AddHours(14), 40);
                //    Server.Counseller.Create(1, JDateTime.Today.AddDays(i).AddHours(18), JDateTime.Today.AddDays(i).AddHours(22), 30);
                //}
                //for (int i = 0; i < 30; i++) {
                //    Server.Counseller.Create(2, JDateTime.Today.AddDays(i - 1).AddHours(22).AddMinutes(30), JDateTime.Today.AddDays(i).AddHours(6).AddMinutes(40), 35);
                //}
                //for (int i = 0; i < 30; i++) {
                //    if (JDateTime.Today.AddDays(i).DayOfWeek == DayOfWeek.Tuesday) {
                //        continue;
                //    }
                //    Server.Counseller.Create(3, JDateTime.Today.AddDays(i).AddHours(10).AddMinutes(30), JDateTime.Today.AddDays(i).AddHours(22).AddMinutes(30), 60);
                //}
            };

            #region Common
            if (Publics.IsFirstLaunch) {
                List<string> permissionsToRequest = new List<string>();
                if (!Tools.HasLocationPermission()) {
                    permissionsToRequest.AddRange(new[]{
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.AccessCoarseLocation
                    });
                }
                if (!Tools.HasCallPermission()) {
                    permissionsToRequest.AddRange(new[]{
                        Manifest.Permission.CallPhone
                    });
                }
                RequestPermissions(permissionsToRequest);
            }

            statusTimer.Elapsed += UpdateStatus;
            statusTimer.Start();
            userTimer.Elapsed += UpdateUser;
            userTimer.Start();
            #endregion
        }

        protected override void OnDestroy() {
            statusTimer.Stop();
            userTimer.Stop();
            base.OnDestroy();
        }
        #region Common
        private void ChangeStatus(object sender, System.EventArgs e) {
            CheckableImageButton status = sender as CheckableImageButton;
            if (status.Checked) {
                Tools.ShowMessage("وضعیت به امن تغییر کند؟", Configs.DurationForever, "بله", MakeSafe);
            }
            else {
                Tools.ShowMessage("از صفحه دیده بان استفاده کنید", Configs.MessageDuration);
            }
        }
        private void UpdateStatus(object sender = null, System.Timers.ElapsedEventArgs e = null) {
            status.Checked = Database.User.Status != UserStatus.Green;
            status.SetImageLevel((int)Database.User.Status);
        }
        private void UpdateUser(object sender, System.Timers.ElapsedEventArgs e) {
            try {
                Server.User.Update();
            }
            catch { }
        }
        private void MakeSafe(string title, string message) {
            CookieBar.Dismiss(this);
            new Thread(() => {
                try {
                    Server.User.MakeSafe();
                }
                catch (RpcException ex) {
                    Tools.ShowError(ex.GetMessage(), Configs.ErrorDuration, this);
                }
                catch {
                    Tools.ShowError("خطای داخلی برنامه", Configs.ErrorDuration, this);
                }
            }).Start();
        }
        #endregion

        #region Home page
        #region // Inputs
        private void OpenWathcerActivity(object sender, EventArgs e) {
            StartActivityForResult(typeof(WatcherActivity), 369);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data) {
            base.OnActivityResult(requestCode, resultCode, data);
            if(requestCode == 369) {
                if(resultCode == Result.Ok) {
                    UpdateStatus();
                    Tools.ShowSuccess("گزارش با موفقیت ثبت شد", Configs.SuccessDuration);
                }
            }
        }

        private void OpenOnlineCounsellingActivity(object sender, EventArgs e) {
            StartActivity(typeof(OnlineCounsellingActivity));
        }
        private void OpenCounsellingCentersActivity(object sender, EventArgs e) {
            StartActivity(typeof(CounsellingCentersActivity));
        }
        private void OpenReadingActivity(object sender, EventArgs e) {
            StartActivity(typeof(ReadingActivity));
        }
        #endregion
        #endregion

        #region Profile page
        #region // EndIcons
        private void PickBirthdayDate(object sender, EventArgs e) {
            DatePickerDialog datePickerDialog;
            //Regex dateRegex = new Regex(@"^(\d{4})/(\d{1,2})/(\d{1,2})$");
            if (JDateTime.TryParse(birthday.Text, out var date)) {
                //var (year, month, day) = dateRegex.Match(birthday.Text).Groups.Skip(1).Select(item => item.Value.ToInt32()).ToArray();
                //var date = JDateTime.Parse(birthday.Text);
                datePickerDialog = DatePickerDialog.NewInstance(this, date.Year, date.Month - 1, date.Day);
            }
            else {
                datePickerDialog = DatePickerDialog.NewInstance(this, JDateTime.Now.Year, JDateTime.Now.Month - 1, JDateTime.Now.Day);
            }
            var maxDate = new IR.Blue_saffron.Persianmaterialdatetimepicker.Utils.PersianCalendar();
            maxDate.Parse($"{JDateTime.Now:yyyy/MM/dd}");
            //maxDate.SetPersianDate(JDateTime.Now.Year, JDateTime.Now.Month - 1, JDateTime.Now.Day);
            datePickerDialog.SetYearRange(1300, JDateTime.Now.Year);
            datePickerDialog.MaxDate = maxDate;
#pragma warning disable CS0618 // Type or member is obsolete
            datePickerDialog.Show(FragmentManager, "DatePickerDialog");
#pragma warning restore CS0618 // Type or member is obsolete
        }
        public void OnDateSet(DatePickerDialog p0, int p1, int p2, int p3) {
            birthday.Text = $"{p1:0000}/{p2 + 1:00}/{p3:00}";
        }
        private bool passwordIsVisible;
        private void PasswordVisibilityChanged(object sender, EventArgs e) {
            passwordIsVisible = !passwordIsVisible;
            if (passwordIsVisible) {
                password.TransformationMethod = HideReturnsTransformationMethod.Instance;
                passwordConfirm.TransformationMethod = HideReturnsTransformationMethod.Instance;
            }
            else {
                password.TransformationMethod = PasswordTransformationMethod.Instance;
                passwordConfirm.TransformationMethod = PasswordTransformationMethod.Instance;
            }
        }
        #endregion
        #region // Inputs
        private void RemoveError(object sender, EventArgs e) {
            var editText = sender as TextInputEditText;
            var editTextLayout = (TextInputLayout)editText.Parent.Parent;
            editTextLayout.ErrorEnabled = false;
        }
        private void RemoveError(object sender, FocusChangeEventArgs e) {
            if (e.HasFocus) {
                var editText = sender as TextInputEditText;
                var editTextLayout = (TextInputLayout)editText.Parent.Parent;
                editTextLayout.ErrorEnabled = false;
            }
        }
        private void RemoveError(object sender, TextChangedEventArgs e) {
            var editText = sender as TextInputEditText;
            var editTextLayout = (TextInputLayout)editText.Parent.Parent;
            editTextLayout.ErrorEnabled = false;
        }
        private void CheckIsEditTextEmpty(params TextInputEditText[] editTexts) {
            foreach (var editText in editTexts) {
                var editTextLayout = (TextInputLayout)editText.Parent.Parent;
                if (editText.Text == "") {
                    editTextLayout.Error = "فیلد الزامی است";
                }
            }
        }
        private void CheckNationalId(object sender, TextChangedEventArgs e = null) {
            var editText = sender as TextInputEditText;
            var editTextLayout = (TextInputLayout)editText.Parent.Parent;
            var nationalIdRegex = new Regex(@"^\d{10}$");
            if (nationalIdRegex.IsMatch(editText.Text)) {
                int control = 0;
                for (int i = 0; i < 9; i++) {
                    control += editText.Text[i].ToInt32() * (10 - i);
                }
                control %= 11;
                if (control >= 2) {
                    control = 11 - control;
                }
                if (control != editText.Text[9].ToInt32()) {
                    editTextLayout.Error = "کد ملی نامعتبر";
                }
                else {
                    editTextLayout.ErrorEnabled = false;
                }
            }
            else {
                editTextLayout.Error = "کد ملی نامعتبر، کد ملی باید به صورت 10 رقمی وارد شود";
            }
        }
        private void CheckBirthday(object sender, TextChangedEventArgs e = null) {
            var editText = sender as TextInputEditText;
            var editTextLayout = (TextInputLayout)editText.Parent.Parent;
            var dateRegex = new Regex(@"^(\d{4})/(\d{1,2})/(\d{1,2})$");
            if (dateRegex.IsMatch(editText.Text)) {
                //var (year, month, day) = dateRegex.Match(editText.Text).Groups.Skip(1).Select(item => item.Value.ToInt32()).ToArray();
                try {
                    //var date = new JDateTime(year, month, day);
                    var date = JDateTime.Parse(editText.Text);
                    if (date > JDateTime.Now) {
                        editTextLayout.Error = "بعد از امروز به دنیا اومدی؟ :)";
                    }
                    else if (date == JDateTime.Now.Date) {
                        editTextLayout.Error = "امروز به دنیا اومدی؟ :)";
                    }
                    else if (date > JDateTime.Now.Date.AddYears(-1)) {
                        editTextLayout.Error = "هنوز یه سالت نشده؟ :-\"";
                    }
                    else {
                        editTextLayout.ErrorEnabled = false;
                    }
                }
                catch {
                    editTextLayout.Error = "تاریخ نامعتبر";
                }
            }
            else {
                editTextLayout.Error = "تاریخ نامعتبر، تاریخ باید به صورت روز/ماه/سال باشد";
            }
        }
        private void CheckPasswords(object sender = null, TextChangedEventArgs e = null) {
            if (password.Text == "") {
                passwordLayout.ErrorEnabled = false;
                passwordConfirmLayout.ErrorEnabled = false;
                passwordConfirmLayout.Visibility = ViewStates.Gone;
                if (passwordConfirm.Text != "") {
                    passwordConfirm.Text = "";
                }
                return;
            }
            passwordConfirmLayout.Visibility = ViewStates.Visible;
            if (passwordConfirm.Text == "") {
                passwordLayout.ErrorEnabled = false;
                passwordConfirmLayout.ErrorEnabled = false;
                return;
            }
            if (password.Text != passwordConfirm.Text) {
                passwordConfirmLayout.Error = "عدم تطابق با رمز عبور";
                return;
            }
            passwordLayout.ErrorEnabled = false;
            passwordConfirmLayout.ErrorEnabled = false;
        }
        private void CheckModify(object sender = null, TextChangedEventArgs e = null) {
            if (!firstNameLayout.ErrorEnabled && firstName.Text != Database.User.FirstName) {
                modifyButton.Visibility = ViewStates.Visible;
                return;
            }
            if (!lastNameLayout.ErrorEnabled && lastName.Text != Database.User.LastName) {
                modifyButton.Visibility = ViewStates.Visible;
                return;
            }
            if (!nationalIdLayout.ErrorEnabled && nationalId.Text != Database.User.NationalId) {
                modifyButton.Visibility = ViewStates.Visible;
                return;
            }
            if (!birthdayLayout.ErrorEnabled && JDateTime.Parse(birthday.Text) != Database.User.Birthday) {
                modifyButton.Visibility = ViewStates.Visible;
                return;
            }
            if (!passwordLayout.ErrorEnabled && !passwordConfirmLayout.ErrorEnabled && password.Text != "" && passwordConfirm.Text != "") {
                modifyButton.Visibility = ViewStates.Visible;
                return;
            }
            modifyButton.Visibility = ViewStates.Gone;
        }
        private void ModifyUser(object sender, EventArgs e) {
            CheckNationalId(nationalId);
            CheckBirthday(birthday);
            CheckPasswords();
            CheckIsEditTextEmpty(firstName, lastName, nationalId, birthday);
            if (password.Text != "") {
                CheckIsEditTextEmpty(password);
            }
            if (firstNameLayout.ErrorEnabled || lastNameLayout.ErrorEnabled || nationalIdLayout.ErrorEnabled || birthdayLayout.ErrorEnabled || passwordLayout.ErrorEnabled || passwordConfirmLayout.ErrorEnabled) {
                return;
            }
            new Thread(() => {
                try {
                    //var birthdayDate = JDateTime.Parse(birthday.Text);
                    //var utcOffset = TimeZoneInfo.Utc.GetUtcOffset(birthdayDate);
                    //birthdayDate += utcOffset;
                    //Database.BirthdayUtcOffset = utcOffset;
                    Server.User.Edit(firstName.Text, lastName.Text, nationalId.Text, JDateTime.Parse(birthday.Text), password.Text == "" ? null : password.Text);
                    RunOnUiThread(() => {
                        firstName.Text = Database.User.FirstName;
                        lastName.Text = Database.User.LastName;
                        nationalId.Text = Database.User.NationalId;
                        birthday.Text = Database.User.Birthday.ToString("yyyy/MM/dd");
                        password.Text = "";
                        passwordConfirm.Text = "";
                    });
                    Tools.ShowSuccess("ویرایش با موفقیت انجام شد", Configs.SuccessDuration);
                }
                catch (RpcException ex) {
                    Tools.ShowError(ex.GetMessage(), Configs.ErrorDuration, this);
                }
                catch {
                    Tools.ShowError("خطای داخلی برنامه", Configs.ErrorDuration, this);
                }
                RunOnUiThread(() => {
                    firstName.Text = Database.User.FirstName;
                    lastName.Text = Database.User.LastName;
                    nationalId.Text = Database.User.NationalId;
                    birthday.Text = Database.User.Birthday.ToString("yyyy/MM/dd");
                });
            }).Start();
        }
        private void Logout(object sender, EventArgs e) {
            Database.User = new User {
                Id = -1,
                PhoneNumber = Database.User.PhoneNumber,
                Password = Database.User.Password
            };
            Intent intent = new Intent(this, typeof(LoginRegisterActivity));
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
            StartActivity(intent);
            Finish();
        }
        #endregion
        #endregion

        #region Settings page
        #region // Inputs
        private void ShowNotificationSwitch(object sender, CompoundButton.CheckedChangeEventArgs e) {
            Database.ShowNotifications = notificationSwitch.Checked;
        }
        private void TwoFactorAuthSwitch(object sender, CompoundButton.CheckedChangeEventArgs e) {
            twoFactorAuthSwitch.Checked = false;
            Tools.ShowMessage("این قابلیت هنوز فعال نیست", Configs.MessageDuration);
        }
        private void LocationAccessSwitch(object sender, CompoundButton.CheckedChangeEventArgs e) {
            if (e.IsChecked && !Tools.HasLocationPermission()) {
                locationAccessSwitch.Checked = false;
                string[] permissions = {
                    Manifest.Permission.AccessFineLocation,
                    Manifest.Permission.AccessCoarseLocation
                };
                int requestId = 1339;
                RequestPermissions(permissions, requestId);
            }
        }
        private void UseBiometricSwitch(object sender, CompoundButton.CheckedChangeEventArgs e) {
            Database.UseBiometric = useBiometricSwitch.Checked;
        }

        private void ShowAboutUs(object sender, EventArgs e) {
            StartActivity(typeof(AboutUsActivity));
        }
        #endregion
        #endregion

        #region Navigation
        private void NavigateHome(object sender, System.EventArgs e) {
            homePageScroll.Visibility = ViewStates.Visible;
            profilePageScroll.Visibility = ViewStates.Gone;
            settingsPageScroll.Visibility = ViewStates.Gone;

            homePageScroll.SmoothScrollTo(0, 0);

            navigationHomePlaceholder.SetChecked(true);
            navigationHome.ImageTintList = ColorStateList.ValueOf(Color.ParseColor(GetString(Resource.Color.Primary)));
        }
        public bool OnNavigationItemSelected(IMenuItem item) {
            switch (item.ItemId) {
                case Resource.Id.navigationProfile:
                    homePageScroll.Visibility = ViewStates.Gone;
                    profilePageScroll.Visibility = ViewStates.Visible;
                    settingsPageScroll.Visibility = ViewStates.Gone;

                    profilePageScroll.SmoothScrollTo(0, 0);

                    navigationHome.ImageTintList = ColorStateList.ValueOf(Color.ParseColor(GetString(Resource.Color.OnAccent)));
                    return true;
                case Resource.Id.navigationSettings:
                    homePageScroll.Visibility = ViewStates.Gone;
                    profilePageScroll.Visibility = ViewStates.Gone;
                    settingsPageScroll.Visibility = ViewStates.Visible;

                    settingsPageScroll.SmoothScrollTo(0, 0);

                    navigationHome.ImageTintList = ColorStateList.ValueOf(Color.ParseColor(GetString(Resource.Color.OnAccent)));
                    return true;
            }
            return false;
        }
        #endregion

        #region Activity
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (Tools.ShouldShowPermissionError(Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation)) {
                Tools.ShowError("لطفا در تنظیمات گوشی دسترسی دهید", Configs.ErrorDuration);
            }
            locationAccessSwitch.Checked = Tools.HasLocationPermission();
            locationAccessSwitch.Clickable = !Tools.HasLocationPermission();
        }
        private int backPressCount = 0;
        public override void OnBackPressed() {
            backPressCount++;
            if (backPressCount == 1) {
                Tools.ShowToast("Press back again to exit", ToastLength.Short);
                new Handler(Looper.MainLooper).PostDelayed(() => { backPressCount--; }, 2000);
            }
            else {
                FinishAffinity();
            }
        }
        #endregion
    }
}


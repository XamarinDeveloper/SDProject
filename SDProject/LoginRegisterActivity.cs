using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Biometric;
using AndroidX.ConstraintLayout.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Internal;
using Google.Android.Material.Tabs;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;
using Grpc.Core;
using Ir.XamarinDev.Android.ProgressButton;
using SDProject.Extensions;
using SDProject.Types;
using SDProject.Utils;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using static Android.Views.View;
using static Google.Android.Material.Tabs.TabLayout;
using static IR.Blue_saffron.Persianmaterialdatetimepicker.Date.DatePickerDialog;
using DatePickerDialog = IR.Blue_saffron.Persianmaterialdatetimepicker.Date.DatePickerDialog;
using Server = SDProject.Utils.Server;

namespace SDProject {
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Splash", MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.ScreenLayout, ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginRegisterActivity : XActivity, IOnDateSetListener {
        protected new static XActivity Instance;

        private TabLayout loginRegisterTabLayout;
        private TabItem loginTab;
        private TabItem registerTab;

        private ConstraintLayout content;

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

        private MaterialButton loginButton;
        private ImageButton biometricButton;
        private MaterialButton registerButton;

        private MaterialTextView error;
        private MaterialTextView forgotPassword;
        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Tools.OverrideDefaultFont(Configs.AppNormalFontRes, Configs.AppBoldFontRes);

            if (Database.UserId != -1) {
                try {
                    Server.User.Login(Database.User.PhoneNumber, Database.User.Password);
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                    return;
                }
                catch (RpcException ex) {
                    Tools.ShowError(ex.GetMessage(), Configs.ErrorDuration);
                }
                catch {
                    Tools.ShowError("خطای داخلی برنامه", Configs.ErrorDuration, this);
                }
            }

            SetContentView(Resource.Layout.activity_login_register);

            SetStatusbarHeight(FindViewById(Resource.Id.statusbarPlaceholder));

            loginRegisterTabLayout = FindViewById<TabLayout>(Resource.Id.loginRegisterTabLayout);
            loginTab = FindViewById<TabItem>(Resource.Id.loginTab);
            registerTab = FindViewById<TabItem>(Resource.Id.registerTab);

            content = FindViewById<ConstraintLayout>(Resource.Id.content);

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

            loginButton = FindViewById<MaterialButton>(Resource.Id.loginButton);
            biometricButton = FindViewById<ImageButton>(Resource.Id.biometricButton);
            registerButton = FindViewById<MaterialButton>(Resource.Id.registerButton);

            error = FindViewById<MaterialTextView>(Resource.Id.error);
            forgotPassword = FindViewById<MaterialTextView>(Resource.Id.forgotPassword);

            birthdayPickDateButton = birthdayLayout.FindViewById<CheckableImageButton>(Resource.Id.text_input_end_icon);
            passwordHideShowButton = passwordLayout.FindViewById<CheckableImageButton>(Resource.Id.text_input_end_icon);
            passwordConfirmHideShowButton = passwordConfirmLayout.FindViewById<CheckableImageButton>(Resource.Id.text_input_end_icon);

            birthdayPickDateButton.Pressable = true;
            birthdayPickDateButton.Clickable = true;

            loginRegisterTabLayout.TabSelected += RemoveTabsTooltips;
            RemoveTabsTooltips(null, null);
            loginRegisterTabLayout.TabSelected += LoginRegisterTabSelected;

            birthdayPickDateButton.Click += PickBirthdayDate;
            passwordHideShowButton.Click += PasswordVisibilityChanged;
            passwordConfirmHideShowButton.Click += PasswordVisibilityChanged;

            phoneNumber.TextChanged += CheckPhoneNumber;
            firstName.TextChanged += RemoveError;
            lastName.TextChanged += RemoveError;
            nationalId.TextChanged += CheckNationalId;
            birthday.TextChanged += CheckBirthday;
            password.TextChanged += CheckPasswords;
            passwordConfirm.TextChanged += CheckPasswords;

            phoneNumber.Click += RemoveError;
            firstName.Click += RemoveError;
            lastName.Click += RemoveError;
            nationalId.Click += RemoveError;
            birthday.Click += RemoveError;
            password.Click += RemoveError;
            passwordConfirm.Click += RemoveError;

            phoneNumber.FocusChange += RemoveError;
            firstName.FocusChange += RemoveError;
            lastName.FocusChange += RemoveError;
            nationalId.FocusChange += RemoveError;
            birthday.FocusChange += RemoveError;
            password.FocusChange += RemoveError;
            passwordConfirm.FocusChange += RemoveError;

            loginButton.Click += Login;
            biometricButton.Click += BiometricLogin;
            registerButton.Click += Register;

            loginButton.MakeProgressButton(this);
            registerButton.MakeProgressButton(this);

            phoneNumber.Text = Database.User.PhoneNumber;
            if (Tools.HasBiometricAccess() && Database.UseBiometric && !Database.User.Password.IsEmpty()) {
                biometricButton.Visibility = ViewStates.Visible;
                BiometricAuth(new BiometricAuthenticationCallback(this));
                var constraintSet = new ConstraintSet();
                constraintSet.Clone(content);
                constraintSet.Clear(Resource.Id.loginButton, ConstraintSet.Start);
                constraintSet.Connect(Resource.Id.loginButton, ConstraintSet.Start, Resource.Id.biometricButton, ConstraintSet.End);
                constraintSet.ApplyTo(content);
            }
            else {
                var constraintSet = new ConstraintSet();
                constraintSet.Clone(content);
                constraintSet.Clear(Resource.Id.loginButton, ConstraintSet.Start);
                constraintSet.Connect(Resource.Id.loginButton, ConstraintSet.Start, Resource.Id.contentSpace, ConstraintSet.Start);
                constraintSet.ApplyTo(content);
            }
        }

        private class BiometricAuthenticationCallback : BiometricPrompt.AuthenticationCallback {
            private readonly LoginRegisterActivity loginRegisterActivity;

            public BiometricAuthenticationCallback(LoginRegisterActivity loginRegisterActivity) {
                this.loginRegisterActivity = loginRegisterActivity;
            }
            public override void OnAuthenticationError(int errorCode, Java.Lang.ICharSequence errString) {
                base.OnAuthenticationError(errorCode, errString);
                // Todo
            }
            public override void OnAuthenticationSucceeded(BiometricPrompt.AuthenticationResult result) {
                base.OnAuthenticationSucceeded(result);
                loginRegisterActivity.password.Text = Database.User.Password;
                loginRegisterActivity.loginButton.PerformClick();
            }
        }

        #region EndIcons
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

        #region Inputs
        private void RemoveError(object sender, EventArgs e) {
            var editText = sender as TextInputEditText;
            var editTextLayout = (TextInputLayout)editText.Parent.Parent;
            editTextLayout.ErrorEnabled = false;
            error.Visibility = ViewStates.Gone;
        }

        private void RemoveError(object sender, FocusChangeEventArgs e) {
            if (e.HasFocus) {
                var editText = sender as TextInputEditText;
                var editTextLayout = (TextInputLayout)editText.Parent.Parent;
                editTextLayout.ErrorEnabled = false;
                error.Visibility = ViewStates.Gone;
            }
        }

        private void RemoveError(object sender, TextChangedEventArgs e) {
            var editText = sender as TextInputEditText;
            var editTextLayout = (TextInputLayout)editText.Parent.Parent;
            editTextLayout.ErrorEnabled = false;
            error.Visibility = ViewStates.Gone;
        }

        private void CheckIsEditTextEmpty(params TextInputEditText[] editTexts) {
            foreach (var editText in editTexts) {
                var editTextLayout = (TextInputLayout)editText.Parent.Parent;
                if (editText.Text == "") {
                    editTextLayout.Error = "فیلد الزامی است";
                }
            }
        }

        private void CheckPhoneNumber(object sender, TextChangedEventArgs e = null) {
            var editText = sender as TextInputEditText;
            var editTextLayout = (TextInputLayout)editText.Parent.Parent;
            if (editText.Text == "") {
                editTextLayout.ErrorEnabled = false;
                return;
            }
            var phoneNumberRegex = new Regex(@"^((\+)?98|0)?9\d{9}$");
            if (phoneNumberRegex.IsMatch(editText.Text)) {
                editTextLayout.ErrorEnabled = false;
            }
            else {
                editTextLayout.Error = "شماره نامعتبر، شماره باید به صورت 09xxxxxxxxx باشد";
            }
        }

        private void CheckNationalId(object sender, TextChangedEventArgs e = null) {
            var editText = sender as TextInputEditText;
            var editTextLayout = (TextInputLayout)editText.Parent.Parent;
            if (editText.Text == "") {
                editTextLayout.ErrorEnabled = false;
                return;
            }
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
            if (editText.Text == "") {
                editTextLayout.ErrorEnabled = false;
                return;
            }
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
            if (password.Text == "" || passwordConfirm.Text == "") {
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

        private void Login(object sender, EventArgs e) {
            error.Visibility = ViewStates.Gone;
            CheckPhoneNumber(phoneNumber);
            CheckIsEditTextEmpty(phoneNumber, password);
            if (phoneNumberLayout.ErrorEnabled || passwordLayout.ErrorEnabled) {
                return;
            }
            new Thread(() => {
                RunOnUiThread(() => {
                    loginButton.ShowProgress();
                });
                try {
                    Server.User.Login(phoneNumber.Text, password.Text);
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                }
                catch (RpcException ex) {
                    Tools.ShowError(ex.GetMessage(), Configs.ErrorDuration, this);
                }
                catch {
                    Tools.ShowError("خطای داخلی برنامه", Configs.ErrorDuration, this);
                }
                RunOnUiThread(() => {
                    loginButton.HideProgress(Resource.String.login_button);
                });
            }).Start();
        }

        private void BiometricLogin(object sender, EventArgs e) {
            BiometricAuth(new BiometricAuthenticationCallback(this));
        }

        private void Register(object sender, EventArgs e) {
            error.Visibility = ViewStates.Gone;
            CheckPhoneNumber(phoneNumber);
            CheckNationalId(nationalId);
            CheckBirthday(birthday);
            CheckPasswords();
            CheckIsEditTextEmpty(phoneNumber, firstName, lastName, nationalId, birthday, password, passwordConfirm);
            if (phoneNumberLayout.ErrorEnabled || firstNameLayout.ErrorEnabled || lastNameLayout.ErrorEnabled || nationalIdLayout.ErrorEnabled || birthdayLayout.ErrorEnabled || passwordLayout.ErrorEnabled || passwordConfirmLayout.ErrorEnabled) {
                return;
            }
            new Thread(() => {
                RunOnUiThread(() => {
                    registerButton.ShowProgress();
                });
                try {
                    //var dateRegex = new Regex(@"^(\d{4})/(\d{1,2})/(\d{1,2})$");
                    //var (birthdayYear, birthdayMonth, birthdayDay) = dateRegex.Match(birthday.Text).Groups.Skip(1).Select(item => item.Value.ToInt32()).ToArray();
                    //var birthdayDate = new JDateTime(birthdayYear, birthdayMonth, birthdayDay);
                    //var birthdayDate = JDateTime.Parse(birthday.Text);
                    //var utcOffset = TimeZoneInfo.Utc.GetUtcOffset(birthdayDate);
                    //birthdayDate += utcOffset;
                    //Database.BirthdayUtcOffset = utcOffset;
                    Server.User.Register(phoneNumber.Text, firstName.Text, lastName.Text, nationalId.Text, JDateTime.Parse(birthday.Text), password.Text);
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                }
                catch (RpcException ex) {
                    Tools.ShowError(ex.GetMessage(), Configs.ErrorDuration, this);
                }
                catch {
                    Tools.ShowError("خطای داخلی برنامه", Configs.ErrorDuration, this);
                }
                RunOnUiThread(() => {
                    registerButton.HideProgress(Resource.String.register_button);
                });
            }).Start();
        }
        #endregion

        #region Tabs
        private void LoginRegisterTabSelected(object sender, TabSelectedEventArgs e) {
            switch (e.Tab.Position) {
                case 0:
                    firstNameLayout.Visibility = ViewStates.Gone;
                    lastNameLayout.Visibility = ViewStates.Gone;
                    nationalIdLayout.Visibility = ViewStates.Gone;
                    birthdayLayout.Visibility = ViewStates.Gone;
                    passwordConfirmLayout.Visibility = ViewStates.Gone;

                    phoneNumberLayout.ErrorEnabled = false;
                    passwordLayout.ErrorEnabled = false;

                    firstName.Text = "";
                    lastName.Text = "";
                    nationalId.Text = "";
                    birthday.Text = "";
                    passwordConfirm.Text = "";

                    password.ImeOptions = ImeAction.Done;

                    loginButton.Visibility = ViewStates.Visible;
                    registerButton.Visibility = ViewStates.Invisible;
                    error.Visibility = ViewStates.Gone;
                    forgotPassword.Visibility = ViewStates.Visible;

                    if (Tools.HasBiometricAccess() && Database.UseBiometric && !Database.User.Password.IsEmpty()) {
                        biometricButton.Visibility = ViewStates.Visible;
                        var constraintSet = new ConstraintSet();
                        constraintSet.Clone(content);
                        constraintSet.Clear(Resource.Id.loginButton, ConstraintSet.Start);
                        constraintSet.Connect(Resource.Id.loginButton, ConstraintSet.Start, Resource.Id.biometricButton, ConstraintSet.End);
                        constraintSet.ApplyTo(content);
                    }
                    else {
                        var constraintSet = new ConstraintSet();
                        constraintSet.Clone(content);
                        constraintSet.Clear(Resource.Id.loginButton, ConstraintSet.Start);
                        constraintSet.Connect(Resource.Id.loginButton, ConstraintSet.Start, Resource.Id.contentSpace, ConstraintSet.Start);
                        constraintSet.ApplyTo(content);
                    }

                    return;
                case 1:
                    firstNameLayout.Visibility = ViewStates.Visible;
                    lastNameLayout.Visibility = ViewStates.Visible;
                    nationalIdLayout.Visibility = ViewStates.Visible;
                    birthdayLayout.Visibility = ViewStates.Visible;
                    passwordConfirmLayout.Visibility = ViewStates.Visible;

                    phoneNumberLayout.ErrorEnabled = false;
                    firstNameLayout.ErrorEnabled = false;
                    lastNameLayout.ErrorEnabled = false;
                    nationalIdLayout.ErrorEnabled = false;
                    birthdayLayout.ErrorEnabled = false;
                    passwordLayout.ErrorEnabled = false;
                    passwordConfirmLayout.ErrorEnabled = false;

                    password.ImeOptions = ImeAction.Next;

                    registerButton.Visibility = ViewStates.Visible;
                    loginButton.Visibility = ViewStates.Invisible;
                    biometricButton.Visibility = ViewStates.Gone;
                    error.Visibility = ViewStates.Gone;
                    forgotPassword.Visibility = ViewStates.Gone;
                    return;
            }
        }
        #endregion

        #region Design
        private void RemoveTabsTooltips(object sender, TabSelectedEventArgs e) {
            for (int i = 0; i < loginRegisterTabLayout.TabCount; i++) {
                TooltipCompat.SetTooltipText(loginRegisterTabLayout.GetTabAt(i).View, (string)null);
            }
        }
        #endregion

        #region Activity
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
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.App;
using client.Classes;
using client.Fragments;
using Firebase.Auth;
using Firebase.Firestore.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using ID.IonBit.IonAlertLib;
using System;
using System.Net.Http;
using Xamarin.Essentials;

namespace client.Activities
{
    [Activity(Label = "Login", NoHistory = true)]
    public class Login : AppCompatActivity//, IOnSuccessListener, IOnFailureListener, IOnCompleteListener
    {
        //firebase auth

        /*Dialog*/
        /* private AlertDialog PasswordDialog;
         private AlertDialog.Builder dialogBuilder;

         private FloatingActionButton BtnCloseDialog;
         private TextInputEditText ResetInputEmail;
         private MaterialButton BtnReset;
         private int EventType;*/
        /**/
        private MaterialButton BtnLogin;
        private TextView TxtSignUp;
        private TextView TxtForgotPassword;
        private TextInputEditText InputEmail;
        private TextInputEditText InputPassword;

        /*root layout*/



        //****Retriving user information



        private string UserEmail;
        private IonAlert loadingDialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_login);
            ConnectViews();
            RequestedOrientation = ScreenOrientation.Portrait;

        }
        private void ConnectViews()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            UserEmail = pref.GetString("Email", string.Empty);



            BtnLogin = FindViewById<MaterialButton>(Resource.Id.BtnLogin);
            TxtSignUp = FindViewById<TextView>(Resource.Id.TxtCreateAccount);
            TxtForgotPassword = FindViewById<TextView>(Resource.Id.TxtForgotPassword);
            InputEmail = FindViewById<TextInputEditText>(Resource.Id.LoginInputEmail);
            InputPassword = FindViewById<TextInputEditText>(Resource.Id.LoginInputPassword);
            /////user infor
            //
            InputEmail.Text = UserEmail;
            BtnLogin.Click += BtnLogin_Click;
            TxtSignUp.Click += TxtSignUp_Click;
            TxtForgotPassword.Click += TxtForgotPassword_Click;
        }
        private void TxtForgotPassword_Click(object sender, EventArgs e)
        {
            ForgotPasswordDlgFragment forgotPasswordDlgFragment = new ForgotPasswordDlgFragment();
            forgotPasswordDlgFragment.Show(SupportFragmentManager.BeginTransaction(), "");
        }
        private void TxtSignUp_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(Register));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(InputEmail.Text) && string.IsNullOrWhiteSpace(InputEmail.Text))
            {
                Toast.MakeText(this, "Please provide your email", ToastLength.Long).Show();
                return;
            }
            if (string.IsNullOrEmpty(InputPassword.Text) && string.IsNullOrWhiteSpace(InputPassword.Text))
            {
                Toast.MakeText(this, "Please provide password", ToastLength.Long).Show();
                return;
            }
            BtnLogin.Enabled = false;

            loadingDialog = new IonAlert(this, IonAlert.ProgressType);
            loadingDialog.SetSpinKit("DoubleBounce")
                .SetSpinColor("#008D91")
                .ShowCancelButton(false)
                .Show();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    UserLogin userLogin = new UserLogin()
                    {
                        Email = InputEmail.Text.Trim(),
                        Password = InputPassword.Text.Trim(),
                    };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);
                    HttpClient httpClient = new HttpClient();
                    HttpContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync($"{API.ApiUrl}/account/login", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var user = Newtonsoft.Json.JsonConvert.DeserializeObject<AppUsers>(result);
                        Preferences.Set("Id", user.Id);
                        Preferences.Set("e", user.Email);
                        Preferences.Set("p", userLogin.Password);
                        Intent intent = new Intent(Application.Context, typeof(MainActivity));
                        StartActivity(intent);
                    }
                    else
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        AndHUD.Shared.ShowError(this, result, MaskType.Clear, TimeSpan.FromSeconds(3));
                    }
                }
                catch (Exception ex)
                {
                    AndHUD.Shared.ShowError(this, ex.Message, MaskType.Clear, TimeSpan.FromSeconds(3));
                }
                finally
                {
                    loadingDialog.Dismiss();
                }
            });



        }
        private void ResetPasswordDialog()
        {

            /*dialogBuilder = new AlertDialog.Builder(this);
            LayoutInflater inflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.reset_password_dialog, null);

            ResetInputEmail = view.FindViewById<TextInputEditText>(Resource.Id.ResetInputEmail);
            BtnReset = view.FindViewById<MaterialButton>(Resource.Id.BtnReset);
            BtnCloseDialog = view.FindViewById<FloatingActionButton>(Resource.Id.FabCloseResetDialog);*/
            /* BtnCloseDialog.Click += BtnCloseDialog_Click;
             BtnReset.Click += BtnReset_Click;*/
            /*  dialogBuilder.SetView(view);
              dialogBuilder.SetCancelable(false);
              PasswordDialog = dialogBuilder.Create();
              PasswordDialog.Show();*/
        }

        /*private void BtnReset_Click(object sender, EventArgs e)
        {
            EventType = 2;
            if (string.IsNullOrEmpty(ResetInputEmail.Text))
            {
                ResetInputEmail.Error = "Please provide your email address";//, ToastLength.Long).Show();
                //ResetInputEmail.RequestFocus();
                return;
            }
            if (string.IsNullOrEmpty(InputEmail.Text))
            {
                InputEmail.Error = "provide your email";
                return;
            }

            UserLogin userLogin = new UserLogin()
            {
                Email = InputEmail.Text.Trim(),
                Password = InputNewPassword.Text.Trim()
            };
            try
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync($"{API.ApiUrl}/account/resetpassword", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var results = await response.Content.ReadAsStringAsync();
                    AndHUD.Shared.ShowSuccess(context, $"{results}", MaskType.Black, TimeSpan.FromSeconds(2));
                    Dismiss();
                }
                else
                {
                    var results = await response.Content.ReadAsStringAsync();
                    AndHUD.Shared.ShowError(context, $"{results}", MaskType.Black, TimeSpan.FromSeconds(2));
                }
            }
            catch (Exception ex)
            {
                AndHUD.Shared.ShowError(context, $"{ex.Message}", MaskType.Black, TimeSpan.FromSeconds(2));

            }
            finally
            {
                loadingDialog.Dismiss();
            }
        };*/
    }
    /*    private void BtnCloseDialog_Click(object sender, EventArgs e)
        {
            rootLayout.Alpha = 1f;
            PasswordDialog.Dismiss();
            //  PasswordDialog.Dispose();
        }*/
    /*    public void OnSuccess(Java.Lang.Object result)
        {
            if (EventType == 1)
            {
                BtnLogin.Enabled = true;

                Intent intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);

                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);

            }
            if (EventType == 2)
            {
                //loading.Dismiss();
                //    loading.Dispose();
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Sent");
                builder.SetMessage("Reset password link has been sent to your email address");
                // builder.SetMessage(auth.CurrentUser.DisplayName);
                builder.SetNeutralButton("OK", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }

        }*/
    /*public void OnFailure(Java.Lang.Exception e)
    {

        BtnLogin.Enabled = true;

        //  loading.Dispose();
        AlertDialog.Builder builder = new AlertDialog.Builder(this);
        builder.SetTitle("Error");
        builder.SetMessage(e.Message);
        builder.SetNeutralButton("OK", delegate
        {
            builder.Dispose();
        });
        builder.Show();
    }

    public void OnComplete(Task task)
    {
        loadingDialog.Dismiss();
    }*/

}
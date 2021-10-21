using admin.Models;
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;

namespace admin.Activities
{
    [Activity(Label = "Login", NoHistory = true)]
    public class Login : Activity, IOnSuccessListener, IOnFailureListener
    {
        //firebase auth
        FirebaseAuth auth;

        //loading progress dialog
        private AlertDialog loading;
        private AlertDialog.Builder loadingBuilder;


        /*Dialog*/
        private AlertDialog PasswordDialog;
        private AlertDialog.Builder dialogBuilder;

        private FloatingActionButton BtnCloseDialog;
        private TextInputEditText ResetInputEmail;
        private MaterialButton BtnReset;
        private int EventType;
        /**/
        private Button BtnLogin;
        //private TextView TxtSignUp;
        private TextView TxtForgotPassword;
        private TextInputEditText InputEmail;
        private TextInputEditText InputPassword;

        /*root layout*/
        private RelativeLayout rootLayout;



        //****Retriving user information



        private string UserEmail;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.activity_login);
            ConnectViews();
        }
        private void ConnectViews()
        {
            ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            UserEmail = pref.GetString("Email", string.Empty);
            BtnLogin = FindViewById<MaterialButton>(Resource.Id.BtnLogin);
            // TxtSignUp = FindViewById<TextView>(Resource.Id.TxtCreateAccount);
            TxtForgotPassword = FindViewById<TextView>(Resource.Id.TxtForgotPassword);
            InputEmail = FindViewById<TextInputEditText>(Resource.Id.LoginInputEmail);
            InputPassword = FindViewById<TextInputEditText>(Resource.Id.LoginInputPassword);
            rootLayout = FindViewById<RelativeLayout>(Resource.Id.rootLayout);
            /////user infor
            //
            InputEmail.Text = UserEmail;
            BtnLogin.Click += BtnLogin_Click;
            //TxtSignUp.Click += TxtSignUp_Click;
            TxtForgotPassword.Click += TxtForgotPassword_Click;
        }
        private void TxtForgotPassword_Click(object sender, EventArgs e)
        {
            ResetPasswordDialog();
            rootLayout.Alpha = 0.5f;
        }
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            EventType = 1;

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
            auth = FirebaseAuth.Instance;
            auth.SignInWithEmailAndPassword(InputEmail.Text.Trim(), InputPassword.Text.Trim())
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this);
            LoadingProgress();
        }
        private void ResetPasswordDialog()
        {

            dialogBuilder = new AlertDialog.Builder(this);
            LayoutInflater inflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.reset_password_dialog, null);

            ResetInputEmail = view.FindViewById<TextInputEditText>(Resource.Id.ResetInputEmail);
            BtnReset = view.FindViewById<MaterialButton>(Resource.Id.BtnReset);
            BtnCloseDialog = view.FindViewById<FloatingActionButton>(Resource.Id.FabCloseResetDialog);
            BtnCloseDialog.Click += BtnCloseDialog_Click;
            BtnReset.Click += BtnReset_Click;
            dialogBuilder.SetView(view);
            dialogBuilder.SetCancelable(false);
            PasswordDialog = dialogBuilder.Create();
            PasswordDialog.Show();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            EventType = 2;
            if (string.IsNullOrEmpty(ResetInputEmail.Text))
            {
                Toast.MakeText(this, "Please provide your email address", ToastLength.Long).Show();
                ResetInputEmail.RequestFocus();
                return;
            }
            auth = FirebaseAuth.Instance;
            auth.SendPasswordResetEmail(ResetInputEmail.Text.Trim())
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this);
            LoadingProgress();
        }

        private void LoadingProgress()
        {
            loadingBuilder = new AlertDialog.Builder(this);
            LayoutInflater inflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.loading_progress, null);
            loadingBuilder.SetView(view);
            loadingBuilder.SetCancelable(false);
            loading = loadingBuilder.Create();
            loading.Show();
        }
        private void BtnCloseDialog_Click(object sender, EventArgs e)
        {
            rootLayout.Alpha = 1f;
            PasswordDialog.Dismiss();
            //  PasswordDialog.Dispose();
        }
        private async void GetUserInfo()
        {

            var results = await CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(FirebaseAuth.Instance.Uid)
                .GetAsync();

            if (results.Exists)
            {
                var user = results.ToObject<AppUsers>();
                if (user.Role == "A")
                {
                    BtnLogin.Enabled = true;
                    loading.Dismiss();
                    Intent intent = new Intent(this, typeof(Dashboad));
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);

                }
                else
                {
                    BtnLogin.Enabled = true;
                    loading.Dismiss();
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Error");
                    builder.SetMessage("Your not registered as a administrator");
                    builder.SetNeutralButton("OK", delegate
                    {
                        builder.Dispose();
                        FirebaseAuth.Instance.SignOut();
                    });
                    builder.Show();

                }
            }
            else
            {
                FirebaseAuth.Instance.SignOut();
            }



        }
        public void OnSuccess(Java.Lang.Object result)
        {
            if (EventType == 1)
            {
                GetUserInfo();
                EventType = 0;
            }
            if (EventType == 2)
            {
                loading.Dismiss();
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
                EventType = 0;
            }

        }
        public void OnFailure(Java.Lang.Exception e)
        {
            loading.Dismiss();
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


    }
}
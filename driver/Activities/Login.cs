using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using driver.Models;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;

namespace driver.Activities
{
    [Activity(Label = "Login", NoHistory = true)]
    public class Login : Activity, IOnSuccessListener, IOnFailureListener, IOnCompleteListener
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
        private MaterialButton BtnLogin;

        private TextView TxtForgotPassword;
        private TextInputEditText InputEmail;
        private TextInputEditText InputPassword;

        /*root layout*/
        private RelativeLayout rootLayout;
        private TextView TxtCreateAccount;


        //****Retriving user information



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




            BtnLogin = FindViewById<MaterialButton>(Resource.Id.BtnLogin);
            TxtForgotPassword = FindViewById<TextView>(Resource.Id.TxtForgotPassword);
            InputEmail = FindViewById<TextInputEditText>(Resource.Id.LoginInputEmail);
            InputPassword = FindViewById<TextInputEditText>(Resource.Id.LoginInputPassword);
            TxtCreateAccount = FindViewById<TextView>(Resource.Id.TxtCreateAccount);
            rootLayout = FindViewById<RelativeLayout>(Resource.Id.rootLayout);
            /////user infor
            //
            BtnLogin.Click += BtnLogin_Click;
            TxtCreateAccount.Click += TxtCreateAccount_Click;
            TxtForgotPassword.Click += TxtForgotPassword_Click;
        }

        private void TxtCreateAccount_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(Signup));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
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

            auth = FirebaseAuth.Instance;
            auth.SignInWithEmailAndPassword(InputEmail.Text, InputPassword.Text)
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this)
                .AddOnCompleteListener(this);
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
            auth.SendPasswordResetEmail(ResetInputEmail.Text)
                .AddOnSuccessListener(this)
                .AddOnFailureListener(this)
                .AddOnCompleteListener(this);
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

        }
        public void OnSuccess(Java.Lang.Object result)
        {

            if (EventType == 1)
            {
                GetUserInfo();

            }
            if (EventType == 2)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Sent");
                builder.SetMessage("Reset password link has been sent to your email address");
                //builder.SetMessage(auth.CurrentUser.DisplayName);
                builder.SetNeutralButton("Ok", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }

        }
        public void OnFailure(Java.Lang.Exception e)
        {
            loading.Dismiss();
            // loading.Dispose();
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Error");
            builder.SetMessage(e.Message);
            builder.SetNeutralButton("Ok", delegate
            {
                builder.Dispose();
            });
            builder.Show();
        }
        private async void GetUserInfo()
        {
            var query = await CrossCloudFirestore.Current.Instance
                .Collection("USERS")
                .Document(FirebaseAuth.Instance.Uid)
                .GetAsync();

            var user = query.ToObject<DriverModel>();
            if (user.Role is "D")
            {
                Intent intent = new Intent(this, typeof(Dashboad));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                Finish();
            }
            else
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage("Your not registered as a driver");
                builder.SetNeutralButton("Ok", delegate
                {
                    builder.Dispose();
                    // auth.SignOut();
                });
                builder.Show();
                FirebaseAuth.Instance.SignOut();
            }
            

        }
        public void OnComplete(Task task)
        {
            loading.Dismiss();
        }


    }
}
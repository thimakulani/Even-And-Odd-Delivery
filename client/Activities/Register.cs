using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using client.Classes;
using Firebase.Auth;
using Google.Android.Material.AppBar;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;
using System.IO;
using AlertDialog = Android.App.AlertDialog;

namespace client.Activities
{
    [Activity(Label = "Register", NoHistory = true)]
    public class Register : AppCompatActivity, IOnCompleteListener, IOnSuccessListener, IOnFailureListener
    {
        private MaterialButton BtnSubmitReg;
        private CheckBox Terms;
        //fire base 

        private TextInputEditText InputName;
        private TextInputEditText InputEmail;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhone;
        private TextView RegTxtTerms;
        private TextInputEditText InputConfirmPassword;
        private TextInputEditText InputPassword;


        //loading progress dialog
        private AlertDialog loading;
        private AlertDialog.Builder loadingBuilder;
        //*****
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_sign_up);
            ConnectViews();
            RequestedOrientation = ScreenOrientation.Portrait;
        }
        private void ConnectViews()
        {
            InputName = FindViewById<TextInputEditText>(Resource.Id.RegisterInputFirstName);
            InputSurname = FindViewById<TextInputEditText>(Resource.Id.RegisterInputLastName);
            InputPhone = FindViewById<TextInputEditText>(Resource.Id.RegisterInputPhoneNumber);
            InputEmail = FindViewById<TextInputEditText>(Resource.Id.RegisterInputEmail);
            RegTxtTerms = FindViewById<TextView>(Resource.Id.RegTxtTerms);
            InputPassword = FindViewById<TextInputEditText>(Resource.Id.RegisterInputPassword);
            InputConfirmPassword = FindViewById<TextInputEditText>(Resource.Id.RegisterInputPassword2);
            BtnSubmitReg = FindViewById<MaterialButton>(Resource.Id.BtnRegister);
            Terms = FindViewById<CheckBox>(Resource.Id.RegTerms);
            MaterialToolbar toolbar = FindViewById<MaterialToolbar>(Resource.Id.toolbar1);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            toolbar.Title = string.Empty;


            BtnSubmitReg.Click += BtnSubmitReg_Click;
            RegTxtTerms.Click += RegTxtTerms_Click;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (Android.Resource.Id.Home == item.ItemId)
            {
                Intent intent = new Intent(this, typeof(Login));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
            }

            return base.OnOptionsItemSelected(item);
        }
        public override void OnBackPressed()
        {
            // base.OnBackPressed();
            Intent intent = new Intent(this, typeof(Login));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
        }
        private void ImgBack_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(Login));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
        }

        private void RegTxtTerms_Click(object sender, EventArgs e)
        {

            TermsAndCoditions();

        }

        private void BtnSubmitReg_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(InputName.Text) && string.IsNullOrWhiteSpace(InputName.Text))
            {
                InputName.RequestFocus();
                InputName.Error = "provide your name";
                return;
            }
            if (string.IsNullOrEmpty(InputSurname.Text) && string.IsNullOrWhiteSpace(InputSurname.Text))
            {

                InputSurname.RequestFocus();
                InputSurname.Error = "provide your last name";
                return;
            }
            if (string.IsNullOrEmpty(InputPhone.Text) && string.IsNullOrWhiteSpace(InputPhone.Text))
            {
                InputPhone.RequestFocus();
                InputPhone.Error = "provide your phone numbers";
                return;
            }
            if (string.IsNullOrEmpty(InputEmail.Text) && string.IsNullOrWhiteSpace(InputEmail.Text))
            {
                InputEmail.RequestFocus();
                InputEmail.Error = "provide your email address";
                return;
            }
            if (string.IsNullOrEmpty(InputPassword.Text) && string.IsNullOrWhiteSpace(InputPassword.Text))
            {
                InputPassword.RequestFocus();
                InputPassword.Error = "provide your password";
                return;
            }
            if (string.IsNullOrEmpty(InputConfirmPassword.Text) && string.IsNullOrWhiteSpace(InputConfirmPassword.Text))
            {
                InputConfirmPassword.RequestFocus();
                InputConfirmPassword.Text = "confirm your password";
                return;
            }
            if (InputPassword.Text != InputConfirmPassword.Text)
            {
                InputConfirmPassword.RequestFocus();
                InputConfirmPassword.Error = "password does not match";
                return;
            }

            if (Terms.Checked)
            {
                LoadingProgress();
                BtnSubmitReg.Enabled = false;
                FirebaseAuth auth = FirebaseAuth.Instance;
                auth.CreateUserWithEmailAndPassword(InputEmail.Text.Trim(), InputPassword.Text.Trim())
                    .AddOnSuccessListener(this)
                    .AddOnFailureListener(this)
                    .AddOnCompleteListener(this);
            }
            else
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Terms and Conditions");
                builder.SetMessage("Please accept the terms and conditions");
                builder.SetNeutralButton("OK", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }

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


        public void OnComplete(Task task)
        {
            loading.Dismiss();
            BtnSubmitReg.Enabled = true;
        }



        /*Terms and condition*/

        private AlertDialog TermsDialog;
        private AlertDialog.Builder dialogBuilder;


        private TextView Parag1;

        private MaterialButton BtnAcceptTerms;

        private void TermsAndCoditions()
        {
            dialogBuilder = new AlertDialog.Builder(this);
            LayoutInflater inflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
            View view = inflater.Inflate(Resource.Layout.terms_and_conditions, null);
            Parag1 = view.FindViewById<TextView>(Resource.Id.terms);
            BtnAcceptTerms = view.FindViewById<MaterialButton>(Resource.Id.BtnAcceptTerms);
            BtnAcceptTerms.Click += BtnAcceptTerms_Click;
            AssetManager assets = this.Assets;
            using (StreamReader sr = new StreamReader(assets.Open("terms.txt")))
            {
                Parag1.Text = sr.ReadToEnd();
            }
            dialogBuilder.SetView(view);
            dialogBuilder.SetCancelable(true);
            TermsDialog = dialogBuilder.Create();
            TermsDialog.Show();
        }

        private void BtnAcceptTerms_Click(object sender, EventArgs e)
        {
            TermsDialog.Dismiss();
            TermsDialog.Dispose();
        }

        public void OnFailure(Java.Lang.Exception e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Error");
            builder.SetMessage(e.Message);
            builder.SetNeutralButton("OK", delegate
            {
                builder.Dispose();
            });
            builder.Show();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            AppUsers user = new AppUsers()
            {
                Color = null,
                Email = InputEmail.Text,
                Make = null,
                Name = InputName.Text,
                Phone = InputPhone.Text,
                RegNo = null,
                Role = "C",
                Surname = InputSurname.Text,
                Type = null,
                Uid = FirebaseAuth.Instance.Uid
            };
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(FirebaseAuth.Instance.Uid)
                .SetAsync(user);
            

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Successful");
            builder.SetMessage("Your account has been successfully created please proceed to login");
            builder.SetNeutralButton("OK", delegate
            {


                builder.Dispose();
                Intent intent = new Intent(this, typeof(Login));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
                Finish();

            });
            builder.Show();
        }
    }
}
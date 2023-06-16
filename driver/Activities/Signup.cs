using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Gms.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.App;
using driver.Models;
using Firebase.Auth;
using Google.Android.Material.AppBar;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;
using System.IO;
using AlertDialog = Android.App.AlertDialog;

namespace driver.Activities
{
    [Activity(Label = "Signup")]
    public class Signup : AppCompatActivity, IOnSuccessListener, IOnFailureListener, IOnCompleteListener
    {
        private TextInputEditText InputName;
        private TextInputEditText InputEmail;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhone;
        private TextView RegTxtTerms;
        private TextInputEditText InputConfirmPassword;
        private TextInputEditText InputPassword;
        private TextInputEditText InputRegNo;
        private TextInputEditText InputColor;
        private TextInputEditText InputMake;
        private MaterialButton BtnSubmitReg;
        private MaterialButton BtnType;
        private CheckBox Terms;
        //fire base 

        //loading progress dialog
        private AlertDialog loading;
        private AlertDialog.Builder loadingBuilder;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
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


            InputMake = FindViewById<TextInputEditText>(Resource.Id.RegisterInputMake);
            InputRegNo = FindViewById<TextInputEditText>(Resource.Id.RegisterInputCarReg);
            InputColor = FindViewById<TextInputEditText>(Resource.Id.RegisterInputColor);





            BtnSubmitReg = FindViewById<MaterialButton>(Resource.Id.BtnRegister);
            BtnType = FindViewById<MaterialButton>(Resource.Id.BtnType);
            Terms = FindViewById<CheckBox>(Resource.Id.RegTerms);
            MaterialToolbar toolbar = FindViewById<MaterialToolbar>(Resource.Id.toolbar1);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);
            toolbar.Title = string.Empty;
            BtnType.Click += BtnType_Click;


            BtnSubmitReg.Click += BtnSubmitReg_Click;
            RegTxtTerms.Click += RegTxtTerms_Click;
        }

        private void BtnType_Click(object sender, EventArgs e)
        {
            Android.Widget.PopupMenu popupMenu = new Android.Widget.PopupMenu(this, BtnType);
            popupMenu.Menu.Add(IMenu.First, 1, 1, "Bike");
            popupMenu.Menu.Add(IMenu.First, 1, 1, "Bakkie");
            popupMenu.Menu.Add(IMenu.First, 1, 1, "Provate Car");
            popupMenu.Show();
            popupMenu.MenuItemClick += PopupMenu_MenuItemClick;
        }

        private void PopupMenu_MenuItemClick(object sender, Android.Widget.PopupMenu.MenuItemClickEventArgs e)
        {
            BtnType.Text = e.Item.TitleFormatted.ToString();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (Android.Resource.Id.Home == item.ItemId)
            {
                Intent intent = new Intent(this, typeof(Login));
                StartActivity(intent);
                Finish();
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



            if (string.IsNullOrEmpty(InputRegNo.Text) && string.IsNullOrWhiteSpace(InputRegNo.Text))
            {
                InputRegNo.RequestFocus();
                InputRegNo.Error = "provide your vehicle registration number";
                return;
            }
            if (string.IsNullOrEmpty(InputMake.Text) && string.IsNullOrWhiteSpace(InputMake.Text))
            {
                InputMake.RequestFocus();
                InputMake.Error = "provide your vehicle make";
                return;
            }
            if (string.IsNullOrEmpty(InputColor.Text) && string.IsNullOrWhiteSpace(InputColor.Text))
            {
                InputColor.RequestFocus();
                InputColor.Error = "provide your vehicle color";
                return;
            }
            if (BtnType.Text == "Type")
            {
                BtnType.Error = "Select car type";
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

        public void OnSuccess(Java.Lang.Object result)
        {
            //data.Put(.Role", "Driver");
            DriverModel driver = new DriverModel()
            {
                Color = InputColor.Text,
                Email = InputEmail.Text,
                Make = InputMake.Text,
                Name = InputName.Text,
                Phone = InputPhone.Text,
                RegNo = InputRegNo.Text,
                Role = null,
                Surname = InputSurname.Text,
                Type = BtnType.Text,
                Uid = FirebaseAuth.Instance.Uid,
                Car = new Car()
                {
                    Color = InputColor.Text,
                    Make = InputMake.Text,
                    RegNo = InputRegNo.Text,
                    Type = BtnType.Text,
                }

            };

            CrossCloudFirestore.Current
                .Instance
                .Collection("USERS")
                .Document(FirebaseAuth.Instance.Uid)
                .SetAsync(driver);

            AndHUD.Shared.ShowSuccess(this, "Your account has been successfully created, Please wait for administrator to approve your registration.", MaskType.Black, TimeSpan.FromSeconds(10));

        }

        public void OnFailure(Java.Lang.Exception e)
        {
            AndHUD.Shared.ShowSuccess(this, e.Message, MaskType.Black, TimeSpan.FromSeconds(10));
        }


    }
}
public class Car
{
    public string Id { get; set; }
    public string RegNo { get; set; }
    public string Type { get; set; }
    public string Color { get; set; }
    public string Make { get; set; }
    public string UserId { get; set; }

}
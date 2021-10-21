using admin.FirebaseHelper;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FirebaseAdmin.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace admin.Fragments
{
    public class DriverRegistrationDialog : DialogFragment
    {
        private TextInputEditText InputName;
        private TextInputEditText InputEmail;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhone;
        private TextInputEditText InputColor;
        private TextInputEditText InputRegNo;
        private TextInputEditText InputMake;
        private MaterialButton BtnType;
        private MaterialButton BtnSubmitReg;
        private Context context;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            base.OnCreateView(inflater, container, savedInstanceState);

            View view = inflater.Inflate(Resource.Layout.create_driver_dialog, container, false);
            ConnectViews(view);
            return view;

        }

        private void ConnectViews(View view)
        {
            context = view.Context;
            InputName = view.FindViewById<TextInputEditText>(Resource.Id.RegisterInputFirstName);
            InputSurname = view.FindViewById<TextInputEditText>(Resource.Id.RegisterInputLastName);
            InputPhone = view.FindViewById<TextInputEditText>(Resource.Id.RegisterInputPhoneNumber);
            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.RegisterInputEmail);
            InputMake = view.FindViewById<TextInputEditText>(Resource.Id.RegisterInputMake);
            InputColor = view.FindViewById<TextInputEditText>(Resource.Id.RegisterInputColor);
            InputRegNo = view.FindViewById<TextInputEditText>(Resource.Id.RegisterInputRegNo);
            BtnType = view.FindViewById<MaterialButton>(Resource.Id.BtnRegisterCarType);
            BtnSubmitReg = view.FindViewById<MaterialButton>(Resource.Id.BtnRegisterDriver);
            FloatingActionButton FabClose = view.FindViewById<FloatingActionButton>(Resource.Id.FabCloseDirverRegDialog);
            FabClose.Click += FabClose_Click;
            BtnSubmitReg.Click += BtnSubmitReg_Click;
            BtnType.Click += BtnType_Click;
        }

        private void BtnType_Click(object sender, EventArgs e)
        {
            PopupMenu popupMenu = new PopupMenu(context, BtnType);
            popupMenu.Menu.Add(IMenu.First, 1, 1, "Bike");
            popupMenu.Menu.Add(IMenu.First, 1, 1, "Bakkie");
            popupMenu.Menu.Add(IMenu.First, 1, 1, "Provate Car");
            popupMenu.Show();
            popupMenu.MenuItemClick += PopupMenu_MenuItemClick;
        }

        private void PopupMenu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            BtnType.Text = e.Item.TitleFormatted.ToString();
        }

        private void FabClose_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
        private async void BtnSubmitReg_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(InputName.Text) && string.IsNullOrWhiteSpace(InputName.Text))
            {
                //Toast.MakeText(this, "Please provide your name.", ToastLength.Long).Show();
                InputName.RequestFocus();
                InputName.Error = "required";
                return;
            }
            if (string.IsNullOrEmpty(InputSurname.Text) && string.IsNullOrWhiteSpace(InputSurname.Text))
            {
                InputSurname.RequestFocus();
                InputSurname.Error = "required";
                return;
            }
            if (string.IsNullOrEmpty(InputPhone.Text) && string.IsNullOrWhiteSpace(InputPhone.Text))
            {
                InputPhone.RequestFocus();
                InputPhone.Error = "required";
                return;
            }
            if (string.IsNullOrEmpty(InputEmail.Text) && string.IsNullOrWhiteSpace(InputEmail.Text))
            {
                InputEmail.RequestFocus();
                InputEmail.Error = "required";
                return;
            }
            if (string.IsNullOrEmpty(InputRegNo.Text) && string.IsNullOrWhiteSpace(InputRegNo.Text))
            {
                InputRegNo.RequestFocus();
                InputRegNo.Error = "required";
                return;
            }
            if (string.IsNullOrEmpty(InputMake.Text) && string.IsNullOrWhiteSpace(InputMake.Text))
            {
                InputMake.RequestFocus();
                InputMake.Error = "required";
                return;
            }
            if (string.IsNullOrEmpty(InputColor.Text) && string.IsNullOrWhiteSpace(InputColor.Text))
            {
                InputColor.RequestFocus();
                InputColor.Error = "required";
                return;
            }
            if (BtnType.Text == "Vehicle Type")
            {
                //Toast.MakeText(context, "Please provide your email address", ToastLength.Long).Show();
                BtnType.RequestFocus();
                BtnType.Error = "required";
                return;
            }
            var stream = Resources.Assets.Open("service_account.json");
            var _auth = FirebaseData.GetFirebaseAdminAuth(stream);

            UserRecordArgs user = new UserRecordArgs()
            {
                Email = InputEmail.Text.Trim(),
                Password = InputPhone.Text.Trim()
            };
            LoadingDialog loading = new LoadingDialog
            {
                Cancelable = false
            };
            loading.Show(ChildFragmentManager.BeginTransaction(), "Loading");
            try
            {
                var results = await _auth.CreateUserAsync(user);
                RegisterDriverInfor(results.Uid);
            }
            catch (Exception ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(context);
                builder.SetTitle("Error");
                builder.SetMessage(ex.Message);
                builder.SetNeutralButton("OK", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }
            finally
            {
                loading.Dismiss();
                //loading.Show();
            }
        }
        private async void RegisterDriverInfor(string uid)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "Name", InputName.Text },
                { "Phone", InputPhone.Text },
                { "Surname", InputSurname.Text },
                { "Email", InputEmail.Text },
                { "Type", BtnType.Text },
                { "Role", "D" },
                { "Make", InputMake.Text },
                { "RegNo", InputRegNo.Text },
                { "Color", InputColor.Text }
            };

            await CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(uid)
                .SetAsync(data);


            AndroidHUD.AndHUD.Shared.ShowSuccess(context, "Successfully registered a driver", AndroidHUD.MaskType.Black, TimeSpan.FromSeconds(2));
            Dismiss();

        }
    }
}
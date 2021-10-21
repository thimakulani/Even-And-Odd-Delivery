using admin.Models;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace admin.Fragments
{
    public class UpdateExistingUserFragment : DialogFragment
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
        private readonly AppUsers appUsers = new AppUsers();

        public UpdateExistingUserFragment(AppUsers appUsers)
        {
            this.appUsers = appUsers;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
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
            InputEmail.Enabled = false;
            FabClose.Click += FabClose_Click;
            BtnSubmitReg.Click += BtnSubmitReg_Click;
            BtnType.Click += BtnType_Click;
            SetInputs();



        }

        private void SetInputs()
        {
            InputName.Text = appUsers.Name;
            InputSurname.Text = appUsers.Surname;
            InputPhone.Text = appUsers.Phone;
            InputEmail.Text = appUsers.Email;
            InputMake.Text = appUsers.Make;
            InputRegNo.Text = appUsers.RegNo;
            InputColor.Text = appUsers.Color;
            if (!string.IsNullOrEmpty(appUsers.Type))
            {
                BtnType.Text = appUsers.Type;
            }
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
                BtnType.RequestFocus();
                BtnType.Error = "required";
                return;
            }

            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "Type", BtnType.Text },
                { "Make", InputMake.Text },
                { "RegNo", InputRegNo.Text },
                { "Color", InputColor.Text },
                { "Role", "D" }
            };

            await CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(appUsers.Uid)
                .UpdateAsync(data);


            AndroidHUD.AndHUD.Shared.ShowSuccess(context, "Successfully registered a driver", AndroidHUD.MaskType.Black, TimeSpan.FromSeconds(2));
            Dismiss();

        }

        private void FabClose_Click(object sender, EventArgs e)
        {
            Dismiss();
            Dispose();
        }
    }
}
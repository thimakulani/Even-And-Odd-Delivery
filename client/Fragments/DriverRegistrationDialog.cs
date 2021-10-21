using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using client.Classes;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace client.Fragments
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

            View view = inflater.Inflate(Resource.Layout.create_driver_dlg, container, false);
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
            CrossCloudFirestore.Current.Instance.Collection("USERS")
               .Document(FirebaseAuth.Instance.Uid)
               .AddSnapshotListener((snapshot, error) =>
               {
                   if (snapshot.Exists)
                   {
                       var user = snapshot.ToObject<AppUsers>();
                       InputName.Text = user.Name;
                       InputSurname.Text = user.Surname;
                       InputPhone.Text = user.Phone;
                       InputEmail.Text = user.Email;
                   }
               });
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
        private void BtnSubmitReg_Click(object sender, EventArgs e)
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
            RegisterDriverInfor();
        }
        private async void RegisterDriverInfor()
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
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
                .Document(FirebaseAuth.Instance.Uid)
                .UpdateAsync(data);


            AndroidHUD.AndHUD.Shared.ShowSuccess(context, "Successfully registered aS a driver", AndroidHUD.MaskType.Black, TimeSpan.FromSeconds(5));
            Dismiss();

        }

      
    }
}
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidHUD;
using driver.Models;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using AndroidX.Fragment.App;
using AndroidX.AppCompat.Widget;

namespace driver.Fragments
{
    public class ProfileFragment : Fragment
    {
        private TextInputEditText InputNames;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhone;
        private TextInputEditText InputEmail;

        private MaterialButton BtnAppyChanges;
        private Context context;

        //userkeyId
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view = inflater.Inflate(Resource.Layout.update_profile_dialog, container, false);
            //ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            context = view.Context;
            ConnectViews(view);
            return view;
        }
        AppCompatImageView img_edit;
        private void ConnectViews(View view)
        {
            InputNames = view.FindViewById<TextInputEditText>(Resource.Id.InputName);
            InputSurname = view.FindViewById<TextInputEditText>(Resource.Id.InputLastName);
            InputPhone = view.FindViewById<TextInputEditText>(Resource.Id.InputPhone);
            InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.InputEmail);
            img_edit = view.FindViewById<AppCompatImageView>(Resource.Id.img_edit);
            BtnAppyChanges = view.FindViewById<MaterialButton>(Resource.Id.btn_update);

            BtnAppyChanges.Click += BtnAppyChanges_Click;
            CrossCloudFirestore
                .Current
                .Instance.Collection("USERS")
               .Document(FirebaseAuth.Instance.Uid)
               .AddSnapshotListener((snapshot, error) =>
               {
                   if (snapshot.Exists)
                   {
                       var user = snapshot.ToObject<DriverModel>();
                       InputNames.Text = user.Name;
                       InputSurname.Text = user.Surname;
                       InputPhone.Text = user.Phone;
                       InputEmail.Text = user.Email;
                   }
               });
            ViewState(false);
            img_edit.Click += (s, e) =>
            {
                ViewState(true);
            };
        }
        private void ViewState(bool v)
        {
            InputEmail.Enabled = v;
            InputNames.Enabled = v;
            InputSurname.Enabled = v;
            InputPhone.Enabled = v;
            if (v)
            {
                BtnAppyChanges.Visibility = ViewStates.Visible;
                img_edit.Visibility = ViewStates.Gone;
            }
            else
            {
                BtnAppyChanges.Visibility = ViewStates.Gone;
                img_edit.Visibility = ViewStates.Visible;
            }
        }

        //public event EventHandler FailUpdateHandler; 
        private async void BtnAppyChanges_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputNames.Text))
            {
                InputNames.Error = "Name cannot be empty";
                return;
            }
            if (string.IsNullOrEmpty(InputSurname.Text))
            {
                InputSurname.Error = "Surname cannot be empty";
                return;
            }
            if (string.IsNullOrEmpty(InputPhone.Text))
            {
                InputPhone.Error = "Phone number cannot be empty";
                return;
            }
            Dictionary<string, object> keyValues = new Dictionary<string, object>
            {
                { "Name", InputNames.Text.Trim() },
                { "Phone", InputPhone.Text.Trim() },
                { "Surname", InputSurname.Text.Trim() }
            };
            await CrossCloudFirestore.Current
                .Instance
                .Collection("USERS")
                .Document(FirebaseAuth.Instance.Uid)
                .UpdateAsync(keyValues);

            ViewState(false);

            AndHUD.Shared.ShowSuccess(context, "Profile has been successfully updated!!", MaskType.Black, TimeSpan.FromSeconds(3));



        }



    }
}
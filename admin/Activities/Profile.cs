using admin.Models;
using Android.App;
using Android.OS;
using AndroidHUD;
using Firebase.Auth;
using Google.Android.Material.AppBar;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace admin.Activities
{
    [Activity(Label = "Profile")]
    public class Profile : Activity
    {
        private TextInputEditText InputNames;
        private TextInputEditText InputSurname;
        private TextInputEditText InputPhone;
        private TextInputEditText InputEmail;

        private MaterialButton BtnAppyChanges;

        private MaterialToolbar include_app_toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.update_profile_dialog);
            include_app_toolbar = FindViewById<MaterialToolbar>(Resource.Id.include_app_toolbar);
            InputNames = FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateName);
            InputSurname = FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateSurname);
            InputPhone = FindViewById<TextInputEditText>(Resource.Id.ProfileUpdatePhone);
            InputEmail = FindViewById<TextInputEditText>(Resource.Id.ProfileUpdateEmail);
            BtnAppyChanges = FindViewById<MaterialButton>(Resource.Id.BtnUpdateProfile);
            InputEmail.Enabled = false;
            BtnAppyChanges.Click += BtnAppyChanges_Click;

            //ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            CrossCloudFirestore.Current
                .Instance
                .Collection("USERS")
                .Document(FirebaseAuth.Instance.Uid)
                .AddSnapshotListener((value, error) =>
                {
                    if (value.Exists)
                    {
                        var user = value.ToObject<AppUsers>();
                        InputNames.Text = user.Name;
                        InputSurname.Text = user.Surname;
                        InputPhone.Text = user.Phone;
                        InputEmail.Text = user.Email;
                    }
                });
            include_app_toolbar.NavigationClick += Include_app_toolbar_NavigationClick;
        }

        private void Include_app_toolbar_NavigationClick(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }

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
            if (string.IsNullOrEmpty(InputEmail.Text))
            {
                InputEmail.Error = "Email cannot be empty";
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



            AndHUD.Shared.ShowSuccess(this, "Profile has been successfully updated!!", MaskType.Black, TimeSpan.FromSeconds(3));

        }
    }
}
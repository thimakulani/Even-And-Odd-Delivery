using Android.OS;
using Android.Views;
using AndroidX.Fragment.App;
using FirebaseAdmin.Messaging;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace admin.Fragments
{
    public class DialogAddAnnouncement : DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.add_announcement_dialog, container, false);
            ConnectView(view);
            return view;
        }
        private MaterialButton SubmitAnnouncement;
        private TextInputEditText InputMessage;
        private void ConnectView(View view)
        {
            SubmitAnnouncement = view.FindViewById<MaterialButton>(Resource.Id.dlgBtnSubmiAnnouncement);
            InputMessage = view.FindViewById<TextInputEditText>(Resource.Id.dlgInputAnnouncement);
            SubmitAnnouncement.Click += SubmitAnnouncement_Click;
        }
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
        private async void SubmitAnnouncement_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"TimeStamp", FieldValue.ServerTimestamp },
                {"Message",InputMessage.Text },

            };
            if (!string.IsNullOrEmpty(InputMessage.Text) && !string.IsNullOrWhiteSpace(InputMessage.Text))
            {
                await CrossCloudFirestore.Current
                    .Instance
                    .Collection("ANNOUNCEMENT")
                    .AddAsync(data);
            }
            var stream = Resources.Assets.Open("service_account.json");
            var fcm = new FirebaseHelper.FirebaseData().GetFirebaseMessaging(stream);
            FirebaseAdmin.Messaging.Message message = new FirebaseAdmin.Messaging.Message()
            {
                Topic = "A",
                Notification = new Notification()
                {
                    Title = "New Announcement",
                    Body = $"{InputMessage.Text}",
                    
                },
            };
            await fcm.SendAsync(message);
            InputMessage.Text = string.Empty;
        }
    }
}
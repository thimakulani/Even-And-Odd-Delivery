using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using driver.Models;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.TextView;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace driver.Dialogs
{
    public class RequestDialogFragment : DialogFragment
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
            View view = inflater.Inflate(Resource.Layout.request_dialog, container, false);
            ConnectViews(view);
            return view;
        }
        private MaterialTextView txt_name;
        private MaterialButton btn_accept;
        private MaterialButton btn_cancel;
        private MaterialTextView txt_dest;
        private MaterialTextView txt_pickup;
        private MaterialTextView txt_price;
        private MaterialTextView txt_distance;
        private DeliveryModal data;

        public RequestDialogFragment(DeliveryModal data)
        {
            this.data = data;

        }





        // private MaterialTextView txt_duration;
        private void ConnectViews(View view)
        {
            btn_accept = view.FindViewById<MaterialButton>(Resource.Id.btn_accept);
            btn_cancel = view.FindViewById<MaterialButton>(Resource.Id.btn_cancel);
            txt_dest = view.FindViewById<MaterialTextView>(Resource.Id.txt_destination);
            txt_distance = view.FindViewById<MaterialTextView>(Resource.Id.txt_distance);
            txt_name = view.FindViewById<MaterialTextView>(Resource.Id.txt_names);
            txt_pickup = view.FindViewById<MaterialTextView>(Resource.Id.txt_pickup);
            txt_price = view.FindViewById<MaterialTextView>(Resource.Id.txt_price);

            btn_accept.Click += Btn_accept_Click;
            btn_cancel.Click += Btn_cancel_Click;

            //var data = v.ToObject<DeliveryModal>();
            txt_dest.Text = $"{data.DestinationAddress.ToUpper()}";
            txt_distance.Text = $"{data.Distance}";
            txt_pickup.Text = $"{data.PickupAddress.ToUpper()}";
            txt_price.Text = $"{data.Price}";


            CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(data.UserId)
                .AddSnapshotListener((user, error) =>
                {
                    if (user.Exists)
                    {
                        var u = user.ToObject<ClientModel>();
                        txt_name.Text = $"{u.Name} {u.Surname}".ToUpper();
                    }
                });

        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }

        private async void Btn_accept_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> keyValuePairs = new Dictionary<string, object>
            {
                { "Status", "A" },
                { "DriverId", FirebaseAuth.Instance.CurrentUser.Uid }
            };
            var query = CrossCloudFirestore
                .Current
                .Instance
                .Collection("REQUESTS")
                .Document(data.KeyId);
            /*var _q = await query.GetAsync();*/

            await CrossCloudFirestore.Current.Instance.RunTransactionAsync(transaction =>
            {
                var doc = transaction.Get(query).ToObject<DeliveryModal>();
                if (doc.DriverId == null)
                {
                    transaction.Update(query, keyValuePairs);
                }

            });
            Dismiss();
        }

        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
    }
}
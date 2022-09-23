using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using driver.Models;
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
        private string id;

        public RequestDialogFragment(string id)
        {
            this.id = id;
        }

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
        private MaterialTextView txt_dest;
        private MaterialTextView txt_pickup;
        private MaterialTextView txt_price;
        private MaterialTextView txt_distance;
       // private MaterialTextView txt_duration;
        private void ConnectViews(View view)
        {
            txt_dest = view.FindViewById<MaterialTextView>(Resource.Id.txt_destination);
            txt_distance = view.FindViewById<MaterialTextView>(Resource.Id.txt_distance);
            txt_name = view.FindViewById<MaterialTextView>(Resource.Id.txt_names);
            txt_pickup = view.FindViewById<MaterialTextView>(Resource.Id.txt_pickup);
            txt_price = view.FindViewById<MaterialTextView>(Resource.Id.txt_price);


            CrossCloudFirestore
                .Current
                .Instance
                .Collection("REQUESTS")
                .Document(id)
                .AddSnapshotListener((v, e) =>
                {
                    if (v.Exists)
                    {
                        var data = v.ToObject<DeliveryModal>();
                        txt_dest.Text = $"{data.DestinationAddress.ToUpper()}";
                        txt_distance.Text = $"{data.Distance}";
                        txt_pickup.Text = $"{data.PickupAddress.ToUpper()}";

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
                        txt_price.Text = $"{data.Price}";
                    }
                });
        }

        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
    }
}
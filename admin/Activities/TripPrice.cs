using admin.Models;
using Android.App;
using Android.OS;
using AndroidHUD;
using Google.Android.Material.AppBar;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;

namespace admin.Activities
{
    [Activity(Label = "TripPrice")]
    public class TripPrice : Activity
    {
        private MaterialToolbar toolbar;
        private TextInputEditText InputPrice;
        private TextInputEditText InputPriceAfter;
        private MaterialButton BtnApplyChanges;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_trip_price);
            // Create your application here
            ConnectViews();
        }

        private void ConnectViews()
        {
            toolbar = FindViewById<MaterialToolbar>(Resource.Id.include_app_toolbar);
            InputPrice = FindViewById<TextInputEditText>(Resource.Id.InitialPrice);
            InputPriceAfter = FindViewById<TextInputEditText>(Resource.Id.InitialAfter);
            BtnApplyChanges = FindViewById<MaterialButton>(Resource.Id.BtnApplyChanges);
            BtnApplyChanges.Click += BtnApplyChanges_Click;

            toolbar.NavigationClick += Toolbar_NavigationClick1;
            CrossCloudFirestore
                 .Current
                 .Instance
                 .Collection("PRICE")
                 .Document("Price")
                 .AddSnapshotListener((snapshot, error) =>
                 {
                     if (snapshot.Exists && snapshot != null)
                     {
                         var price = snapshot.ToObject<PriceModel>();
                         InputPrice.Text = price.InitialPrice;
                         InputPriceAfter.Text = price.PriceAfter;
                     }
                 });
        }

        private void Toolbar_NavigationClick1(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }
        private async void BtnApplyChanges_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputPrice.Text))
            {
                InputPrice.Error = "Cannot be empty";
                return;
            }
            if (string.IsNullOrEmpty(InputPriceAfter.Text))
            {
                InputPriceAfter.Error = "Cannot be empty";
                return;
            }
            PriceModel price = new PriceModel()
            {
                PriceAfter = InputPriceAfter.Text,
                InitialPrice = InputPrice.Text
            };
            await CrossCloudFirestore
                 .Current
                 .Instance
                 .Collection("PRICE")
                 .Document("Price")
                 .SetAsync(price);
            AndHUD.Shared.ShowSuccess(this, "Price Updated", MaskType.Black, TimeSpan.FromSeconds(2));
        }


    }
}
using Android.OS;
using AndroidX.Fragment.App;
using Android.Views;
using Android.Widget;
using client.Classes;
using Google.Android.Material.FloatingActionButton;
using Plugin.CloudFirestore;
using System;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Google.Android.Material.AppBar;
using client.Activities;
using ID.IonBit.IonAlertLib;
using Android.Content;
using System.Collections.Generic;

namespace client.Fragments
{
    public class CompleteRequestDialog : DialogFragment
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
            View view = inflater.Inflate(Resource.Layout.complete_request_dialog, container, false);
            ConnectViews(view);
            return view;
        }
        private TextInputEditText InputItemType;
        private TextInputEditText InputPickUpLocation;
        private TextInputEditText InputDestinationLocation;
        private TextInputEditText InputPersonName;
        private TextInputEditText InputPersonContact;
        private MaterialButton BtnSubmitDeliveryRequest;
        private DeliveryModal deliveryModal;

        public CompleteRequestDialog(DeliveryModal deliveryModal)
        {
            this.deliveryModal = deliveryModal;
        }
        
        private void ConnectViews(View view) 
        {
            MaterialToolbar toolbar = view.FindViewById<MaterialToolbar>(Resource.Id.toolbar);
            InputItemType = view.FindViewById<TextInputEditText>(Resource.Id.InputPacelItemType);
            InputPickUpLocation = view.FindViewById<TextInputEditText>(Resource.Id.InputPacelPickupLocation);
            InputDestinationLocation = view.FindViewById<TextInputEditText>(Resource.Id.InputPacelDestination);
            InputPersonName = view.FindViewById<TextInputEditText>(Resource.Id.InputPacelPersonN);
            InputPersonContact = view.FindViewById<TextInputEditText>(Resource.Id.InputPacelPersonContacts);
            BtnSubmitDeliveryRequest = view.FindViewById<MaterialButton>(Resource.Id.BtnSubmitDeliveryRequest);


            InputPickUpLocation.Text = deliveryModal.PickupAddress;
            InputDestinationLocation.Text = deliveryModal.DestinationAddress;

            BtnSubmitDeliveryRequest.Click += async (sender, args) =>
            {
                if (string.IsNullOrEmpty(InputPersonName.Text) && string.IsNullOrWhiteSpace(InputPersonName.Text))
                {
                    //Toast.MakeText(this, "Please provide the name of the person you delivering to", ToastLength.Long).Show();
                    InputPersonName.Error = "Name could not be empty";
                    InputPersonName.RequestFocus();
                    return;
                }
                if (string.IsNullOrEmpty(InputPersonContact.Text) && string.IsNullOrWhiteSpace(InputPersonContact.Text))
                {
                    //Toast.MakeText(this, "Please provide the contact numbers of the person you delivering to", ToastLength.Long).Show();
                    InputPersonContact.Error = "Please provide phone number";
                    InputPersonContact.RequestFocus();

                    return;
                }
                if (string.IsNullOrEmpty(InputItemType.Text))
                {
                    InputItemType.Error = "Item Description";
                    InputItemType.RequestFocus();
                    return;
                }
                //find nearest driver
                FindNearestDriver();


                deliveryModal.PersonContact = InputPersonContact.Text;
                deliveryModal.PersonName = InputPersonName.Text;
                deliveryModal.Status = "W";
                deliveryModal.RequestTime = DateTime.Now.ToString("dddd, dd MMMM yyyy, HH:mm tt");
                var query = await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("REQUESTS")
                    .AddAsync(deliveryModal);
                Dictionary<string, object> keys = new Dictionary<string, object>();
                keys.Add("RequestId", query.Id);
                await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("UPCOMING")
                    .Document(tempDrivers[0].Driver_Id)
                    .SetAsync(keys);


                var dlg = new IonAlert(view.Context, IonAlert.SuccessType);
                dlg.SetTitleText("Success");
                dlg.SetContentText("Your request has been successfully made.");
                dlg.CancelEvent += (s, e) =>
                {

                };
                dlg.DismissEvent += (s, e) =>
                {
                    Dismiss();
                };
                dlg.Show();
                
            };
            toolbar.NavigationClick += (sender, args) =>
            {
                this.Dismiss();
            };

        }
        public event EventHandler CompleteHandler;
        private List<TempDriver> tempDrivers = new List<TempDriver>();
        private async void FindNearestDriver()
        {
            tempDrivers.Clear();
            var query = await CrossCloudFirestore
                 .Current.Instance
                 .Collection("USERS")
                 .WhereEqualsTo("Status", "Online")
                 .WhereEqualsTo("Role", "D").GetAsync();
            if(!query.IsEmpty)
            {
                var drivers = query.ToObjects<AppUsers>();
                foreach(var driver in drivers)
                {
                    var distance = Xamarin.Essentials
                        .LocationExtensions
                        .CalculateDistance(new Xamarin.Essentials.Location(driver.Location.Latitude, driver.Location.Longitude),
                        new Xamarin.Essentials.Location(double.Parse(deliveryModal.PickupLat), double.Parse(deliveryModal.PickupLong)), Xamarin.Essentials.DistanceUnits.Kilometers);
                    tempDrivers.Add(new TempDriver { Away = distance, Driver_Id = driver.Uid });
                }
                tempDrivers.Sort((x, y) =>  x.Away.CompareTo(y.Away));
            }
                
        }
        public event EventHandler<DisposeEventArgs> DismissDialog;
        public class DisposeEventArgs: EventArgs
        {
            public bool DismissDialog { get; set; }
        }
        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            DismissDialog.Invoke(this, new DisposeEventArgs { DismissDialog = true} );

        }
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
       

   
    }
    public class TempDriver
    {
        public string Driver_Id { get; set; }
        public double Away { get; set; }
    }
}
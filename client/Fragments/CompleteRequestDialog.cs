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
using FirebaseAdmin.Messaging;
using System.Net.Http;
using System.Text;

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
        private Requests deliveryModal;

        public CompleteRequestDialog(Requests deliveryModal)
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
                    InputPersonName.Error = "NAME CAN NOT BE EMPTY";
                    InputPersonName.RequestFocus();
                    return;
                }
                if (string.IsNullOrEmpty(InputPersonContact.Text) && string.IsNullOrWhiteSpace(InputPersonContact.Text))
                {
                    //Toast.MakeText(this, "Please provide the contact numbers of the person you delivering to", ToastLength.Long).Show();
                    InputPersonContact.Error = "PLEASE PROVIDE PHONE NUMBER";
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
                //FindNearestDriver();


                deliveryModal.PersonContact = InputPersonContact.Text;
                deliveryModal.PersonName = InputPersonName.Text;
                deliveryModal.Status = "W";
                deliveryModal.RequestTime = DateTime.Now;



                try
                {
                    HttpClient httpClient = new HttpClient();
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(deliveryModal);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync($"{API.ApiUrl}/requests", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var str = await response.Content.ReadAsStringAsync();
                        var request = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(str);
                        await CrossCloudFirestore
                            .Current
                            .Instance
                            .Collection("REQUESTS")
                            .Document(request.Id)
                            .SetAsync(deliveryModal);
                    }


                }
                catch (Exception)
                {

                    throw;
                }

                try
                {
                    var stream = Resources.Assets.Open("service_account.json");
                    var fcm = FirebaseHelper.FirebaseAdminSDK.GetFirebaseMessaging(stream);
                    FirebaseAdmin.Messaging.Message message = new FirebaseAdmin.Messaging.Message()
                    {
                        Topic = "requests",
                        Notification = new Notification()
                        {
                            Title = "New Query",
                            Body = $"REQUEST HAS BEEN MADE FOR ADDRESS:  {deliveryModal.PickupAddress.ToUpper()} TO {deliveryModal.DestinationAddress.ToLower()}",

                        },
                    };
                    await fcm.SendAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                var dlg = new IonAlert(view.Context, IonAlert.SuccessType);
                dlg.SetTitleText("Success");
                dlg.SetContentText("Your request has been successfully made.");
                dlg.CancelEvent += (s, e) =>
                {
                    Dismiss();
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
            if (!query.IsEmpty)
            {
                var drivers = query.ToObjects<AppUsers>();
                foreach (var driver in drivers)
                {
                    var distance = Xamarin.Essentials
                        .LocationExtensions
                        .CalculateDistance(new Xamarin.Essentials.Location(driver.Location.Latitude, driver.Location.Longitude),
                        new Xamarin.Essentials.Location(deliveryModal.PickupLat, deliveryModal.PickupLong), Xamarin.Essentials.DistanceUnits.Kilometers);
                    tempDrivers.Add(new TempDriver { Away = distance, Driver_Id = driver.Uid });
                }
                tempDrivers.Sort((x, y) => x.Away.CompareTo(y.Away));
            }

        }
        public event EventHandler<DisposeEventArgs> DismissDialog;
        public class DisposeEventArgs : EventArgs
        {
            public bool DismissDialog { get; set; }
        }
        public override void OnDismiss(IDialogInterface dialog)
        {
            base.OnDismiss(dialog);
            DismissDialog.Invoke(this, new DisposeEventArgs { DismissDialog = true });

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
    public class Response
    {
        public string Id { get; set; }
        public string Content { get; set; }
    }
}
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using driver.Activities;
using driver.Dialogs;
using driver.Models;
using Firebase.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextView;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using Plugin.CloudFirestore.Reactive;
using System;
using Android.Views.Animations;
using System.Collections.Generic;
using Xamarin.Essentials;
using static AndroidAboutPage.Resource;
using Android.Content;
using AndroidHUD;
using Animation = Android.Views.Animations.Animation;
using static AndroidX.RecyclerView.Widget.RecyclerView;
using Google.Android.Material.BottomSheet;
using AndroidX.RecyclerView.Widget;

namespace driver.Fragments
{
    public class MainFragment : Fragment, IOnMapReadyCallback
    {
        private GoogleMap gmap;
        private RelativeLayout DeliveryRequestNavigation;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.main_fragment, container, false);
            context = view.Context;
            ConnectViews(view);

            return view;
        }

        SwitchCompat BtnStatus;
        MaterialTextView TxtStatus;
        private RelativeLayout RequestMainMenuLayout;

        public MainFragment(Dashboad dashboad)
        {
            dashboad.PermissionHandler += Dashboad_PermissionHandler;
        }

        private async void Dashboad_PermissionHandler(object sender, Dashboad.PermissioGranede e)
        {
            if (e.Granted)
            {
                var location = await Geolocation.GetLocationAsync();
                var pos = new LatLng(location.Latitude, location.Longitude);
                //googleMap.AnimateCamera()
                gmap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(pos, 17));
                //gmap.CameraIdle += GoogleMap_CameraIdle;
            }
        }

        private void ConnectViews(View view)
        {
            var mapFragment = ChildFragmentManager.FindFragmentById(Resource.Id.fragMap).JavaCast<SupportMapFragment>();
            mapFragment.GetMapAsync(this);
            BtnStatus = view.FindViewById<SwitchCompat>(Resource.Id.btn_status);
            TxtStatus = view.FindViewById<MaterialTextView>(Resource.Id.txt_status);


            RequestMainMenuLayout = view.FindViewById<RelativeLayout>(Resource.Id.RequestMainMenuLayout);
            //toolbarRequests = FindViewById<MaterialToolbar>(Resource.Id.toolbarRequests);
            ConnectDeliveryRequestNavigation(view);
            ConnectBottomSheet(view);
            BtnStatus.Click += (s, e) =>
            {

                CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("USERS")
                    .Document(FirebaseAuth.Instance.Uid)
                    .UpdateAsync("Status", BtnStatus.Checked);
            };
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(FirebaseAuth.Instance.Uid)
                .AddSnapshotListener((value, error) =>
                {
                    if (value.Exists)
                    {
                        var user = value.ToObject<DriverModel>();
                        BtnStatus.Checked = user.IsOnline;
                        isOnline = user.IsOnline;
                        if (user.IsOnline)
                        {
                            TxtStatus.Text = "ONLINE";

                        }
                        else
                        {
                            TxtStatus.Text = "OFFLINE";
                        }
                    }
                });

            UpcomingRequests();
        }
        private LinearLayout bottomSheetLayout;
        private BottomSheetBehavior bottomSheet;
        private TextView txtRequestFromName;
        private TextView txtRequestFromContact;
        private TextView txtRequestToName;
        private TextView txtRequestToContact;
        private TextView txtRequestItemType;
        private TextView txtRequestPickupLication;
        private TextView txtRequestDestination;
        private TextView txtRequestDistance;
        private TextView txtRequestPrice;
        private TextView txtRequestStatus;
        private MaterialButton BtnAcceptRequest;
        private void ConnectBottomSheet(View view)
        {
            bottomSheetLayout = view.FindViewById<LinearLayout>(Resource.Id.bottom_sheet_delivery_request);
            bottomSheet = BottomSheetBehavior.From(bottomSheetLayout);
            BtnAcceptRequest = view.FindViewById<MaterialButton>(Resource.Id.BtnAcceptRequest);
            txtRequestDestination = view.FindViewById<TextView>(Resource.Id.txtRequestDestination);
            txtRequestDistance = view.FindViewById<TextView>(Resource.Id.txtRequestDistance);
            txtRequestFromContact = view.FindViewById<TextView>(Resource.Id.txtRequestFromContact);
            txtRequestFromName = view.FindViewById<TextView>(Resource.Id.txtRequestFromName);
            txtRequestItemType = view.FindViewById<TextView>(Resource.Id.txtRequestItemType);
            txtRequestPickupLication = view.FindViewById<TextView>(Resource.Id.txtRequestPickupLication);
            txtRequestPrice = view.FindViewById<TextView>(Resource.Id.txtRequestPrice);
            txtRequestStatus = view.FindViewById<TextView>(Resource.Id.txtRequestStatus);
            txtRequestToContact = view.FindViewById<TextView>(Resource.Id.txtRequestToContact);
            txtRequestToName = view.FindViewById<TextView>(Resource.Id.txtRequestToName);
            BtnAcceptRequest.Click += BtnAcceptRequest_Click;


        }
        private Animation AnimMainMenuLayout;
        private Animation AnimRequestNavigation;
        private async void BtnAcceptRequest_Click(object sender, EventArgs e)
        {
            AnimMainMenuLayout = AnimationUtils.LoadAnimation(context, Resource.Animation.slide_right);
            AnimRequestNavigation = AnimationUtils.LoadAnimation(context, Resource.Animation.float_up);

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                gmap.Clear();//driver's name to be added
                Dictionary<string, object> valuePairs = new Dictionary<string, object>
                {
                    { "Status", "A" },
                    { "DriverId", FirebaseAuth.Instance.Uid }
                };
                await CrossCloudFirestore.Current.Instance
                    .Collection("REQUESTS")
                    .Document(KeyPosition)
                    .UpdateAsync(valuePairs);
                //Toast.MakeText(this, UserKeyId, ToastLength.Long).Show();
                bottomSheet.State = BottomSheetBehavior.StateCollapsed;
                RequestMainMenuLayout.StartAnimation(AnimMainMenuLayout);
                AnimMainMenuLayout.AnimationEnd += AnimMainMenuLayout_AnimationEnd;

                try
                {
                    /*string json;
                    json = await mapHelper.GetDirectionJsonAsync(pickupLocationLatLng, destinationLocationLatLng);
                    if (!string.IsNullOrEmpty(json))
                    {
                        mapHelper.DrawTripOnMap(json);
                    }*/
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: ", ex.Message);
                }
                // UpdateState("Accepted", KeyPositiono
            }
            else
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(context);
                builder.SetTitle("Error");
                builder.SetMessage("Unable to connect, please check your Internet connection");
                builder.SetNeutralButton("Ok", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }
        }
        private void AnimMainMenuLayout_AnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {
            RequestMainMenuLayout.Visibility = ViewStates.Gone;
            DeliveryRequestNavigation.StartAnimation(AnimRequestNavigation);
            DeliveryRequestNavigation.Visibility = ViewStates.Visible;
            AnimRequestNavigation.AnimationEnd += AnimRequestNavigation_AnimationEnd;
        }
        private void AnimRequestNavigation_AnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {

        }
        private MaterialButton BtnPickupDestination;
        private MaterialButton BtnReassign;
        private FloatingActionButton ImgNavigate, imgCancelDelivery, ImgCall;
        string KeyPosition;
        private void ConnectDeliveryRequestNavigation(View view)
        {
            BtnPickupDestination = view.FindViewById<MaterialButton>(Resource.Id.BtnPickupDestination);
            BtnReassign = view.FindViewById<MaterialButton>(Resource.Id.BtnReassign);
            ImgNavigate = view.FindViewById<FloatingActionButton>(Resource.Id.ImgNavigate);
            ImgCall = view.FindViewById<FloatingActionButton>(Resource.Id.ImgCall);
            imgCancelDelivery = view.FindViewById<FloatingActionButton>(Resource.Id.imgCancelDelivery);
            DeliveryRequestNavigation = view.FindViewById<RelativeLayout>(Resource.Id.DeliveryRequestNavigation);
            DeliveryRequestNavigation.Visibility = ViewStates.Gone;
            ImgCall.Click += ImgCall_Click;
            imgCancelDelivery.Click += ImgCancelDelivery_Click;
            ImgNavigate.Click += ImgNavigate_Click;
            BtnPickupDestination.Click += BtnPickupDestination_Click;
        }
        private string PickPhone;
        private string DestPhone;

        private void BtnPickupDestination_Click(object sender, EventArgs e)
        {
            var query = CrossCloudFirestore.Current.Instance
                    .Collection("REQUESTS")
                    .Document(KeyPosition);
            Dictionary<string, object> valuePairs = new Dictionary<string, object>();

            if (BtnPickupDestination.Text == "Pickup")
            {
                valuePairs.Add("Status", "P");
                query.UpdateAsync(valuePairs);


                //UpdateState("Picked up", KeyPosition);
                BtnPickupDestination.Text = "Deliver";
                imgCancelDelivery.Enabled = false;
                imgCancelDelivery.Alpha = 0.5f;
                HUD("Picked up");
            }
            else if (BtnPickupDestination.Text == "Deliver")
            {
                valuePairs.Add("Status", "D");
                query.UpdateAsync(valuePairs);



                BtnPickupDestination.Text = "Done";
                HUD("Updated");
                //preferencesEditor.Clear();
            }
            else if (BtnPickupDestination.Text == "Done")
            {
                /*int pos = items.FindIndex(x => x.KeyId == KeyPosition);
                if (pos >= 0)
                {
                    items.RemoveAt(pos);
                    adapter.NotifyDataSetChanged();
                }*/
                RequestMainMenuLayout.Visibility = ViewStates.Visible;
                DeliveryRequestNavigation.Visibility = ViewStates.Gone;
                BtnPickupDestination.Text = "Pickup";
                HUD("Done");
                gmap.Clear();
            }

        }
        private async void OpenNavigationMap(double lat, double lon, string caption)
        {
            var location = new Xamarin.Essentials.Location(lat, lon);
            var option = new MapLaunchOptions { Name = caption };
            await Xamarin.Essentials.Map.OpenAsync(location, option);
        }
        private LatLng pickupLocationLatLng;
        private LatLng destinationLocationLatLng;
        private void ImgNavigate_Click(object sender, EventArgs e)
        {
            if (BtnPickupDestination.Text == "Deliver")
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(context);
                builder.SetTitle("Confirm");
                builder.SetMessage("Navigate to destination?");
                builder.SetPositiveButton("Yes", delegate
                {
                    builder.Dispose();
                    OpenNavigationMap(destinationLocationLatLng.Latitude, destinationLocationLatLng.Longitude, "Destination");


                });
                builder.SetNegativeButton("No", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }
            if (BtnPickupDestination.Text == "Pickup")
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(context);
                builder.SetTitle("Confirm");
                builder.SetMessage("Navigate to pickup location?");
                builder.SetPositiveButton("Yes", delegate
                {
                    builder.Dispose();
                    OpenNavigationMap(pickupLocationLatLng.Latitude, pickupLocationLatLng.Longitude, "Pickup location ");


                });
                builder.SetNegativeButton("No", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }
        }
        private async void ImgCancelDelivery_Click(object sender, EventArgs e)
        {

            animOpenNavigation = AnimationUtils.LoadAnimation(context, Resource.Animation.float_down);

            Dictionary<string, object> valuePairs = new Dictionary<string, object>
            {
                { "Status", "W" },
                { "DriverId", null }
            };

            await CrossCloudFirestore.Current.Instance
                    .Collection("REQUESTS")
                    .Document(KeyPosition)
                    .UpdateAsync(valuePairs);


            DeliveryRequestNavigation.StartAnimation(animOpenNavigation);
            animOpenNavigation.AnimationEnd += AnimOpenNavigation_AnimationEnd;
            HUD("Request canceled");

        }
        private void ImgCall_Click(object sender, EventArgs e)
        {
            int index = 0;

            string[] arr = { "Pick up location", "Drop point" };
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(context);
            builder.SetSingleChoiceItems(arr, 0, (s, args) =>
            {

                index = args.Which;

            });
            builder.SetPositiveButton("Call", delegate
            {

                if (index == 1)
                {
                    PhoneDialer.Open(DestPhone);
                    builder.Dispose();
                }
                else
                {
                    PhoneDialer.Open(PickPhone);
                    builder.Dispose();
                }
            });
            builder.SetNegativeButton("Cancel", delegate
            {
                builder.Dispose();
            });
            builder.Show();
        }
        private void HUD(string message)
        {
            AndHUD.Shared.ShowSuccess(context, message, MaskType.Black, TimeSpan.FromSeconds(2));
        }
        private Animation animOpenNavigation;
        private Animation animOpenMainMenu;
        private void AnimOpenNavigation_AnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {
            DeliveryRequestNavigation.Visibility = ViewStates.Gone;
            animOpenMainMenu = AnimationUtils.LoadAnimation(context, Resource.Animation.slide_left);
            RequestMainMenuLayout.StartAnimation(animOpenMainMenu);
            RequestMainMenuLayout.Visibility = ViewStates.Visible;

        }

        bool isOnline = false;
        bool inProgress = false;
        private Context context;

        private void UpcomingRequests()
        {
            RequestDialogFragment requestDialog;
            DeliveryModal data;
            CrossCloudFirestore
                    .Current.Instance.Collection("REQUESTS").WhereIn("Status", new[] { "P", "A", "W" })
                    // .OrderBy("TimeStamp", false)
                    .AddSnapshotListener((snapshot, error) =>
                    {
                        if (error != null)
                        {
                            Console.WriteLine("Errorrr", error.Message);
                        }
                        if (!snapshot.IsEmpty && snapshot != null)
                        {

                            foreach (var dc in snapshot.DocumentChanges)
                            {
                                switch (dc.Type)
                                {
                                    case DocumentChangeType.Added:
                                        var doc = dc.Document.ToObject<DeliveryModal>();
                                        CheckRequestAsync(doc);
                                        if (doc.Status == "W")
                                        {

                                            doc.KeyId = dc.Document.Id;
                                            //items.Add(doc);
                                            if (isOnline && !inProgress)
                                            {

                                                data = doc;
                                                requestDialog = new RequestDialogFragment(data);
                                                requestDialog.Show(ChildFragmentManager.BeginTransaction(), null);
                                                inProgress = true;
                                                try
                                                {
                                                    // Use default vibration length
                                                    Vibration.Vibrate();


                                                }
                                                catch (FeatureNotSupportedException ex)
                                                {
                                                    // Feature not supported on device
                                                }
                                                catch (Exception ex)
                                                {
                                                    // Other error has occurred.
                                                }
                                            }
                                        }
                                        break;
                                    case DocumentChangeType.Modified:
                                        var mod = dc.Document.ToObject<DeliveryModal>();
                                        mod.KeyId = dc.Document.Id;
                                        if (mod.Status == "A" || mod.Status == "P")
                                        {
                                            if (mod.DriverId == FirebaseAuth.Instance.Uid)
                                            {
                                                CheckRequestAsync(mod);
                                                inProgress = true;
                                                //adapter.NotifyDataSetChanged();
                                            }

                                        }

                                        if (mod.Status == "D")
                                        {
                                            inProgress = false;
                                        }

                                        if (mod.Status == "W" && mod.DriverId == null)
                                        {

                                        }
                                        break;
                                    case DocumentChangeType.Removed:
                                        break;
                                }
                            }
                        }
                    });

        }

        public async void OnMapReady(GoogleMap googleMap)
        {

            gmap = googleMap;
            var stream = Resources.Assets.Open("uber_style.json");
            using (var reader = new System.IO.StreamReader(stream))
            {
                var data = reader.ReadToEnd();
                gmap.SetMapStyle(new MapStyleOptions(data));
            }
            var location = await Geolocation.GetLocationAsync();
            var pos = new LatLng(location.Latitude, location.Longitude);
            //googleMap.AnimateCamera()
            gmap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(pos, 17));
        }
        //location coordinates
        double PickupLongitude;
        double Pickuplatitude;
        double DestinationLongitude;
        double Destinationlatitude;


        //
        private void CheckRequestAsync(DeliveryModal doc)
        {
            if (doc.DriverId == FirebaseAuth.Instance.Uid)
            {
                if (doc.Status == "A" || doc.Status == "P")
                {
                    inProgress = true;
                    RequestMainMenuLayout.Visibility = ViewStates.Gone;
                    DeliveryRequestNavigation.Visibility = ViewStates.Visible;
                    KeyPosition = doc.KeyId;
                    PickPhone = doc.ContactNo;
                    DestPhone = doc.PersonContact;

                    System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
                    cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
                    System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;


                    Pickuplatitude = double.Parse(doc.PickupLat);

                    //Toast.MakeText(this, Pickuplatitude.ToString(), ToastLength.Long).Show();

                    PickupLongitude = Convert.ToDouble(doc.PickupLong);
                    Destinationlatitude = Convert.ToDouble(doc.DestinationLat);
                    DestinationLongitude = Convert.ToDouble(doc.DestinationLong);
                    try
                    {
                        pickupLocationLatLng = new LatLng(Pickuplatitude, PickupLongitude);
                        destinationLocationLatLng = new LatLng(Destinationlatitude, DestinationLongitude);
                        gmap.Clear();
                        string json;
                        /* json = await mapHelper.GetDirectionJsonAsync(pickupLocationLatLng, destinationLocationLatLng);
                         if (!string.IsNullOrEmpty(json))
                         {
                             mapHelper.DrawTripOnMap(json);
                         }*/
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("Error: Line 329", Ex.Message);
                    }
                }
            }
        }

    }


}
public class UpcomingRequest
{
    [MapTo("RequestId")]
    public string Id { get; set; }
}
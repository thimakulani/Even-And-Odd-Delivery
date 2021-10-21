using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidHUD;
using AndroidX.RecyclerView.Widget;
using driver.Adapters;
using driver.MapsHelper;
using driver.Models;
using Firebase.Auth;
using Google.Android.Material.AppBar;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace driver.Activities
{
    [Activity(Label = "DeliveryRequestActivity")]
    public class DeliveryRequestActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private GoogleMap mGoogleMap;
        /// 
        readonly string[] permission = { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation };
        const int requestLocationId = 0;
        private Animation AnimMainMenuLayout;
        private Animation AnimRequestNavigation;

        LocationRequest locationRequest;
        FusedLocationProviderClient locationClient;
        Android.Locations.Location lastLocation;
        private static readonly int UPDATE_INTERVAL = 5;
        private static readonly int UPDATE_FASTEST_INTERVAL = 5;
        private static readonly int DISPLACEMENT = 3;

        private LocationCallBackHelper locationCallBack;
        private LatLng pickupLocationLatLng;
        private LatLng destinationLocationLatLng;

        //location coordinates
        double PickupLongitude;
        double Pickuplatitude;
        double DestinationLongitude;
        double Destinationlatitude;
        //

        private readonly List<DeliveryModal> items = new List<DeliveryModal>();
        private RecyclerView RecyclerRequests;
        string KeyPosition;

        //*Bottom sheet**//
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

        private RelativeLayout RequestMainMenuLayout;

        //Delivery Navigation Included Layout
        private RelativeLayout DeliveryRequestNavigation;
        private MaterialButton BtnPickupDestination;
        private MaterialButton BtnReassign;
        private FloatingActionButton ImgNavigate, imgCancelDelivery, ImgCall;
        private SupportMapFragment mapFragment;
        private MaterialToolbar toolbarRequests;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if(savedInstanceState == null)
            {
                CrossCloudFirestore.Current
                    .Instance.ClearPersistenceAsync();
            }
            // Create your application here
            SetContentView(Resource.Layout.activity_home);
            RequestedOrientation = ScreenOrientation.Portrait;
            mapFragment = SupportFragmentManager.FindFragmentById(Resource.Id.fragMap).JavaCast<SupportMapFragment>();


            mapFragment.GetMapAsync(this);
            try
            {
                if (CheckPermission())
                {
                    CreateLocationRequest();
                    GetLocation();
                    StartLocationUpdate();
                }
            }
            catch (System.Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            ConnectViews();
        }

        private void ConnectViews()
        {
            RecyclerRequests = FindViewById<RecyclerView>(Resource.Id.RecyclerDeliveryRequest);

            RequestMainMenuLayout = FindViewById<RelativeLayout>(Resource.Id.RequestMainMenuLayout);
            toolbarRequests = FindViewById<MaterialToolbar>(Resource.Id.toolbarRequests);
            ConnectDeliveryRequestNavigation();

            ConnectBottomSheet();
            toolbarRequests.NavigationClick += ToolbarRequests_NavigationClick;
            // Notifications();



            SetupAdapter();

        }

        private void ToolbarRequests_NavigationClick(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            OnBackPressed();
            Finish();
        }

        private void ConnectBottomSheet()
        {
            bottomSheetLayout = FindViewById<LinearLayout>(Resource.Id.bottom_sheet_delivery_request);
            bottomSheet = BottomSheetBehavior.From(bottomSheetLayout);
            BtnAcceptRequest = FindViewById<MaterialButton>(Resource.Id.BtnAcceptRequest);
            txtRequestDestination = FindViewById<TextView>(Resource.Id.txtRequestDestination);
            txtRequestDistance = FindViewById<TextView>(Resource.Id.txtRequestDistance);
            txtRequestFromContact = FindViewById<TextView>(Resource.Id.txtRequestFromContact);
            txtRequestFromName = FindViewById<TextView>(Resource.Id.txtRequestFromName);
            txtRequestItemType = FindViewById<TextView>(Resource.Id.txtRequestItemType);
            txtRequestPickupLication = FindViewById<TextView>(Resource.Id.txtRequestPickupLication);
            txtRequestPrice = FindViewById<TextView>(Resource.Id.txtRequestPrice);
            txtRequestStatus = FindViewById<TextView>(Resource.Id.txtRequestStatus);
            txtRequestToContact = FindViewById<TextView>(Resource.Id.txtRequestToContact);
            txtRequestToName = FindViewById<TextView>(Resource.Id.txtRequestToName);
            BtnAcceptRequest.Click += BtnAcceptRequest_Click;


        }
        private async void BtnAcceptRequest_Click(object sender, EventArgs e)
        {
            AnimMainMenuLayout = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_right);
            AnimRequestNavigation = AnimationUtils.LoadAnimation(this, Resource.Animation.float_up);

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                mGoogleMap.Clear();//driver's name to be added
                Dictionary<string, object> valuePairs = new Dictionary<string, object>
                {
                    { "Status", "A" },
                    { "DriverId", FirebaseAuth.Instance.Uid }
                };
                await CrossCloudFirestore.Current.Instance
                    .Collection("DELIVERY")
                    .Document(KeyPosition)
                    .UpdateAsync(valuePairs);
                //Toast.MakeText(this, UserKeyId, ToastLength.Long).Show();
                bottomSheet.State = BottomSheetBehavior.StateCollapsed;
                RequestMainMenuLayout.StartAnimation(AnimMainMenuLayout);
                AnimMainMenuLayout.AnimationEnd += AnimMainMenuLayout_AnimationEnd;

                try
                {
                    string json;
                    json = await mapHelper.GetDirectionJsonAsync(pickupLocationLatLng, destinationLocationLatLng);
                    if (!string.IsNullOrEmpty(json))
                    {
                        mapHelper.DrawTripOnMap(json);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: ", ex.Message);
                }
                // UpdateState("Accepted", KeyPositiono
            }
            else
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
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
        private string PickPhone;
        private string DestPhone;
        RequestAdapter adapter;
        private void SetupAdapter()
        {
            LinearLayoutManager linearLayout = new LinearLayoutManager(this);
            RecyclerRequests.SetLayoutManager(linearLayout);
            adapter = new RequestAdapter(items);
            adapter.ItemClick += Adapter_ItemClick;
            RecyclerRequests.SetAdapter(adapter);

            CrossCloudFirestore
                .Current.Instance.Collection("DELIVERY").WhereIn("Status", new[] { "P", "A", "W" })
                .OrderBy("TimeStamp", false)
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
                                    if(doc.Status == "W")
                                    {
                                        doc.KeyId = dc.Document.Id;
                                        items.Add(doc);
                                    }
                                    else
                                    {
                                        adapter.NotifyDataSetChanged();
                                    }
                                    CheckRequestAsync(doc);
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Modified:
                                    var mod = dc.Document.ToObject<DeliveryModal>();
                                    mod.KeyId = dc.Document.Id;
                                    if (mod.Status == "A" || mod.Status == "P")
                                    {
                                        if (mod.DriverId == FirebaseAuth.Instance.Uid)
                                        {
                                            CheckRequestAsync(mod);
                                            adapter.NotifyDataSetChanged(); 
                                        }
                                        else
                                        {
                                            int pos = items.FindIndex(x => x.KeyId == dc.Document.Id);
                                            if(pos >= 0)
                                            {
                                                items.RemoveAt(pos);
                                            }
                                            adapter.NotifyDataSetChanged();
                                        }
                                    }

                                    if (mod.Status == "D")
                                    {
                                        int pos = items.FindIndex(x => x.KeyId == dc.Document.Id);
                                        if (pos >= 0)
                                        {
                                            items.RemoveAt(pos);
                                        }
                                        adapter.NotifyDataSetChanged();
                                    }
                                    else if (mod.Status == "D")
                                    {

                                    }
                                    
                                    if (mod.Status == "W" && mod.DriverId == null)
                                    {
                                        int pos = items.FindIndex(X => X.KeyId == dc.Document.Id);
                                        if(pos>=0)
                                        {
                                            items.Add(mod);
                                        }
                                        adapter.NotifyDataSetChanged();
                                    }
                                    break;
                                case DocumentChangeType.Removed:

                                    break;
                            }
                        }
                    }
                });

        }

        private async void CheckRequestAsync(DeliveryModal doc)
        {
            if (doc.DriverId == FirebaseAuth.Instance.Uid)
            {
                if (doc.Status == "A" || doc.Status == "P")
                {
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
                        mGoogleMap.Clear();
                        string json;
                        json = await mapHelper.GetDirectionJsonAsync(pickupLocationLatLng, destinationLocationLatLng);
                        if (!string.IsNullOrEmpty(json))
                        {
                            mapHelper.DrawTripOnMap(json);
                        }
                    }
                    catch (Exception Ex)
                    {
                        Console.WriteLine("Error: Line 329", Ex.Message);
                    }
                }
            }
        }

        private void Adapter_ItemClick(object sender, RequestAdapterClickEventArgs e)
        {

            int indexPos = e.Position;

            KeyPosition = items[indexPos].KeyId;
            txtRequestDestination.Text = items[indexPos].DestinationAddress;
            txtRequestDistance.Text = items[indexPos].Distance;
            txtRequestFromContact.Text = items[indexPos].ContactNo;
            txtRequestFromName.Text = items[indexPos].Name;
            txtRequestItemType.Text = items[indexPos].ItemType;
            txtRequestPickupLication.Text = items[indexPos].PickupAddress;
            txtRequestPrice.Text = items[indexPos].Price;
            if(items[indexPos].Status == "W")
            {
                txtRequestStatus.Text = "Waiting";
            }

            

            txtRequestToContact.Text = items[indexPos].PersonContact;
            txtRequestToName.Text = items[indexPos].PersonName;


            PickPhone = items[indexPos].ContactNo;
            DestPhone = items[indexPos].PersonContact;

            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            Pickuplatitude = Convert.ToDouble(items[indexPos].PickupLat);
            PickupLongitude = Convert.ToDouble(items[indexPos].PickupLong);
            Destinationlatitude = Convert.ToDouble(items[indexPos].DestinationLat);
            DestinationLongitude = Convert.ToDouble(items[indexPos].DestinationLong);




            //Toast.MakeText(this, Pickuplatitude.ToString(), ToastLength.Long).Show();
            pickupLocationLatLng = new LatLng(Pickuplatitude, PickupLongitude);
            destinationLocationLatLng = new LatLng(Destinationlatitude, DestinationLongitude);



            if (items[indexPos].Status != "C")
            {
                BtnAcceptRequest.Visibility = ViewStates.Visible;

            }
            else
            {
                BtnAcceptRequest.Visibility = ViewStates.Gone;

            }
            bottomSheet.State = BottomSheetBehavior.StateExpanded;

        }

        private void ConnectDeliveryRequestNavigation()
        {
            DeliveryRequestNavigation = FindViewById<RelativeLayout>(Resource.Id.DeliveryRequestNavigation);
            BtnPickupDestination = FindViewById<MaterialButton>(Resource.Id.BtnPickupDestination);
            BtnReassign = FindViewById<MaterialButton>(Resource.Id.BtnReassign);
            ImgNavigate = FindViewById<FloatingActionButton>(Resource.Id.ImgNavigate);
            ImgCall = FindViewById<FloatingActionButton>(Resource.Id.ImgCall);
            imgCancelDelivery = FindViewById<FloatingActionButton>(Resource.Id.imgCancelDelivery);
            DeliveryRequestNavigation.Visibility = ViewStates.Gone;
            ImgCall.Click += ImgCall_Click;
            imgCancelDelivery.Click += ImgCancelDelivery_Click;
            ImgNavigate.Click += ImgNavigate_Click;
            BtnPickupDestination.Click += BtnPickupDestination_Click;
            BtnReassign.Click += BtnReassign_Click;
        }
        private AlertDialog dialog;
        private TextInputEditText InputReason;
        private MaterialButton BtnSend;
        private void BtnReassign_Click(object sender, EventArgs e)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            LayoutInflater inflater = (LayoutInflater)GetSystemService(Context.LayoutInflaterService);
            var view = inflater.Inflate(Resource.Layout.reassign_layout, null);
            BtnSend = view.FindViewById<MaterialButton>(Resource.Id.BtnSubmitReasign);
            InputReason = view.FindViewById<TextInputEditText>(Resource.Id.InputReason);

            BtnSend.Click += BtnSend_Click;
            builder.SetView(view);
            dialog = builder.Create();
            dialog.Show();
        }
        private void HUD(string message)
        {
            AndHUD.Shared.ShowSuccess(this, message, MaskType.Black, TimeSpan.FromSeconds(2));
        }
        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(InputReason.Text))
            {
                Dictionary<string, object> hashMap = new Dictionary<string, object>
                {
                    { "driver_id", FirebaseAuth.Instance.CurrentUser.Uid },
                    { "request_id", FirebaseAuth.Instance.CurrentUser.Uid }
                };

                CrossCloudFirestore.Current.Instance.Collection("Reassign")
                    .Document(KeyPosition)
                    .SetAsync(hashMap);
                HUD("Your request has been successfully submitted");
                dialog.Dismiss();
            }
            else
            {
                InputReason.Error = "Please provide a reason";
            }
        }

        private void BtnPickupDestination_Click(object sender, EventArgs e)
        {
            var query = CrossCloudFirestore.Current.Instance
                    .Collection("DELIVERY")
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
                int pos = items.FindIndex(x => x.KeyId == KeyPosition);
                if(pos >= 0)
                {
                    items.RemoveAt(pos);
                    adapter.NotifyDataSetChanged();
                }
                RequestMainMenuLayout.Visibility = ViewStates.Visible;
                DeliveryRequestNavigation.Visibility = ViewStates.Gone;
                BtnPickupDestination.Text = "Pickup";
                HUD("Done");
                mGoogleMap.Clear();
            }

        }
        private void ImgNavigate_Click(object sender, EventArgs e)
        {
            if (BtnPickupDestination.Text == "Deliver")
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
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
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
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
        private async void OpenNavigationMap(double lat, double lon, string caption)
        {
            var location = new Xamarin.Essentials.Location(lat, lon);
            var option = new MapLaunchOptions { Name = caption };
            await Xamarin.Essentials.Map.OpenAsync(location, option);
        }
        private Animation animOpenNavigation;
        private Animation animOpenMainMenu;
        private async void ImgCancelDelivery_Click(object sender, EventArgs e)
        {

            animOpenNavigation = AnimationUtils.LoadAnimation(this, Resource.Animation.float_down);

            Dictionary<string, object> valuePairs = new Dictionary<string, object>
            {
                { "Status", "W" },
                { "DriverId", null }
            };

            await CrossCloudFirestore.Current.Instance
                    .Collection("DELIVERY")
                    .Document(KeyPosition)
                    .UpdateAsync(valuePairs);


            DeliveryRequestNavigation.StartAnimation(animOpenNavigation);
            animOpenNavigation.AnimationEnd += AnimOpenNavigation_AnimationEnd;
            HUD("Request canceled");

        }
        private void AnimOpenNavigation_AnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {
            DeliveryRequestNavigation.Visibility = ViewStates.Gone;
            animOpenMainMenu = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_left);
            RequestMainMenuLayout.StartAnimation(animOpenMainMenu);
            RequestMainMenuLayout.Visibility = ViewStates.Visible;

        }
        private void ImgCall_Click(object sender, EventArgs e)
        {
            int index = 0;

            string[] arr = { "Pick up location", "Drop point" };
            Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
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
        #region maps
        MapFunctionHelper mapHelper;
        public void OnMapReady(GoogleMap googleMap)
        {
            mGoogleMap = googleMap;
            mGoogleMap.CameraIdle += MGoogleMap_CameraIdle;
            string mapKey = Resources.GetString(Resource.String.mapKey);


            mapHelper = new MapFunctionHelper(mapKey, mGoogleMap);
        }

        private void MGoogleMap_CameraIdle(object sender, EventArgs e)
        {

        }

        private bool CheckPermission()
        {
            bool permisionGranted = false;
            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted)
            {
                //               permisionGranted = true;
                RequestPermissions(permission, requestLocationId);
            }
            else
            {
                return true;
            }
            return permisionGranted;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] gra0ntResults)
        {
            if (gra0ntResults[0] == (int)Permission.Granted)
            {
                Toast.MakeText(this, "Permission was granted", ToastLength.Long).Show();
                CreateLocationRequest();
                GetLocation();
                StartLocationUpdate();
            }
            else
            {
                Toast.MakeText(this, "Permission was denied", ToastLength.Long).Show();
            }
        }
        private void CreateLocationRequest()
        {
            locationRequest = LocationRequest.Create();
            locationRequest.SetInterval(UPDATE_INTERVAL);
            locationRequest.SetFastestInterval(UPDATE_FASTEST_INTERVAL);
            locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            locationRequest.SetSmallestDisplacement(DISPLACEMENT);
            locationClient = LocationServices.GetFusedLocationProviderClient(this);
            locationCallBack = new LocationCallBackHelper();
            locationCallBack.CurrentLocation += LocationCallBack_CurrentLocation;
        }

        private void LocationCallBack_CurrentLocation(object sender, LocationCallBackHelper.OnLocationCapturedEventArgs e)
        {
            lastLocation = e.Location_;
            LatLng position = new LatLng(lastLocation.Latitude, lastLocation.Longitude);
            //if (!RdbDestinationLocation.Checked && !RdbPickUpLocation.Checked && !PickupAddressFromSearch && !DestinationAddressFromSearch)
            //{
            //lastLocation = e.location;
            //LatLng position = new LatLng(lastLocation.Latitude, lastLocation.Longitude);
            mGoogleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(position, 17));
            //}

        }
        private async void GetLocation()
        {
            if (!CheckPermission())
            {
                return;
            }
            lastLocation = await locationClient.GetLastLocationAsync();
            if (lastLocation != null)
            {
                LatLng pos = new LatLng(lastLocation.Latitude, lastLocation.Longitude);
                mGoogleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(pos, 13));
                ////loading to database
                ///
                Dictionary<string, object> hashMap = new Dictionary<string, object>
                {
                    { "lat", lastLocation.Latitude },
                    { "lon", lastLocation.Longitude }
                };
                await CrossCloudFirestore.Current.Instance
                    .Collection("LOCATION")
                    .Document(FirebaseAuth.Instance.CurrentUser.Uid)
                    .SetAsync(hashMap);

                //TxtPickup.Text =  address.Result[0].Thoroughfare.Append(" ") + " " + address.Result[0].SubAdminArea;
            }
        }
        private void StartLocationUpdate()
        {
            if (CheckPermission())
            {
                locationClient.RequestLocationUpdates(locationRequest, locationCallBack, null);
            }
        }


        #endregion

    }
}


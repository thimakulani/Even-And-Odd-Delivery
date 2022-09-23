using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using client.Classes;
using client.Interface;
using Firebase.Auth;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.RadioButton;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;
using Google.Rpc;
using Java.Util;
using OSRMLib.OSRMServices;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static client.MainActivity;

namespace client.Fragments
{
    public class MainFragment : Fragment, IOnMapReadyCallback
    {
        private GoogleMap gmap;
        private MaterialTextView TxtDistance;
        private MaterialTextView TxtPrice;
        private MaterialTextView TxtDuration;



        private MaterialTextView TxtPickup;
        private MaterialTextView TxtDestination;
        private MaterialRadioButton RadioBtnPickup;
        private MaterialRadioButton RadioBtnDest;
        private AppCompatImageView ImgMyLocation;
        private AppCompatImageView ImgCenterMarker;
        private MaterialButton BtnContinue;
        private BottomSheetBehavior bottomSheet;

        private LatLng pickup_lat_lan;
        private LatLng dest_lat_lan;
        private MainActivity mainActivity;




        public MainFragment(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
            this.mainActivity.PermissionHandler += MainActivity_PermissionHandler;
        }

        private async void MainActivity_PermissionHandler(object sender, PermissioGranede e)
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

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.main_fragment, container, false);
            ConnectViews(view);
            Init();
            return view;
        }
        private double InitialPrice;
        private double AfterInitial;
        private void Init()
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("PRICE")
                .Document("Price")
                .AddSnapshotListener((snapshot, error) =>
                {
                    if (snapshot.Exists)
                    {
                        var price = snapshot.ToObject<TripPrice>();
                        InitialPrice = double.Parse(price.InitialPrice);
                        AfterInitial = double.Parse(price.PriceAfter);
                    }
                    else
                    {
                        InitialPrice = 25.0;
                        AfterInitial = 7.0;
                    }
                });
        }
        MaterialButton BtnRequest;
        private void ConnectViews(View view)
        {
            LinearLayout bottomSheetLayout = view.FindViewById<LinearLayout>(Resource.Id.bottom_sheet);
            ImgCenterMarker = view.FindViewById<AppCompatImageView>(Resource.Id.ImgCenterMarker);
            ImgMyLocation = view.FindViewById<AppCompatImageView>(Resource.Id.ImgMyLocation);
            RadioBtnDest = view.FindViewById<MaterialRadioButton>(Resource.Id.RdbDestinationLocation);
            RadioBtnPickup = view.FindViewById<MaterialRadioButton>(Resource.Id.RdbPickUpLocation);
            //bs
            TxtDuration = view.FindViewById<MaterialTextView>(Resource.Id.TxtDuration);
            TxtPrice = view.FindViewById<MaterialTextView>(Resource.Id.TxtPrice);
            TxtDistance = view.FindViewById<MaterialTextView>(Resource.Id.TxtDistance);
            BtnRequest = view.FindViewById<MaterialButton>(Resource.Id.BtnContinueRequest);


            TxtPickup = view.FindViewById<MaterialTextView>(Resource.Id.TxtPickUpLocation);
            TxtDestination = view.FindViewById<MaterialTextView>(Resource.Id.TxtDestinationLocation);
            BtnContinue = view.FindViewById<MaterialButton>(Resource.Id.BtnOpenBottomSheet);
            var mapFragment = ChildFragmentManager.FindFragmentById(Resource.Id.fragMap).JavaCast<SupportMapFragment>();
            mapFragment.GetMapAsync(this);

            //bottomsheet
            bottomSheet = BottomSheetBehavior.From(bottomSheetLayout);

            RadioBtnPickup.Checked = true;

            RadioBtnPickup.Click += (sender, e) =>
            {
                if (RadioBtnDest.Checked)
                {
                    RadioBtnDest.Checked = false;
                }
            };
            RadioBtnDest.Click += (sender, e) =>
            {
                if (RadioBtnPickup.Checked)
                {
                    RadioBtnPickup.Checked = false;
                }
            };
            ImgMyLocation.Click += async (sender, e) =>
            {
                var pos = await Geolocation.GetLastKnownLocationAsync();
                if(pos!=null )
                {
                    if (move)
                    {
                        if (RadioBtnPickup.Checked)
                        {
                            TxtPickup.Text = await GetAddress(pos.Latitude, pos.Longitude);
                        }
                        else
                        {
                            TxtDestination.Text = await GetAddress(pos.Latitude, pos.Longitude);
                        } 
                    }
                }
            };
          
            BtnRequest.Click += BtnRequest_Click;
            BtnContinue.Click += BtnContinue_Click;
        }



        private void BtnRequest_Click(object sender, EventArgs e)
        {
            DeliveryModal deliveryModal = new DeliveryModal()
            {
                Distance = TxtDistance.Text,
                DestinationAddress = TxtDestination.Text,
                PickupAddress = TxtPickup.Text,
                DestinationLat = dest_lat_lan.Latitude.ToString(),
                DestinationLong = dest_lat_lan.Longitude.ToString(),
                PickupLat = dest_lat_lan.Latitude.ToString(),
                PickupLong = dest_lat_lan.Longitude.ToString(),
                Price = TxtPrice.Text,
                UserId = FirebaseAuth.Instance.Uid,
            };
            CompleteRequestDialog complete = new CompleteRequestDialog(deliveryModal);
            complete.Show(ChildFragmentManager.BeginTransaction(), null);
            complete.DismissDialog += Complete_DismissDialog;
        }

        private void Complete_DismissDialog(object sender, CompleteRequestDialog.DisposeEventArgs e)
        {
            gmap.Clear();
            ImgCenterMarker.Visibility = ViewStates.Visible;
            bottomSheet.PeekHeight = 0;
        }

        private void BtnContinue_Click(object sender, EventArgs e)
        {
            if(dest_lat_lan != null && pickup_lat_lan != null)
            {

                
                var startLocation = new OSRMLib.Helpers.Location(pickup_lat_lan.Latitude, pickup_lat_lan.Longitude);
                var destLocation = new OSRMLib.Helpers.Location(dest_lat_lan.Latitude, dest_lat_lan.Longitude);

                
                GetRoute(startLocation, destLocation);
                
               
            }
        }

        public async void OnMapReady(GoogleMap googleMap)
        {
            var stream = Resources.Assets.Open("uber_style.json");
            gmap = googleMap;
            using (var reader = new System.IO.StreamReader(stream))
            {
                var data = reader.ReadToEnd();
                gmap.SetMapStyle(new MapStyleOptions(data));
            }
            var location = await Geolocation.GetLocationAsync();
            var pos = new LatLng(location.Latitude, location.Longitude);
            ////googleMap.AnimateCamera()
            gmap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(pos, 17));
            gmap.CameraIdle += GoogleMap_CameraIdle;
        }

        private async void GoogleMap_CameraIdle(object sender, EventArgs e)
        {
            var target = gmap.CameraPosition.Target;
            var pos = new LatLng(target.Latitude, target.Longitude);
            if(pos != null && move)
            {
                if(NetworkAccess.Internet != Connectivity.NetworkAccess)
                { 
                    return; 
                }
                string address = await GetAddress(pos.Latitude, pos.Longitude);
                if(address == null)
                {
                    return;
                }

                if (RadioBtnPickup.Checked)
                {
                    TxtPickup.Text = address;
                    pickup_lat_lan = new LatLng(pos.Latitude, pos.Longitude);
                }
                if (RadioBtnDest.Checked)
                {
                    TxtDestination.Text = address;
                    dest_lat_lan = new LatLng(pos.Latitude, pos.Longitude);
                }
            }
        }
        private async Task<string> GetAddress(double lat, double lon)
        {

            try
            {
                var r = await Geocoding.GetPlacemarksAsync(lat, lon);
                var address = r?.FirstOrDefault();

                StringBuilder s = new StringBuilder();
                if (!string.IsNullOrEmpty(address.SubThoroughfare.Trim()) && !string.IsNullOrWhiteSpace(address.SubThoroughfare))
                {
                    s.Append(address.SubThoroughfare);
                }
                if (!string.IsNullOrEmpty(address.Thoroughfare) && !string.IsNullOrWhiteSpace(address.Thoroughfare))
                {
                    if (!string.IsNullOrEmpty(address.SubThoroughfare) && !string.IsNullOrWhiteSpace(address.SubThoroughfare))
                    {
                        s.Append(", ");
                    }
                    s.Append(address.Thoroughfare);
                }

                if (!string.IsNullOrEmpty(address.Locality) && !string.IsNullOrWhiteSpace(address.Locality))
                {
                    if (!string.IsNullOrEmpty(address.SubLocality) && !string.IsNullOrWhiteSpace(address.SubLocality))
                    {
                        s.Append(", ");
                    }
                    s.Append(address.Locality);
                }
                return s.ToString();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Console.WriteLine(fnsEx.Message);
            }
            catch (Exception ex)
            {
                // Handle exception that may have occurred in geocoding
                Console.WriteLine(ex.Message);
            }

            return String.Empty;


        }
        //osrm
        RouteService routeS = new RouteService();

        public async void GetRoute(OSRMLib.Helpers.Location startPos, OSRMLib.Helpers.Location endPos)
        {
            BtnContinue.Enabled = false;
            BtnContinue.Text = "PLEASE WAIT...";
            gmap.Clear();
            routeS.Coordinates = new List<OSRMLib.Helpers.Location> { startPos, endPos };

            var response = await routeS.Call();
            
            var points = response.Routes[0].Geometry;
            Java.Util.ArrayList routeList = new Java.Util.ArrayList();
            foreach (var point in points)
            {
                routeList.Add(new LatLng(point.Latitude, point.Longitude));
            }
            PolylineOptions polylineOptions = new PolylineOptions()
                .AddAll(routeList)
                .InvokeWidth(10)
                .InvokeColor(Resource.Color.material_blue_grey_800)
                .InvokeStartCap(new SquareCap())
                .InvokeEndCap(new SquareCap())
                .InvokeJointType(JointType.Round)
                .Geodesic(true);
            gmap.AddPolyline(polylineOptions);

            LatLng firstpoint = pickup_lat_lan;
            LatLng lastpoint = dest_lat_lan;

            //Pickup marker options
            MarkerOptions pickupMarkerOptions = new MarkerOptions();
            pickupMarkerOptions.SetPosition(firstpoint);
            pickupMarkerOptions.SetTitle("Pickup Location");
            
            pickupMarkerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));


            //Destination marker options
            MarkerOptions destinationMarkerOptions = new MarkerOptions();
            destinationMarkerOptions.SetPosition(lastpoint);
            destinationMarkerOptions.SetTitle("Destination");
            destinationMarkerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));

            Marker pickupMarker = gmap.AddMarker(pickupMarkerOptions);

            gmap.AddMarker(destinationMarkerOptions);

            double radiusDegrees = 0.10;
            LatLng northEast = new LatLng(pickup_lat_lan.Latitude + radiusDegrees, pickup_lat_lan.Longitude + radiusDegrees);
            LatLng southWest = new LatLng(pickup_lat_lan.Latitude - radiusDegrees, pickup_lat_lan.Longitude - radiusDegrees);
            LatLngBounds bounds = new LatLngBounds(southWest, northEast);
            gmap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(pickup_lat_lan, 15));
            gmap.SetPadding(40, 100, 40, 70);
            pickupMarker.ShowInfoWindow();



            ImgCenterMarker.Visibility = ViewStates.Gone;
            double distance = Math.Round(response.Routes[0].Legs[0].Distance/1000,2);
            TxtDistance.Text = $"{distance} KM";
            TxtDuration.Text = $"{Math.Round(response.Routes[0].Duration/60)} Minutes";
            TxtPrice.Text = $"R{CalculatePrice(distance)}";
            
            bottomSheet.State = BottomSheetBehavior.StateExpanded;
            ImgCenterMarker.Visibility = ViewStates.Gone;
            BtnContinue.Enabled = true;
            BtnContinue.Text = "CONTINUE";
            move = false;

            

        }
        private bool move = true; 
        private double CalculatePrice(double distance)
        {
            double fares;
            if (distance <= 5)
            {
                fares = InitialPrice;
            }
            else
            {
                fares = ((distance - 5) * AfterInitial) + InitialPrice;

            }
            return Math.Round(fares);
        }


    }
}
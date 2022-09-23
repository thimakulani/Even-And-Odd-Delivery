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
using Google.Android.Material.TextView;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static AndroidAboutPage.Resource;

namespace driver.Fragments
{
    public class MainFragment : Fragment, IOnMapReadyCallback
    {
        private GoogleMap gmap;
      

        private LatLng pickup_lat_lan;
        private LatLng dest_lat_lan;
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
            ConnectViews(view);
            DeliveryRequestNavigation = view.FindViewById<RelativeLayout>(Resource.Id.DeliveryRequestNavigation);
            DeliveryRequestNavigation.Visibility = ViewStates.Gone;
            return view;
        }

        MaterialButton BtnRequest;
        SwitchCompat BtnStatus;
        MaterialTextView TxtStatus;
        private Animation animOpenNavigation;

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

            BtnStatus.Click += (s, e) =>
            {
                if(BtnStatus.Checked)
                {
                    CrossCloudFirestore
                        .Current
                        .Instance
                        .Collection("USERS")
                        .Document(FirebaseAuth.Instance.Uid)
                        .UpdateAsync("Status", "Online");
                }
                else
                {
                    CrossCloudFirestore
                        .Current
                        .Instance
                        .Collection("USERS")
                        .Document(FirebaseAuth.Instance.Uid)
                        .UpdateAsync("Status", "Offline");
                }

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
                        if(user.Status == "Online")
                        {
                            TxtStatus.Text = user.Status.ToUpper();
                            
                            BtnStatus.Checked = true;
                        }
                        if (user.Status == "Offline")
                        {
                            TxtStatus.Text = user.Status.ToUpper();
                            BtnStatus.Checked = false;
                        }
                    }
                });
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("UPCOMING")
                .Document(FirebaseAuth.Instance.Uid)
                .AddSnapshotListener((v, e) =>
                {
                    if (v.Exists)
                    {
                        var data = v.ToObject<UpcomingRequest>();
                        RequestDialogFragment requestDialog = new RequestDialogFragment(data.Id);
                        requestDialog.Show(ChildFragmentManager.BeginTransaction(), null);
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


    }

}
public class UpcomingRequest
{
    [MapTo("RequestId")]
    public string Id { get; set; }
}
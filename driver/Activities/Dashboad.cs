using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Extensions;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.App;
using driver.Fragments;
using driver.Models;
using Firebase.Auth;
using Firebase.Messaging;
using Google.Android.Material.AppBar;
using Google.Android.Material.BottomNavigation;
using Plugin.CloudFirestore;
using System;
using System.Threading.Tasks;
using static Google.Android.Material.Navigation.NavigationBarView;
using AlertDialog = Android.App.AlertDialog;

namespace driver.Activities
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false)]
    public class Dashboad : AppCompatActivity, IOnItemSelectedListener
    {
        MaterialToolbar toolbar;
        private DriverModel user = new DriverModel();
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_dashboard);
            this.RequestedOrientation = ScreenOrientation.Portrait;

            ///*OUT*/
            toolbar = FindViewById<MaterialToolbar>(Resource.Id.toolbar_dashboad);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            navigation.SetOnItemSelectedListener(this);
            Xamarin.Essentials.Connectivity.ConnectivityChanged += (sender, args) =>
            {
                if (args.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet)
                {
                    Toast.MakeText(this, "Connection established", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "Connection disconnected", ToastLength.Short).Show();
                }
            };
            CheckGps();
            DocumentRef = CrossCloudFirestore
                            .Current
                            .Instance;
            MainFragment welcomeFrag = new MainFragment(this);
            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.frameLayout_container, welcomeFrag)
                .Commit();
           // welcomeFrag.RequestEventHandler += WelcomeFrag_RequestEventHandler;
            await FirebaseMessaging.Instance.SubscribeToTopic("requests");

            DocumentRef
                .Collection("USERS")
                .Document(FirebaseAuth.Instance.Uid)
                .AddSnapshotListener((value, error) =>
                {

                    if (value.Exists)
                    {
                        user = value.ToObject<DriverModel>();
                        toolbar.Title = $"{user.Name} {user.Surname}".ToUpper();
                        if(user.Status == "Online")
                        {
                            UpdateCoordinate(true);
                        }
                        else
                        {
                            UpdateCoordinate(false);
                        }
                    }
                });
            CheckUserType();
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == 0)
            {
                if (!Settings.CanDrawOverlays(this))
                {

                }
                else
                {
                    StartService(new Intent(this, typeof(FloatingService)));
                }
            }
        }
        public event EventHandler<PermissioGranede> PermissionHandler;
        public class PermissioGranede : EventArgs
        {
            public bool Granted { get; set; }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            if(user.Status == "Online")
            {
                if (!Settings.CanDrawOverlays(this))
                {
                    StartActivityForResult(new Intent(Settings.ActionManageOverlayPermission, Android.Net.Uri.Parse("package:" + PackageName)), 0);
                }
                else
                {
                    StartService(new Intent(this, typeof(FloatingService)));

                }
            }

        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (grantResults[0] == Permission.Granted)
            {
                PermissionHandler.Invoke(this, new PermissioGranede { Granted = true });
            }
        }
        GeoPoint geoPoint;

        private IFirestore DocumentRef;

        public void UpdateCoordinate(bool flag)
        {
            
            if (flag)
            {

                Task startWork = new Task(() =>
                {
                    Task.Delay(500);
                });
                startWork.ContinueWith(async t =>
                {
                    try
                    {
                        var pos = await Xamarin.Essentials.Geolocation.GetLocationAsync();
                        geoPoint = new GeoPoint(pos.Latitude, pos.Longitude);
                        await DocumentRef
                            .Collection("USERS")
                            .Document(FirebaseAuth.Instance.Uid)
                            .UpdateAsync("Location", geoPoint);
                        UpdateCoordinate(true);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                      
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
                startWork.Start();
            }
           
        }
        private async void CheckUserType()
        {
            var results = await DocumentRef
                .Collection("USERS")
                .Document(FirebaseAuth.Instance.Uid)
                .GetAsync();
            if (results.Exists)
            {
                DriverModel driver = results.ToObject<DriverModel>();

                if (driver.Role == "D")
                {
                    var requests = await DocumentRef
                        .Collection("DELIVERY")
                        .WhereEqualsTo("DriverId", FirebaseAuth.Instance.Uid)
                        .WhereIn("Status", new[] { "A", "P" })
                        .GetAsync();
                    if (requests.Count > 0)
                    {
                        Intent intent = new Intent(this, typeof(DeliveryRequestActivity));
                        StartActivity(intent);
                    }

                }
                else
                {
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Error");
                    builder.SetCancelable(false);
                    builder.SetMessage("Your not registered as a driver, Please contact the administrator.");
                    builder.SetNeutralButton("Continue", delegate
                    {
                        builder.Dispose();
                        FirebaseAuth.Instance.SignOut();
                        Finish();
                    });
                    builder.Show();
                }
            }

        }
       

        private void HUD(string message)
        {
            AndHUD.Shared.ShowSuccess(this, message, MaskType.Black, TimeSpan.FromSeconds(2));
        }


        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_requests:
                    MainFragment welcomeFrag = new MainFragment(this);
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.frameLayout_container, welcomeFrag)
                        .Commit();
                    //welcomeFrag.RequestEventHandler += WelcomeFrag_RequestEventHandler;
                    return true;
                case Resource.Id.navigation_history:
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.frameLayout_container, new HistoryFragment())
                        .Commit();
                    return true;
                case Resource.Id.navigation_about:
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.frameLayout_container, new AboutFragment())
                        .Commit();
                    return true;
                case Resource.Id.navigation_profile:
                    ProfileFragment profileFrag = new ProfileFragment();

                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.frameLayout_container, profileFrag)
                        .Commit();
                    return true;
                case Resource.Id.navigation_logout:
                    AlertDialog.Builder builder = new AlertDialog.Builder(this);
                    builder.SetTitle("Confirm");

                    builder.SetMessage("Are you sure that you want to exit");
                    builder.SetPositiveButton("Yes", delegate
                    {
                        builder.Dispose();
                        Firebase.Auth.FirebaseAuth.Instance.SignOut();
                        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        {
                            base.FinishAndRemoveTask();
                        }
                        else
                        {
                            base.Finish();
                        }
                    });
                    builder.SetNegativeButton("No", delegate
                    {
                        builder.Dispose();
                        return;
                    });
                    builder.Show();
                    return true;
            }
            return false;
        }


        private void ProfileFrag_SuccessUpdateHandler(object sender, EventArgs e)
        {
            HUD("Profile successfully updated");
        }

        private void CheckGps()
        {
            LocationManager locationManager = (LocationManager)GetSystemService(Context.LocationService);
            bool gps_enable = false;
            // bool newtwork_enable = false;
            gps_enable = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            if (!gps_enable)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Confirm");
                builder.SetMessage("Enable location");
                builder.SetNegativeButton("Cancel", delegate
                {
                    builder.Dispose();
                });
                builder.SetPositiveButton("Settings", delegate
                {

                    StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
                });
                builder.Show();

            }

        }

        private void Upcoming()
        {

        }

    }
}


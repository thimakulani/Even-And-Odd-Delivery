using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Extensions;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using client.Classes;
using client.Fragments;
using Firebase.Auth;
using Firebase.Messaging;
using Gauravk.BubbleNavigation;
using Google.Android.Material.AppBar;
using Google.Android.Material.Dialog;
using Plugin.CloudFirestore;
using System;

namespace client
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        private AppUsers users = new AppUsers();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            RequestedOrientation = ScreenOrientation.Portrait;

            var toolbar_main = FindViewById<MaterialToolbar>(Resource.Id.toolbar_main);
            if(savedInstanceState == null)
            {

                SupportFragmentManager.BeginTransaction()
                   .Add(Resource.Id.fragment_container, new MainFragment(this))
                   .Commit();
                toolbar_main.Title = "HOME";
            }
            Xamarin.Essentials.Connectivity.ConnectivityChanged += (sender, args) =>
            {
                if(args.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet)
                {
                    Toast.MakeText(this, "Connection established", ToastLength.Short).Show();
                }
                else
                {
                    Toast.MakeText(this, "Connection disconnected", ToastLength.Short).Show();
                }
            };
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(FirebaseAuth.Instance.Uid)
                .AddSnapshotListener(async (value, error) =>
                {
                    if (value.Exists)
                    {
                        users = value.ToObject<AppUsers>();
                        await FirebaseMessaging.Instance.SubscribeToTopic(FirebaseAuth.Instance.Uid);
                        await FirebaseMessaging.Instance.SubscribeToTopic("A");
                    }
                });

            IsLocationEnabled();

            

            
            toolbar_main.InflateMenu(Resource.Menu.top_bar_menue);
            toolbar_main.MenuItemClick += (s, e) =>
            {
                if(e.Item.ItemId == Resource.Id.about)
                {
                    
                    SupportFragmentManager.BeginTransaction()
                        .Replace(Resource.Id.fragment_container, new AboutFragment())
                        .Commit();
                    toolbar_main.Title = "ABOUT";
                }
                if (e.Item.ItemId == Resource.Id.become_driver)
                {
                    if (users.Role == "C")
                    {
                        DriverRegistrationDialog driver = new DriverRegistrationDialog();
                        driver.Show(SupportFragmentManager.BeginTransaction(), "Driver Registration");
                        toolbar_main.Title = "BECOME DRIVER";
                    }
                    else if (users.Role == null)
                    {
                        MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(this);
                        builder.SetTitle("Confirm");
                        // builder.SetMessage("Reset password link has been sent to your email address");
                        builder.SetMessage("Waiting for your driver request to be approved by administrators");
                        builder.SetPositiveButton("OK", delegate
                        {

                            builder.Dispose();

                        });
                        builder.Show();
                    }
                    else if (users.Role == "D")
                    {
                        MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(this);
                        builder.SetTitle("Confirm");
                        // builder.SetMessage("Reset password link has been sent to your email address");
                        builder.SetMessage("You have already registered as a driver");
                        builder.SetPositiveButton("OK", delegate
                        {

                            builder.Dispose();

                        });
                        builder.Show();
                    }
                    else
                    {
                        MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(this);
                        builder.SetTitle("Confirm");
                        // builder.SetMessage("Reset password link has been sent to your email address");
                        builder.SetMessage("You have already registered as administrators");
                        builder.SetPositiveButton("Yes", delegate
                        {

                            builder.Dispose();

                        });
                        builder.Show();
                    }
                }
            };

            BubbleNavigationLinearView bubbleNavigationLinearView = FindViewById<BubbleNavigationLinearView>(Resource.Id.bubble_nav);
            bubbleNavigationLinearView.NavigationChange += (e, position) =>
            {
               if(position.Position == 0)
                {
                    SupportFragmentManager.BeginTransaction()
                       .Replace(Resource.Id.fragment_container, new MainFragment(this))
                       .Commit();
                    toolbar_main.Title = "HOME";
                }
                if (position.Position == 1)
                {
                    SupportFragmentManager.BeginTransaction()
                       .Replace(Resource.Id.fragment_container, new HistoryFragment())
                       .Commit();
                    toolbar_main.Title = "HISTORY";
                }
                if (position.Position == 2)
                {
                    SupportFragmentManager.BeginTransaction()
                       .Replace(Resource.Id.fragment_container, new HelpFragment())
                       .Commit();
                }
                if (position.Position == 3)
                {
                    SupportFragmentManager.BeginTransaction()
                       .Replace(Resource.Id.fragment_container, new ProfileFragment())
                       .Commit();
                    toolbar_main.Title = "PROFILE";
                }
                if (position.Position == 4)
                {
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("Confirm");
                    alert.SetMessage("Are you sure you want to exit:");
                    alert.SetPositiveButton("Yes", delegate
                    {
                        alert.Dispose();
                        FirebaseAuth.Instance.SignOut();
                        if (Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        {
                            base.FinishAndRemoveTask();
                        }
                        else
                        {
                            base.Finish();
                        }

                    });
                    alert.SetNegativeButton("No", delegate
                    {
                        alert.Dispose();
                    });
                    alert.Show();
                }
            }; 
        }
        public event EventHandler<PermissioGranede> PermissionHandler;
        public class PermissioGranede : EventArgs
        {
            public bool Granted { get; set; }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            if (grantResults[0] == Permission.Granted)
            {
                PermissionHandler.Invoke(this, new PermissioGranede { Granted = true });
            }
        }
        public override void OnBackPressed()
        {

        }
        private void IsLocationEnabled()
        {
            LocationManager locationManager = (LocationManager)GetSystemService(Context.LocationService);
            bool gps_enable = false;
            // bool newtwork_enable = false;
            gps_enable = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            if (!gps_enable)
            {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
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

     
    }
}
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Extensions;
using Android.Locations;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.App;
using client.Activities;
using client.Classes;
using client.Fragments;
using Firebase.Auth;
using Firebase.Messaging;
using Google.Android.Material.AppBar;
using Google.Android.Material.Dialog;
using Google.Android.Material.Navigation;
using Plugin.CloudFirestore;
using System;

namespace client
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity
    {
        private NavigationView nav_view;
        // HomeFragment homeFragment;
        TextView HeaderUsername;
        TextView TxtHeaderEmail;
        private DrawerLayout drawerLayout;
        private MaterialToolbar toolbar_main;
        AppUsers users = new AppUsers();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            string log = Intent.GetStringExtra("Log");
            RequestedOrientation = ScreenOrientation.Portrait;

            toolbar_main = FindViewById<MaterialToolbar>(Resource.Id.toolbar_main);
            nav_view = FindViewById<NavigationView>(Resource.Id.nav_view);
            nav_view.InflateMenu(Resource.Menu.nav_menu);
            nav_view.InflateHeaderView(Resource.Layout.nav_header_layout);
            HeaderUsername = nav_view.GetHeaderView(0).FindViewById<TextView>(Resource.Id.TxtHeaderUsername);
            TxtHeaderEmail = nav_view.GetHeaderView(0).FindViewById<TextView>(Resource.Id.TxtHeaderEmail);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.nav_drawer);
            nav_view.NavigationItemSelected += Nav_view_NavigationItemSelected;



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
                        HeaderUsername.Text = $"{users.Name} {users.Surname}";
                        TxtHeaderEmail.Text = users.Email;
                        await FirebaseMessaging.Instance.SubscribeToTopic(FirebaseAuth.Instance.Uid);
                        await FirebaseMessaging.Instance.SubscribeToTopic("A");
                    }
                });
            
            WelcomeFragment homeFragment = new WelcomeFragment();
            SupportFragmentManager.BeginTransaction()
                .Add(Resource.Id.fragment_container, homeFragment)
                .Commit();
            toolbar_main.Title = "Home";
            homeFragment.RequestEventHandler += HomeFragment_RequestEventHandler;

            if (log == "History")
            {
                HistoryFragment frag = new HistoryFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, frag)
                    .Commit();
                toolbar_main.Title = "History";


            }


            toolbar_main.SetNavigationIcon(Resource.Mipmap.ic_menu_white_18dp);
            toolbar_main.NavigationClick += Toolbar_main_NavigationClick1;
            CheckGps();


        }

        private void Toolbar_main_NavigationClick1(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
        }



        private void HomeFragment_RequestEventHandler(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(DeliveryRequest));
            intent.PutExtra("Log", "Other");
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
            if (toolbar_main.Title == "History")
            {
                WelcomeFragment homeFragment = new WelcomeFragment();
                SupportFragmentManager.BeginTransaction()
                    .Add(Resource.Id.fragment_container, homeFragment)
                    .Commit();
                toolbar_main.Title = "Home";
                homeFragment.RequestEventHandler += HomeFragment_RequestEventHandler;
            }


        }

        private void Nav_view_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e)
        {
            drawerLayout.CloseDrawers();
            if (e.MenuItem.ItemId == Resource.Id.nav_profile)
            {


                ProfileFragment profileFragment = new ProfileFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, profileFragment)
                    .Commit();
                toolbar_main.Title = "User Profile";

            }
            if (e.MenuItem.ItemId == Resource.Id.nav_home)
            {
                WelcomeFragment homeFragment = new WelcomeFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, homeFragment)
                    .Commit();
                // TxtAppBarTitle.Text = "Home";
                homeFragment.RequestEventHandler += HomeFragment_RequestEventHandler;
                toolbar_main.Title = "Home";


            }
            if (e.MenuItem.ItemId == Resource.Id.nav_help)
            {
                HelpFragment helpFrag = new HelpFragment();
                SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.fragment_container, helpFrag)
                .Commit();
                toolbar_main.Title = "Help";


            }
            if (e.MenuItem.ItemId == Resource.Id.nav_history)
            {
                HistoryFragment frag = new HistoryFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, frag)
                    .Commit();
                toolbar_main.Title = "History";


            }
            if (e.MenuItem.ItemId == Resource.Id.nav_about)
            {
                AboutFragment frag = new AboutFragment();
                SupportFragmentManager.BeginTransaction()
                    .Replace(Resource.Id.fragment_container, frag)
                    .Commit();
                toolbar_main.Title = "About";



            }
            if (e.MenuItem.ItemId == Resource.Id.nav_become_driver)
            {
                if(users.Role == "C")
                {
                    DriverRegistrationDialog driver = new DriverRegistrationDialog();
                    driver.Show(SupportFragmentManager.BeginTransaction(), "Driver Registration");
                }
                else if(users.Role == null)
                {
                    MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(this);
                    builder.SetTitle("Confirm");
                    // builder.SetMessage("Reset password link has been sent to your email address");
                    builder.SetMessage("Waiting for your driver request to be approved by administrators");
                    builder.SetPositiveButton("Ok", delegate
                    {

                        builder.Dispose();

                    });
                    builder.Show();
                }
                else if (users.Role=="D")
                {
                    MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(this);
                    builder.SetTitle("Confirm");
                    // builder.SetMessage("Reset password link has been sent to your email address");
                    builder.SetMessage("You have already registrerd as a driver");
                    builder.SetPositiveButton("Ok", delegate
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
            if (e.MenuItem.ItemId == Resource.Id.nav_logout)
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

        }
        public override void OnBackPressed()
        {

        }
        private void CheckGps()
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
        private void ProfileFragment_SuccessUpdateHandler(object sender, System.EventArgs e)
        {
            HUD("Profile successfully updated");
        }
        private void HUD(string message)
        {
            AndHUD.Shared.ShowSuccess(this, message, MaskType.Black, TimeSpan.FromSeconds(2));
        }
        public void CheckLocationEnabled()
        {
            LocationManager locationManager = (LocationManager)GetSystemService(Context.LocationService);
            bool gps_enable = false;
            // bool newtwork_enable = false;
            gps_enable = locationManager.IsProviderEnabled(LocationManager.GpsProvider);
            if (!gps_enable)
            {
                Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this);
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
using admin.Adapters;
using admin.Models;
using Android.App;
using Android.Content;
using Android.Gms.Extensions;
using Android.OS;
using AndroidX.RecyclerView.Widget;
using Android.Widget;
using AndroidX.AppCompat.App;
using Firebase.Auth;
using Firebase.Messaging;
using Google.Android.Material.Dialog;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace admin.Activities
{
    [Activity(Label = "Dashboad")]
    public class Dashboad : AppCompatActivity
    {
        //loading progress dialog

        //dialogs
        private TextView txtDashboardUsername;
        private RecyclerView RecyclerMennu;
        int i = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_dashboard);
            RecyclerMennu = FindViewById<RecyclerView>(Resource.Id.RecyclerMennu);
            txtDashboardUsername = FindViewById<TextView>(Resource.Id.txtDashboardUsername);
            //FirebaseMessaging.Instance.SubscribeToTopic("QUERIES");
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(FirebaseAuth.Instance.Uid)
                .AddSnapshotListener(async (value, error) =>
                {
                    if (value.Exists)
                    {
                        var user = value.ToObject<AppUsers>();
                        if (user.Role == "A")
                        {
                            txtDashboardUsername.Text = $"{user.Name} {user.Surname}";
                            await FirebaseMessaging.Instance.SubscribeToTopic("QUERIES");
                            if(i==0)
                            {
                                SetUpMenu();
                            }
                            i++;
                        }
                        else
                        {
                            
                            MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(this);
                            builder.SetTitle("Error");
                            builder.SetMessage("Unauthorized user");
                            builder.SetCancelable(false);
                            builder.SetNeutralButton("Ok", delegate
                            {
                                builder.Dispose();
                                FirebaseAuth.Instance.SignOut();
                                Finish();
                            }).Show();
                        }
                    }
                    else
                    {
                        MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(this);
                        builder.SetTitle("Error");
                        builder.SetMessage("Unauthorized user");
                        builder.SetCancelable(false);
                        builder.SetNeutralButton("Ok", delegate
                        {
                            builder.Dispose();
                            FirebaseAuth.Instance.SignOut();
                            Finish();
                        }).Show();
                    }
                });

            

        }
        private readonly List<Menu_Items> items = new List<Menu_Items>();
        private void SetUpMenu()
        {
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_admin_panel_settings_black_18dp, Title = "My Profile" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_admin_panel_settings_black_18dp, Title = "Super Users" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.user_profile_icon, Title = "Drivers and Clients" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_announcement_black_18dp, Title = "Announce" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_commute_white_18dp, Title = "Driver Requests" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_bar_chart_black_18dp, Title = "Stats" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_addchart_black_18dp, Title = "Manage Fee" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_account_tree_black_18dp, Title = "Invoice" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_assignment_black_18dp, Title = "Delivery Report" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_attach_email_black_18dp, Title = "Queries" });
            items.Add(new Menu_Items { Icon = Resource.Mipmap.ic_exit_to_app_black_18dp, Title = "Logout" });

            MenuAdapter adapter = new MenuAdapter(items);
            GridLayoutManager gridLayoutManager = new GridLayoutManager(this, 2);
            RecyclerMennu.SetLayoutManager(gridLayoutManager);
            RecyclerMennu.SetAdapter(adapter);
            RecyclerMennu.HasFixedSize = true;
            adapter.ItemClick += Adapter_ItemClick;
        }

        private void Adapter_ItemClick(object sender, MenuAdapterClickEventArgs e)
        {
            if (e.Position == 0)
            {
                Intent intent = new Intent(this, typeof(Profile));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 1)
            {
                Intent intent = new Intent(this, typeof(SuperUser));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 2)
            {
                Intent intent = new Intent(this, typeof(RegisterDriver));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 3)
            {
                Intent intent = new Intent(this, typeof(Anouncements));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 4)
            {
                Intent intent = new Intent(this, typeof(DriverRequests));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 5)
            {
                Intent intent = new Intent(this, typeof(StatsActivity));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 6)
            {

                Intent intent = new Intent(this, typeof(TripPrice));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 7)
            {
                Intent intent = new Intent(this, typeof(Invoice));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 8)
            {
                Intent intent = new Intent(this, typeof(Report));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 9)
            {
                Intent intent = new Intent(this, typeof(Queries));
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
            }
            if (e.Position == 10)
            {

                MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(this);

                //AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Confirm");
                // builder.SetMessage("Reset password link has been sent to your email address");
                builder.SetMessage("Are you sure that you want to exit");
                builder.SetPositiveButton("Yes", delegate
                {
                    builder.Dispose();
                    //  base.OnBackPressed();
                    FirebaseAuth.Instance.SignOut();
                    Finish();
                });
                builder.SetNegativeButton("No", delegate
                {
                    builder.Dispose();
                    return;
                });
                builder.Show();
            }
        }
        private void CVAbout_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(About));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
        }

        public override void OnBackPressed()
        {
            //AlertDialog.Builder builder = new AlertDialog.Builder(this);
            MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(this);
            builder.SetTitle("Confirm");
            // builder.SetMessage("Reset password link has been sent to your email address");
            builder.SetMessage("Are you sure that you want to exit");
            builder.SetPositiveButton("Yes", delegate
            {

                builder.Dispose();
                base.OnBackPressed();
                Finish();
            });
            builder.SetNegativeButton("No", delegate
            {
                builder.Dispose();
                return;
            });
            builder.Show();
        }


    }

}
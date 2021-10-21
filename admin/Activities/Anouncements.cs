using admin.Adapters;
using admin.Fragments;
using admin.Models;
using Android.App;
using Android.OS;
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.AppBar;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace admin.Activities
{
    [Activity(Label = "Anouncements")]
    public class Anouncements : AppCompatActivity
    {
        private RecyclerView Recycler;

        private readonly List<AnnouncementModel> items = new List<AnnouncementModel>();
        private MaterialToolbar toolbar;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_announcements);
            Recycler = FindViewById<RecyclerView>(Resource.Id.RecyclerAnnouncements);
            toolbar = FindViewById<MaterialToolbar>(Resource.Id.toolbar_announcement);
            toolbar.InflateMenu(Resource.Menu.top_app_bar);
            toolbar.SetNavigationIcon(Resource.Mipmap.ic_arrow_back_black_18dp);

            toolbar.MenuItemClick += Toolbar_MenuItemClick;
            toolbar.NavigationClick += Toolbar_NavigationClick;
            /*Get announcements*/
            Recycler.SetLayoutManager(new LinearLayoutManager(this));
            AnnouncementAdapter adapter = new AnnouncementAdapter(items);
            Recycler.SetAdapter(adapter);
            adapter.ItemDeleteClick += Adapter_ItemDeleteClick;


            CrossCloudFirestore
                .Current
                .Instance
                .Collection("ANNOUNCEMENT")
                .OrderBy("TimeStamp", true)
                .AddSnapshotListener((snapshop, error) =>
                {
                    if (!snapshop.IsEmpty)
                    {
                        foreach (var dc in snapshop.DocumentChanges)
                        {
                            var announce = new AnnouncementModel();
                            switch (dc.Type)
                            {
                                case DocumentChangeType.Added:
                                    announce = dc.Document.ToObject<AnnouncementModel>();
                                    announce.Id = dc.Document.Id;
                                    //Toast.MakeText(this, $"{announce.TimeStamp.ToDateTime().Year}", ToastLength.Long).Show();
                                    items.Add(announce);
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Modified:
                                    announce = dc.Document.ToObject<AnnouncementModel>();
                                    announce.Id = dc.Document.Id;
                                    items[dc.OldIndex] = announce;
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Removed:
                                    items.RemoveAt(dc.OldIndex);
                                    adapter.NotifyItemRemoved(dc.OldIndex);
                                    break;
                            }
                        }
                    }
                });


        }

        private void Adapter_ItemDeleteClick(object sender, AnnouncementAdapterClickEventArgs e)
        {
            MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(this);
            builder.SetTitle("Confirm");
            builder.SetMessage("Are you sure you want to delete the announcement");
            builder.SetNegativeButton("No", delegate
            {
                builder.Dispose();
            });
            builder.SetPositiveButton("Yes", async delegate
            {
                await CrossCloudFirestore.Current
                   .Instance
                   .Collection("ANNOUNCEMENT")
                   .Document(items[e.Position].Id)
                   .DeleteAsync();
                builder.Dispose();
            });
            builder.Show();
        }

        private void Toolbar_NavigationClick(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }

        private void Toolbar_MenuItemClick(object sender, AndroidX.AppCompat.Widget.Toolbar.MenuItemClickEventArgs e)
        {
            if (e.Item.ItemId == Resource.Id.nav_add_announcements)
            {
                DialogAddAnnouncement dlog = new DialogAddAnnouncement();
                dlog.Show(SupportFragmentManager.BeginTransaction(), "Announcements");
            }
        }

        
      


    }
}
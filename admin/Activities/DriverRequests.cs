using admin.Adapters;
using admin.Models;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.AppBar;
using Plugin.CloudFirestore;
using System.Collections.Generic;

namespace admin.Activities
{
    [Activity(Label = "DriverRequests")]
    public class DriverRequests : AppCompatActivity
    {
        private MaterialToolbar include_app_toolbar;
        private RecyclerView recycler_driver_requests;
        private readonly List<AppUsers> Items = new List<AppUsers>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_driver_requests);
            include_app_toolbar = FindViewById<MaterialToolbar>(Resource.Id.include_app_toolbar);
            recycler_driver_requests = FindViewById<RecyclerView>(Resource.Id.recycler_driver_requests);
            include_app_toolbar.NavigationClick += Include_app_toolbar_NavigationClick1; ;

            recycler_driver_requests.SetLayoutManager(new LinearLayoutManager(this));
            DriverRequestAdapter adapter = new DriverRequestAdapter(Items);
            recycler_driver_requests.SetAdapter(adapter);
            adapter.BtnApproveClick += Adapter_BtnApproveClick;
            adapter.BtnDeclinelick += Adapter_BtnDeclinelick;

            CrossCloudFirestore.Current
                .Instance
                .Collection("USERS")
                .WhereEqualsTo("Role", null)
                .AddSnapshotListener((value, error) =>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var dc in value.DocumentChanges)
                        {
                            var user = new AppUsers();

                            switch (dc.Type)
                            {

                                case DocumentChangeType.Added:
                                    user = dc.Document.ToObject<AppUsers>();
                                    user.Uid = dc.Document.Id;
                                    Items.Add(user);
                                    adapter.NotifyItemInserted(dc.NewIndex);
                                    break;
                                case DocumentChangeType.Modified:
                                    user = dc.Document.ToObject<AppUsers>();
                                    user.Uid = dc.Document.Id;
                                    Items[dc.OldIndex] = user;
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Removed:
                                    break;
                            }
                        }
                    }
                });



        }

        private void Include_app_toolbar_NavigationClick1(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }


        private async void Adapter_BtnDeclinelick(object sender, DriverRequestAdapterClickEventArgs e)
        {
            await CrossCloudFirestore.Current
               .Instance
               .Collection("USERS")
               .Document(Items[e.Position].Uid)
               .UpdateAsync("Role", "-D");

        }

        private async void Adapter_BtnApproveClick(object sender, DriverRequestAdapterClickEventArgs e)
        {
            await CrossCloudFirestore.Current
                .Instance
                .Collection("USERS")
                .Document(Items[e.Position].Uid)
                .UpdateAsync("Role", "D");


        }


    }
}
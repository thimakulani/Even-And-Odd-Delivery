using Android.Content;
using Android.OS;
using AndroidX.RecyclerView.Widget;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using client.Adapters;
using client.Classes;
using Google.Android.Material.Button;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace client.Fragments
{
    public class WelcomeFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        //string keyId;
        private MaterialButton RequestBtn;
        private RecyclerView recyclerFeeds;
        private readonly List<AnnouncementModel> items = new List<AnnouncementModel>();
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.welcome_fragment, container, false);


            //var mapFragment = (SupportMapFragment)FragmentManager.FindFragmentById(Resource.Id.fragMap);

            //ISharedPreferences pref = Application.Context.GetSharedPreferences("UserInfo", FileCreationMode.Private);
            //keyId = FirebaseAuth.Instance.CurrentUser.Uid;



            ConnectViews(view);
            return view;
        }
        private Context context;
        private void ConnectViews(View view)
        {
            RequestBtn = view.FindViewById<MaterialButton>(Resource.Id.BtnDeliveryRequest);
            recyclerFeeds = view.FindViewById<RecyclerView>(Resource.Id.recyclerFeeds);
            RequestBtn.Click += RequestBtn_Click;
            context = view.Context;
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context.ApplicationContext);
            AnnouncementAdapter adapter = new AnnouncementAdapter(items);
            recyclerFeeds.SetLayoutManager(linearLayoutManager);
            recyclerFeeds.SetAdapter(adapter);

            /*GET ANNOUNCEMENTS*/

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
                                    items.Add(announce);
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Modified:
                                    announce = dc.Document.ToObject<AnnouncementModel>();
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



        public event EventHandler RequestEventHandler;
        private void RequestBtn_Click(object sender, EventArgs e)
        {
            if (Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet)
            {
                RequestEventHandler(sender, e);
            }
            else
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(context);
                builder.SetTitle("Network Error");
                builder.SetMessage("Please check your Internet connection before you proceed");
                builder.SetNeutralButton("OK", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }

        }
    }
}
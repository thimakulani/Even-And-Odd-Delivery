using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using client.Adapters;
using client.Classes;
using Firebase.Auth;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace client.Fragments
{
    public class HistoryFragment : Fragment
    {
        private RecyclerView RecyclerHistory;
        private readonly List<DeliveryRequestModel> items = new List<DeliveryRequestModel>();
        private Context context;


        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.activity_history, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            RecyclerHistory = view.FindViewById<RecyclerView>(Resource.Id.RecyclerHistory);

            LinearLayoutManager linearLayout = new LinearLayoutManager(view.Context);
            RequestAdapter adapter = new RequestAdapter(items);
            RecyclerHistory.SetLayoutManager(linearLayout);
            RecyclerHistory.SetAdapter(adapter);
            adapter.BtnCancelClick += Adapter_BtnCancelClick;
            adapter.BtnViewDriverClick += Adapter_BtnViewDriverClick;
            /*get history*/
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("DELIVERY")
                .WhereEqualsTo("UserId", FirebaseAuth.Instance.Uid)
                .OrderBy("TimeStamp", true)
                .AddSnapshotListener((value, errors) =>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var item in value.DocumentChanges)
                        {
                            DeliveryRequestModel modal = new DeliveryRequestModel();
                           
                            switch (item.Type)
                            {
                                case DocumentChangeType.Added:

                                    modal = item.Document.ToObject<DeliveryRequestModel>();
                                    modal.KeyId = item.Document.Id;
                                    items.Add(modal);
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Modified:
                                    modal = item.Document.ToObject<DeliveryRequestModel>();
                                    modal.KeyId = item.Document.Id;
                                    items[item.OldIndex] = modal;
                                    adapter.NotifyItemChanged(item.OldIndex);
                                    
                                    break;
                                case DocumentChangeType.Removed:
                                    break;
                                default:
                                    break;
                            }
                        }
                        {

                        }
                    }
                });
        }

        private void Adapter_BtnViewDriverClick(object sender, RequestAdapterClickEventArgs e)
        {
            int index = e.Position;
            DriverDialogFragment driverDialogFragment = new DriverDialogFragment(items[index].DriverId);
            driverDialogFragment.Show(ChildFragmentManager.BeginTransaction(), "Driver Info");
        }


        private void Adapter_BtnCancelClick(object sender, RequestAdapterClickEventArgs e)
        {
            int index = e.Position;
            AlertDialog.Builder alert = new AlertDialog.Builder(context);
            alert.SetTitle("Confirm");
            alert.SetMessage("Are you sure you want to cancel the delivery request:");
            alert.SetPositiveButton("Yes", delegate
            {
                Dictionary<string, object> valuePairs = new Dictionary<string, object>();
                CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("DELIVERY")
                    .Document(items[index].KeyId)
                    .UpdateAsync("Status", "C");

                AndHUD.Shared.ShowSuccess(context, "Your request has been cancelled", MaskType.Black, TimeSpan.FromSeconds(10));
                alert.Dispose();

            });
            alert.SetNegativeButton("No", delegate
            {
                alert.Dispose();
            });
            alert.Show();

        }

    }
}
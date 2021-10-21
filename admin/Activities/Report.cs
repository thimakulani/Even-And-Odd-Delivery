using admin.Adapters;
using admin.Common;
using admin.Models;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Print;
using Android.Runtime;
using Android.Support.V7.App;
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;
using Com.Karumi.Dexter;
using Com.Karumi.Dexter.Listener;
using Com.Karumi.Dexter.Listener.Single;
using Google.Android.Material.AppBar;
using Google.Android.Material.Button;
using iTextSharp.text;
using Plugin.CloudFirestore;
using Supercharge;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace admin.Activities
{
    [Activity(Label = "Report", MainLauncher = false)]
    public class Report : AppCompatActivity, IPermissionListener
    {
        private MaterialButton BtnGenerate;
        private MaterialButton RequestsType;
        private MaterialToolbar toolbar;
        // private string FileName;

        private RecyclerView RecyclerHistory;
        private readonly List<DeliveryModal> delivariesList = new List<DeliveryModal>();
        private List<DeliveryModal> ReportData = new List<DeliveryModal>();


        //permissions

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_report);

            BtnGenerate = FindViewById<MaterialButton>(Resource.Id.BtnGenerate);
            RecyclerHistory = FindViewById<RecyclerView>(Resource.Id.RecyclerHistory);
            RequestsType = FindViewById<MaterialButton>(Resource.Id.RequestsType);
            toolbar = FindViewById<MaterialToolbar>(Resource.Id.include_app_toolbar);
            BtnGenerate.Click += BtnGenerate_Click;
            RequestsType.Click += RequestsType_Click;
            // CheckPermission();
            toolbar.Title = "Delivery Report";
            toolbar.NavigationClick += Toolbar_NavigationClick;
            
            Dexter.WithActivity(this)
                .WithPermission(Manifest.Permission.WriteExternalStorage)
                .WithListener(this)
                .Check();

            var shimmerLayout = FindViewById<ShimmerLayout>(Resource.Id.shimmer_layout);
            shimmerLayout.StartShimmerAnimation();
            SetRecycler(delivariesList);
            //get data
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("DELIVERY")//DeliveryRequests
                .AddSnapshotListener(true, (value, error) =>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var item in value.DocumentChanges)
                        {
                            DeliveryModal Delivery = new DeliveryModal();
                            switch (item.Type)
                            {
                                case DocumentChangeType.Added:
                                    Delivery = item.Document.ToObject<DeliveryModal>();
                                    Delivery.KeyId = item.Document.Id;
                                    delivariesList.Add(Delivery);
                                    ReportData.Add(Delivery);
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Modified:
                                    Delivery = item.Document.ToObject<DeliveryModal>();
                                    Delivery.KeyId = item.Document.Id;
                                    delivariesList[item.OldIndex] = Delivery;
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Removed:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                });
            shimmerLayout.StopShimmerAnimation();
            shimmerLayout.Visibility = ViewStates.Gone;
            PrintReport.WriteFileToStorage(this, "Lato_Black.ttf");

        }

        private void Toolbar_NavigationClick(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            base.OnBackPressed();
        }

        private void RequestsType_Click(object sender, EventArgs e)
        {
            Android.Widget.PopupMenu popupMenu = new Android.Widget.PopupMenu(this, RequestsType);
            popupMenu.Menu.Add(IMenu.None, 1, 1, "All-Requests");
            popupMenu.Menu.Add(IMenu.None, 1, 1, "Monthly");
            popupMenu.Menu.Add(IMenu.None, 2, 2, "Weekly");
            popupMenu.Menu.Add(IMenu.None, 3, 3, "Daily");
            popupMenu.Show();
            popupMenu.MenuItemClick += PopupMenu_MenuItemClick;
        }

        private void PopupMenu_MenuItemClick(object sender, Android.Widget.PopupMenu.MenuItemClickEventArgs e)
        {

            RequestsType.Text = e.Item.TitleFormatted.ToString();
            if (e.Item.TitleFormatted.ToString() == "All-Requests")
            {
                ReportData = delivariesList;
                SetRecycler(ReportData);

            }
            if (e.Item.TitleFormatted.ToString() == "Weekly")
            {
                DateTime today = DateTime.Today;
                int currDayWeek = (int)today.DayOfWeek;
                DateTime sunday = today.AddDays(-currDayWeek);
                DateTime monday = sunday.AddDays(1);
                if (currDayWeek == 0)
                {
                    monday = monday.AddDays(-7);
                }
                var dates = Enumerable.Range(0, 7).Select(days => monday.AddDays(days)).ToList();
                ReportData = (from data in delivariesList
                              where
                              data.RequestTime.Contains(dates[0].Date.ToString("dd MMMMM yyyy")) ||
                              data.RequestTime.Contains(dates[1].Date.ToString("dd MMMMM yyyy")) ||
                              data.RequestTime.Contains(dates[2].Date.ToString("dd MMMMM yyyy")) ||
                              data.RequestTime.Contains(dates[3].Date.ToString("dd MMMMM yyyy")) ||
                              data.RequestTime.Contains(dates[4].Date.ToString("dd MMMMM yyyy")) ||
                              data.RequestTime.Contains(dates[5].Date.ToString("dd MMMMM yyyy")) ||
                              data.RequestTime.Contains(dates[6].Date.ToString("dd MMMMM yyyy"))
                              select data).ToList<DeliveryModal>();
                SetRecycler(ReportData);
            }
            if (e.Item.TitleFormatted.ToString() == "Daily")
            {
                ReportData = (from data in delivariesList
                              where

                              data.RequestTime.Contains(DateTime.Now.ToString("dddd, dd MMMM yyyy,"))
                              select data).ToList<DeliveryModal>();
                SetRecycler(ReportData);
            }
            if (e.Item.TitleFormatted.ToString() == "Monthly")
            {
                ReportData = (from data in delivariesList
                              where
                              data.RequestTime.Contains(DateTime.Now.ToString("MMMM yyyy"))
                              select data).ToList<DeliveryModal>();
                SetRecycler(ReportData);
            }
            //Toast.MakeText(this, ReportData.Count.ToString(), ToastLength.Long).Show();
        }
        RequestAdapter adapter;
        private void SetRecycler(List<DeliveryModal> reportData)
        {
            LinearLayoutManager linearLayout = new LinearLayoutManager(this);
            adapter = new RequestAdapter(ref reportData);
            RecyclerHistory.SetLayoutManager(linearLayout);
            RecyclerHistory.SetAdapter(adapter);

        }




        private async void BtnGenerate_Click(object sender, EventArgs e)
        {

            //CreatePDFFile(Common.PrintReport.GetAppPath(this) + documentName);
            string dname = Resource.String.app_name + " " + DateTime.Now.ToString("dddd dd MMM HH mm ss tt") + ".pdf";
            if (new Java.IO.File(dname).Exists())
            {
                new Java.IO.File(dname).Delete();
            }
            try
            {
                BtnGenerate.Text = "Please wait...";
                BtnGenerate.Enabled = false;
                //FileStream fs = new FileStream(documentName, FileMode.Create);
                ReportModel reportModel = new ReportModel(ReportData);
                await Task.Run(() =>
                {
                    reportModel.GenerateInvoice(new FileStream(PrintReport.GetAppPath(this) + dname, FileMode.Create, FileAccess.Write));
                });
                PrintReport.WriteFileToStorage(this, dname);
                PrintManager printManager = (PrintManager)GetSystemService(PrintService);
                PrintReportPDFAdapter _adapter = new PrintReportPDFAdapter(this, PrintReport.GetAppPath(this)
                        + dname, dname);
                printManager.Print("Document", _adapter, new PrintAttributes.Builder().Build());
                BtnGenerate.Enabled = true;
                BtnGenerate.Text = "Generate Report";
            }
            catch (FileNotFoundException ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            catch (DocumentException ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
            catch (IOException ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }
        }
        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
            Toast.MakeText(this, "Must accept this permission", ToastLength.Long).Show();
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
            Toast.MakeText(this, "permission granted", ToastLength.Long).Show();

        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {


        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] gra0ntResults)
        {
            if (gra0ntResults[0] == (int)Permission.Granted)
            {
                Toast.MakeText(this, "Permission was granted", ToastLength.Long).Show();

            }
            else
            {
                Toast.MakeText(this, "Permission was denied", ToastLength.Long).Show();
            }
        }




    }
}
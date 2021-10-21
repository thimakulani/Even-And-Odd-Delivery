using admin.Common;
using admin.Models;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Print;
using Android.Views;
using Android.Widget;
using Apitron.PDF.Kit;
using Apitron.PDF.Kit.FixedLayout.Resources;
using Apitron.PDF.Kit.FlowLayout.Content;
using Apitron.PDF.Kit.Styles;
using Apitron.PDF.Kit.Styles.Appearance;
using Com.Karumi.Dexter;
using Com.Karumi.Dexter.Listener;
using Com.Karumi.Dexter.Listener.Single;
using Firebase.Auth;
using Google.Android.Material.AppBar;
using Google.Android.Material.Button;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace admin.Activities
{
    [Activity(Label = "Invoice")]
    public class Invoice : Activity, IPermissionListener
    {
        private MaterialToolbar include_app_toolbar;
        private MaterialButton BtnYear;
        private MaterialButton BtnMonth;
        private MaterialButton BtnGenerate;
        readonly string[] months = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
        private readonly List<DeliveryModal> items = new List<DeliveryModal>();

        private string PersonName = "Admin";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_invoice);
            include_app_toolbar = FindViewById<MaterialToolbar>(Resource.Id.include_app_toolbar);
            BtnYear = FindViewById<MaterialButton>(Resource.Id.BtnYear);
            BtnMonth = FindViewById<MaterialButton>(Resource.Id.BtnMonth);
            BtnGenerate = FindViewById<MaterialButton>(Resource.Id.BtnGenerateInvoice);
            BtnYear.Click += BtnYear_Click;
            BtnMonth.Click += BtnMonth_Click;
            BtnGenerate.Click += BtnGenerate_Click;
            BtnYear.Text = DateTime.Now.Year.ToString();
            BtnMonth.Text = DateTime.Now.ToString("MMMM");

            CrossCloudFirestore
                .Current
                .Instance
                .Collection("DELIVERY")
                .WhereIn("Status", new object[] { "D", "A", "P" })
                .AddSnapshotListener((value, error) =>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var dc in value.DocumentChanges)
                        {
                            switch (dc.Type)
                            {
                                case DocumentChangeType.Added:
                                    items.Add(dc.Document.ToObject<DeliveryModal>());
                                    
                                    break;
                                case DocumentChangeType.Modified:
                                    break;
                                case DocumentChangeType.Removed:
                                    break;
                            }
                        }
                    }

                });

            include_app_toolbar.NavigationClick += Include_app_toolbar_NavigationClick1;

            Dexter.WithActivity(this)
                .WithPermission(Manifest.Permission.WriteExternalStorage)
                .WithListener(this)
                .Check();


            CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(FirebaseAuth.Instance.Uid)
                .AddSnapshotListener((snapshot, error) =>
                {
                    if (snapshot.Exists)
                    {
                        AppUsers user = snapshot.ToObject<AppUsers>();
                        PersonName = $"{user.Name} {user.Surname}";
                    }
                });

        }

        private void Include_app_toolbar_NavigationClick1(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }

        private void BtnYear_Click(object sender, EventArgs e)
        {
            PopupMenu popupMenu = new PopupMenu(this, BtnYear);
            int startYear = 2019;
            int diff = (DateTime.Now.Year - startYear) + 1;
            for (int i = 0; i < diff; i++)
            {
                popupMenu.Menu.Add(IMenu.First, 1, 1, (startYear + i).ToString());
            }
            popupMenu.Show();
            popupMenu.MenuItemClick += PopupMenu_MenuItemClick;
        }

        private void PopupMenu_MenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            BtnYear.Text = e.Item.TitleFormatted.ToString();
        }

        private void BtnMonth_Click(object sender, EventArgs e)
        {
            PopupMenu popupMenu = new PopupMenu(this, BtnYear);

            for (int i = 0; i < months.Length - 1; i++)
            {
                popupMenu.Menu.Add(IMenu.First, 1, 1, months[i]);
                if (months[i] == DateTime.Now.ToString("MMMM") && BtnYear.Text == DateTime.Now.Year.ToString())
                {
                    break;
                }
            }


            popupMenu.Show();
            popupMenu.MenuItemClick += PopupMenu_MenuItemClick1;
        }

        private void PopupMenu_MenuItemClick1(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            BtnMonth.Text = $"{e.Item.TitleFormatted}";
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            BtnGenerate.Text = "Please wait...";
            BtnGenerate.Enabled = false;
            GenerateInvoice();
            BtnGenerate.Text = "Generate";
            BtnGenerate.Enabled = true;
        }
        private async void GenerateInvoice()
        {

            string dname = "Invoice" + " " + DateTime.Now.ToString("dddd dd MMM HH mm ss tt") + ".pdf";
            if (new Java.IO.File(dname).Exists())
            {
                new Java.IO.File(dname).Delete();
            }
            FlowDocument document = new FlowDocument();
            document.StyleManager.RegisterStyle("gridrow.tableHeader", new Style() { Background = RgbColors.LightSlateGray });
            document.StyleManager.RegisterStyle("gridrow.centerAlignedCells > *,gridrow > *.centerAlignedCell", new Style() { Align = Align.Center, Margin = new Thickness(10, 0, 0, 0) });
            document.StyleManager.RegisterStyle("gridrow > *.leftAlignedCell", new Style() { Align = Align.Left, Margin = new Thickness(5, 0, 0, 0) });
            document.StyleManager.RegisterStyle("gridrow > *", new Style() { Align = Align.Right, Margin = new Thickness(0, 0, 5, 0) });




            ResourceManager resourceManager = new ResourceManager();

            document.PageHeader.Margin = new Thickness(0, 40, 0, 20);
            document.PageHeader.Padding = new Thickness(10, 0, 10, 0);
            document.PageHeader.Height = 120;
            document.PageHeader.Background = RgbColors.LightBlue;
            document.PageHeader.LineHeight = 60;
            document.PageHeader.Add(new TextBlock("Invoice")
            {
                //Display = Display.InlineBlock,
                Align = Align.Right,
            });
            Section pageSection = new Section() { Padding = new Thickness(20) };




            pageSection.AddItems(CreateInfoSubsections(new string[] { "Dates: ", DateTime.Now.ToString("dddd, dd/MMMM/yyyy") }));
            pageSection.AddItems(CreateInfoSubsections(new string[] { "Time: ", DateTime.Now.ToString("HH:mm:ss tt") }));
            pageSection.AddItems(CreateInfoSubsections(new string[] { "Generated By: ", PersonName }));
            pageSection.Add(new Hr() { Padding = new Thickness(0, 20, 0, 20) });
            pageSection.Add(new Br { Height = 20 });
            //pageSection.Add(new Section() { Width = 250, Display = Display.InlineBlock });

            document.Add(pageSection);

            document.Add(CreateInvoice());



            await Task.Run(() =>
            {
                document.Write(new FileStream(PrintReport.GetAppPath(this) + dname, FileMode.Create, FileAccess.Write), resourceManager, new Apitron.PDF.Kit.FixedLayout.PageProperties.PageBoundary(Apitron.PDF.Kit.FixedLayout.Boundaries.A4));
            });


            PrintReport.WriteFileToStorage(this, dname);
            PrintManager printManager = (PrintManager)this.GetSystemService(Context.PrintService);
            Common.PrintReportPDFAdapter _adapter = new Common.PrintReportPDFAdapter(this, Common.PrintReport.GetAppPath(this)
                    + dname, dname);
            printManager.Print("Document", _adapter, new PrintAttributes.Builder().Build());
        }

        private Grid CreateInvoice()
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            Grid requestsGrid = new Grid(20, Length.Auto, Length.Auto, Length.Auto)
            {
                new GridRow(new TextBlock("#"), new TextBlock("Delivered Requests"), new TextBlock("Total Amount"),
                new TextBlock("Total Requests"))
                {
                    Class = "tableHeader centerAlignedCells"
                }
            };
            int counter = 0;
            int totalRequests = 0;
            double countAmout = 0.0;

            for (int i = 0; i < items.Count; i++)
            {
                string time_stamp = $"{items[i].TimeStamp.ToDateTime():dddd, dd MMMM yyyy, HH:mm tt}";
                if (time_stamp.Contains(BtnMonth.Text) && time_stamp.Contains(BtnYear.Text))
                {
                    if (items[i].Status == "D")
                    {
                        countAmout += double.Parse(items[i].Price);
                        counter++;
                    }
                    totalRequests++;
                }
            }

            requestsGrid.Add(new GridRow(new TextBlock("01"), new TextBlock(counter.ToString()), new TextBlock($"R{countAmout}"), new TextBlock(totalRequests.ToString())));
            return requestsGrid;
        }

        private IList<Section> CreateInfoSubsections(string[] info)
        {
            System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
            cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;
            List<Section> sections = new List<Section>();
            double width = 100 / info.Length;
            for (int i = 0; i < info.Length; i++)
            {
                Section ss = new Section()
                {
                    Width = Length.FromPercentage(width),
                    //Display = Display.InlineBlock
                };

                ss.Add(new TextBlock(info[i]));
                ss.Add(new Br());
                sections.Add(ss);
            }




            return sections;
        }

        public void OnPermissionDenied(PermissionDeniedResponse p0)
        {
        }

        public void OnPermissionGranted(PermissionGrantedResponse p0)
        {
        }

        public void OnPermissionRationaleShouldBeShown(PermissionRequest p0, IPermissionToken p1)
        {
        }



    }
}
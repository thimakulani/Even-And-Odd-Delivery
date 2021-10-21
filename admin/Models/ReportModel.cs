using Android.Content;
using Apitron.PDF.Kit;
using Apitron.PDF.Kit.FixedLayout.Resources;
using Apitron.PDF.Kit.FlowLayout.Content;
using Apitron.PDF.Kit.Styles;
using Apitron.PDF.Kit.Styles.Appearance;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;


namespace admin.Models
{
    class ReportModel
    {
        private readonly List<DeliveryModal> reportData = new List<DeliveryModal>();

        public ReportModel(List<DeliveryModal> reportData)
        {
            this.reportData = reportData;

        }

        public void GenerateInvoice(System.IO.Stream stream)
        {
            FlowDocument document = new FlowDocument();

            document.StyleManager.RegisterStyle("gridrow.tableHeader",
            new Apitron.PDF.Kit.Styles.Style() { Background = RgbColors.LightSlateGray });

            document.StyleManager.RegisterStyle("gridrow.centerAlignedCells > *, gridrow >*.centerAlignedCell",
            new Style() { Align = Align.Center, Margin = new Thickness(0) });

            document.StyleManager.RegisterStyle("gridrow > *.leftAlignedCell",
            new Style() { Align = Align.Left, Margin = new Thickness(5, 0, 0, 0) });

            document.StyleManager.RegisterStyle("gridrow > *",
            new Style() { Align = Align.Right, Margin = new Thickness(0, 0, 5, 0) });

            ResourceManager resourceManager = new ResourceManager();
            //resourceManager.RegisterResource(new Apitron.PDF.Kit.FixedLayout.Resources.XObjects.Image("logo",
            //Path.Combine(fileName, "storeLogo.png"), true)
            //{ Interpolate = true });

            document.PageHeader.Margin = new Thickness(0, 20, 0, 20);
            document.PageHeader.Padding = new Thickness(10, 0, 10, 0);
            document.PageHeader.Height = 120;
            document.PageHeader.Background = RgbColors.LightGray;
            document.PageHeader.LineHeight = 60;
         

            document.PageHeader.Add(new TextBlock("REPORT")
            {
                Display = Display.InlineBlock,
                Align = Align.Right,

                //Font = new Apitron.PDF.Kit.FixedLayout.Resources.Fonts.Font(StandardFonts.CourierBold, 20),
                Color = RgbColors.Black
            });
            Section pageSection = new Section() { Padding = new Thickness(20) };


            Section head = new Section()
            {
                Border = new Border(1),
                BorderColor = RgbColors.Blue,
                Padding = new Thickness(Length.FromPoints(5)),
                Width = Length.FromPercentage(100),

            };
            head.Add(new TextBlock("Even & Odds Delivery"));
            head.Add(new Br());
            head.Add(new TextBlock(DateTime.Now.ToString("dddd dd-MMMM-yyyy HH:mm:ss tt")));

            document.Add(head);
            //Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            //keyValuePairs.Add("Dates: ", );
            //keyValuePairs.Add("Company ", DateTime.Now.ToString("Even and Odds"));



            //pageSection.AddItems(CreateInfoSubsections(keyValuePairs));
            pageSection.Add(new Hr() { Padding = new Thickness(0, 20, 0, 20) });
            pageSection.Add(CreateRequestsGrid());
            // add new line after grid
            pageSection.Add(new Br { Height = 20 });

            document.Add(pageSection);

            document.Write(stream, resourceManager, new Apitron.PDF.Kit.FixedLayout.PageProperties.PageBoundary(Apitron.PDF.Kit.FixedLayout.Boundaries.A4));


        }
        private  Grid  CreateRequestsGrid()
        {
            Grid requestsGrid = new Grid(20, Length.Auto, Length.Auto, Length.Auto, Length.Auto, Length.Auto)
            {
                new GridRow(new TextBlock("#"), new TextBlock("Driver"), new TextBlock("Pickup Location"),
                new TextBlock("Destination"), new TextBlock("Dates"), new TextBlock("Trip Status"))
                {
                    Class = "tableHeader centerAlignedCells"
                }
            };
            int counter = 0; 

            foreach (var item in reportData)
            {
                counter++;
                TextBlock pos = new TextBlock(counter.ToString("0#"));
                TextBlock driver;
                if (item.DriverId == null)
                {
                    driver = new TextBlock("---------------");
                }
                else
                {
                    var results = CrossCloudFirestore
                        .Current
                        .Instance
                        .Collection("USERS")
                        .Document(item.DriverId)
                        .GetAsync().Result;
                    if (results.Exists)
                    {
                        var user = results.ToObject<AppUsers>();
                        driver = new TextBlock($"{user.Name} {user.Surname}");
                    }
                    else
                    {
                        driver = new TextBlock("---------------");
                    }
                        
                    
                }
                TextBlock pickup = new TextBlock(item.PickupAddress);
                TextBlock dest = new TextBlock(item.DestinationAddress);
                TextBlock dates = new TextBlock(item.RequestTime);


                string s = null;
                switch (item.Status)
                {
                    case "W":
                        s = "Waiting for driver";
                        break;
                    case "A":
                        s = "Accepted";
                        break;
                    case "P":
                        s = "Picked up";
                        break;
                    case "D":
                        s = "Delivered";
                        break;
             
                }
                TextBlock status = new TextBlock(s);

                requestsGrid.Add(new GridRow(pos, driver, pickup, dest, dates, status));

            }


            return requestsGrid;
        }
    }
}
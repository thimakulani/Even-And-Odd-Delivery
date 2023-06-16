using admin.Models;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Google.Android.Material.AppBar;
using Google.Android.Material.Button;
using Microcharts;
using Microcharts.Droid;
using Plugin.CloudFirestore;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace admin.Activities
{
    [Activity(Label = "StatsActivity")]
    public class StatsActivity : Activity
    {
        private ChartView chartStats;
        private MaterialToolbar toolbar;
        private ProgressBar loading_stats_progress;
        private readonly List<string> months = new List<string>();
        private readonly List<int> counter = new List<int>();
        private MaterialButton BtnChartType;
        private MaterialButton BtnYear;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_graphs_stats);
            toolbar = FindViewById<MaterialToolbar>(Resource.Id.include_app_toolbar);
            BtnChartType = FindViewById<MaterialButton>(Resource.Id.BtnChartType);
            BtnYear = FindViewById<MaterialButton>(Resource.Id.BtnYear);
            loading_stats_progress = FindViewById<ProgressBar>(Resource.Id.loading_stats_progress);
            //BtnYear.Click += BtnYear_Click;
            BtnYear.Visibility = ViewStates.Gone;
            BtnChartType.Click += BtnChartType_Click;

            chartStats = FindViewById<ChartView>(Resource.Id.chartStats);
            toolbar.NavigationClick += Toolbar_NavigationClick1;
            string[] monthNames = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames;
            foreach (var m in monthNames)
            {
                months.Add(m);
                counter.Add(0);
            }


            CrossCloudFirestore
                .Current
                .Instance
                .Collection("REQUESTS")
                .OrderBy("TimeStamp", false)
                .AddSnapshotListener(true, (value, error) =>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var data in value.DocumentChanges)
                        {
                            var delivery = data.Document.ToObject<DeliveryModal>();

                            DateTime dateTime = DateTime.Parse(delivery.RequestTime);
                            if (months.Contains(dateTime.ToString("MMMM")))
                            {
                                int pos = months.IndexOf(dateTime.ToString("MMMM"));
                                counter[pos] = counter[pos] + 1;
                            }
                        }
                        DrawCharts();
                    }
                });

        }

        private void Toolbar_NavigationClick1(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Finish();
        }

        private void BtnChartType_Click(object sender, EventArgs e)
        {
            Android.Widget.PopupMenu popupMenu = new Android.Widget.PopupMenu(this, BtnChartType);
            popupMenu.Menu.Add(IMenu.First, 0, 1, "Bar Chart");
            popupMenu.Menu.Add(IMenu.First, 1, 1, "Line Chart");
            popupMenu.Menu.Add(IMenu.First, 2, 1, "Point Chart");
            popupMenu.Menu.Add(IMenu.First, 3, 1, "Donut Chart");
            popupMenu.Menu.Add(IMenu.First, 4, 1, "Radar Chart");
            popupMenu.Show();
            popupMenu.MenuItemClick += PopupMenu_MenuItemClick;
        }
        // int Year = 2020;
        //private void BtnYear_Click(object sender, EventArgs e)
        //{
        //    Android.Widget.PopupMenu popupYear = new Android.Widget.PopupMenu(this, BtnYear);
        //    var currentYear = DateTime.Now.Year;


        //    while (Year <= currentYear)
        //    {
        //        popupYear.Menu.Add(IMenu.First, 0, 1, Year.ToString());
        //        Year++;

        //    }

        //    popupYear.Show();
        //    popupYear.MenuItemClick += PopupYear_MenuItemClick;


        //}

        private void PopupYear_MenuItemClick(object sender, Android.Widget.PopupMenu.MenuItemClickEventArgs e)
        {
            BtnYear.Text = e.Item.TitleFormatted.ToString();
        }

        private void PopupMenu_MenuItemClick(object sender, Android.Widget.PopupMenu.MenuItemClickEventArgs e)
        {
            type = e.Item.ItemId;
            BtnYear.Text = e.Item.TitleFormatted.ToString();
            DrawCharts();
        }

        private int type = 0;
        private void DrawCharts()
        {
            List<ChartEntry> DataEntry = new List<ChartEntry>();
            string[] colors = { "#157979", "#154779", "#5F1C80", "#801C59",
                            "#9CBDD6", "#75863D" , "#1E1011", "#48D53B",
                            "#48D5C7", "#6761F0", "#8A80A3", "#D3C6F4"
            };
            for (int i = 0; i < months.Count; i++)
            {
                DataEntry.Add(new ChartEntry(counter[i])
                {
                    Label = months[i],
                    Color = SKColor.Parse(colors[i]),
                    ValueLabel = counter[i].ToString(),
                    TextColor = SKColor.Parse(colors[i]),
                    ValueLabelColor = SKColor.Parse(colors[i])
                });
                if (months[i].Contains(DateTime.Now.ToString("MMMM")))
                {
                    break;
                }
            }
            loading_stats_progress.Visibility = ViewStates.Gone;

            if (type == 0)
            {
                var chart = new BarChart()
                {
                    Entries = DataEntry,
                };
                chartStats.Chart = chart;
            }
            if (type == 1)
            {
                var chart = new LineChart()
                {
                    Entries = DataEntry,
                };
                chartStats.Chart = chart;
            }
            if (type == 2)
            {
                var chart = new PointChart()
                {
                    Entries = DataEntry,
                };
                chartStats.Chart = chart;
            }
            if (type == 3)
            {
                var chart = new DonutChart()
                {
                    Entries = DataEntry,
                };
                chartStats.Chart = chart;
            }
            if (type == 4)
            {
                var chart = new RadarChart()
                {
                    Entries = DataEntry,
                };
                chartStats.Chart = chart;
            }
        }



    }
}
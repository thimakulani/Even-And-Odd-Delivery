using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidAboutPage;
using System;
using Xamarin.Essentials;

namespace driver.Activities
{
    [Activity(Label = "About")]
    public class About : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            //  SetContentView(Resource.Layout.activity_about);
            RequestedOrientation = ScreenOrientation.Portrait;
            Element element = new Element
            {
                Title = "About"
            };

            View about = new AboutPage(this)
                .IsRtl(false)
                .SetImage(Resource.Drawable.delivery_icon_2)
                .SetDescription("Even & odds delivery")
                .AddItem(new Element() { Title = $"Version {AppInfo.VersionString}" })
                .AddItem(element)
                .AddGroup("Contact us on")
                .AddEmail("")
                .AddFacebook("Thima")
                .AddItem(CreateCopyright())
                .AddGroup("Developers")
                .AddEmail("thimakulani@gmail.com")
                .AddFacebook("https//m.facebook.com/thima.sigauque")
                .AddGitHub("Thimakulani")
                .AddInstagram("instagram.com/thimasigauque")
                .Create();
            SetContentView(about);
        }

        private Element CreateCopyright()
        {
            Element copy = new Element();
            string cr = $"Copyright {DateTime.Now.Year} by Thima Kulani";
            copy.Title = cr;
            copy.IconDrawable = Resource.Drawable.delivery_icon_2;
            return copy;
        }
    }
}
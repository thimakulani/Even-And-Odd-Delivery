using Android.Content;
using Android.OS;
using Android.Views;
using AndroidAboutPage;
using AndroidX.Fragment.App;
using System;
using Xamarin.Essentials;

namespace client.Fragments
{
    public class AboutFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            base.OnCreateView(inflater, container, savedInstanceState);
            Element element = new Element
            {
                Title = "About"
            };

            View about = new AboutPage(Context.ApplicationContext)
                .IsRtl(false)
                .SetImage(Resource.Drawable.delivery_icon)
                .AddGroup("Even & Odds delivery")
                .SetDescription("Growing group of people desires faster home delivery, yet most are highly price sensitive Moving goods from one place to another is never an easy to go task if you don't own a car")
                .AddItem(new Element() { Title = $"Version {VersionTracking.CurrentVersion}" })
                .AddItem(element)
                .AddGroup("Contact us on")
                .AddEmail("even.odds72@gmail.com")
                //.AddFacebook("Thima")
                .AddItem(CreateCopyright())
                .AddGroup("Developer")
                .AddEmail("thimakulani@gmail.com")
                .AddFacebook("thima.sigauque")
                .AddGitHub("Thimakulani")
                .AddInstagram("thimasigauque")
                .Create();
            return about;


        }
        private Element CreateCopyright()
        {
            Element copy = new Element();
            string cr = $"Copyright {DateTime.Now.Year} by Thima Kulani";
            copy.Title = cr;
            copy.IconDrawable = Resource.Mipmap.ic_copyright_black_18dp;
            return copy;
        }
    }
}
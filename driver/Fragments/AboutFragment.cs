using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using AndroidAboutPage;
using System;

namespace driver.Fragments
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
                .SetImage(Resource.Drawable.delivery_icon_2)
                .SetDescription("Even & Odds delivery")
                .AddItem(new Element() { Title = "Version 1.0.0" })
                .AddItem(element)
                .AddGroup("Contact us on")
                .AddEmail("thimakulani@gmail.com")
                .AddFacebook("Thima")
                // .AddWebsite("")
                .AddPlayStore("Thima")
                .AddItem(CreateCopyright())
                .AddGroup("Developers")
                .AddEmail("thimakulani@gmail.com")
                .AddFacebook("https//m.facebook.com/thima.sigauque")
                .AddGitHub("Thimakulani")
                .AddInstagram("instagram.com/thimasigauque")
                .Create();
            return about;
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
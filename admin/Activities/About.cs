using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using AndroidAboutPage;
using System;

namespace admin.Activities
{
    [Activity(Label = "About")]
    public class About : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            //  SetContentView(Resource.Layout.activity_about);
            Element element = new Element
            {
                Title = "About"
            };

            View about = new AboutPage(this)
                .IsRtl(false)
                .SetImage(Resource.Drawable.delivery_icon_2)
                .SetDescription("This Even and odds delivery \n  growing group of people desires faster home delivery, yet most are highly price sensitive Moving goods from one place to another is never an easy to go task if you don't own a car")
                .AddItem(new Element() { Title = "Version 1.0.0" })
                .AddItem(element)
                .AddGroup("Contact us on")
                .AddEmail("thimakulani@gmail.com")
                .AddFacebook("Thima")
                .AddItem(CreateCopyright())
                .AddGroup("Developer")
                .AddEmail("thimakulani@gmail.com")
                .AddFacebook("thima.sigauque")
                .AddGitHub("Thimakulani")
                .AddInstagram("thimasigauque")
                .Create();
            SetContentView(about);
        }

        private Element CreateCopyright()
        {
            Element copy = new Element();
            string cr = $"Copyright {DateTime.Now.Year} developed by Thima Kulani Sigauque";
            copy.Title = cr;
            copy.IconDrawable = Resource.Drawable.ic_copyright_black_18dp;
            return copy;
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
        }
    }
}
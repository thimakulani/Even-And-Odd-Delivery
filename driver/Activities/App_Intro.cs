using Android.App;
using Android.Content;
using Android.OS;
using System.Drawing;
using Xamarin.Essentials;

namespace driver.Activities
{
    [Activity(Label = "AppIntro")]
    public class App_Intro : AppIntro.AppIntro
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            AddSlide(AppIntro.AppIntroFragment.NewInstance("xx", "xx",
                Resource.Drawable.delivery_icon,
                Color.Green.ToInt()));
            AddSlide(AppIntro.AppIntroFragment.NewInstance("yy", "yy",
                Resource.Drawable.delivery_icon_2,
                Color.Green.ToInt()));
            AddSlide(AppIntro.AppIntroFragment.NewInstance("zz", "zz",
                Resource.Drawable.delivery_icon_3,
                Color.Green.ToInt()));
        }
        public override void OnDonePressed()
        {
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences("AppInto", FileCreationMode.Private);
            ISharedPreferencesEditor edit = sharedPreferences.Edit();
            edit.PutString("AppInto", "Done");
            edit.Apply();
            //base.OnDonePressed();
            Intent intent = new Intent(this, typeof(Login));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);

        }
        public override void OnSkipPressed()
        {
            ISharedPreferences sharedPreferences = Application.Context.GetSharedPreferences("AppInto", FileCreationMode.Private);
            ISharedPreferencesEditor edit = sharedPreferences.Edit();
            edit.PutString("AppInto", "Done");
            edit.Apply();

            Intent intent = new Intent(this, typeof(Login));
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
        }
    }
}
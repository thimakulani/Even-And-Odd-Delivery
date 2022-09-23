using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using client.Activities;
using Firebase.Auth;
using System;
using System.Threading.Tasks;

namespace client
{
    [Activity(Label = "@string/app_name", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashScreen : Activity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            RequestedOrientation = ScreenOrientation.Portrait;

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
        }
        protected override void OnResume()
        {
            base.OnResume();
            Task startWork = new Task(() =>
            {
                Task.Delay(3000);
            });
            startWork.ContinueWith(t =>
            {
                try
                {
                    var user = FirebaseAuth.Instance.CurrentUser;
                    if (user != null)
                    {
                        Intent intent = new Intent(Application.Context, typeof(MainActivity));
                        StartActivity(intent);
                        OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                    }
                    else
                    {
                        Intent intent = new Intent(Application.Context, typeof(Login));
                        StartActivity(intent);
                        OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Errr", ex.Message);
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
            startWork.Start();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}
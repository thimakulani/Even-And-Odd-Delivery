using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using client.Activities;
using client.Classes;
using Firebase.Auth;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;

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
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                string e = Preferences.Get("e", null);
                string p = Preferences.Get("p", null);
                try
                {
                    if (string.IsNullOrWhiteSpace(e) || string.IsNullOrWhiteSpace(p))
                    {
                        Intent intent = new Intent(Application.Context, typeof(Login));
                        StartActivity(intent);
                        OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                        return;
                    }
                    UserLogin userLogin = new UserLogin()
                    {
                        Email = e,
                        Password = p,
                    };
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);



                    HttpClient httpClient = new HttpClient();
                    HttpContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync($"{API.ApiUrl}/account/login", httpContent);
                    if (response.IsSuccessStatusCode)
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
                catch (Exception)
                {
                    Intent intent = new Intent(Application.Context, typeof(Login));
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.Side_in_right, Resource.Animation.Side_out_left);
                }

            });
        }
        /*        protected override void OnResume()
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
                }*/
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}
public class UserLogin
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class UpdateUser
{
    public string Id { get; set; }
    public string Url { get; set; }
}

public class UserSignUp
{

    public string Name { get; set; }
    public string Email { get; set; }
    public string Surname { get; set; }
    public string Phone { get; set; }
    public string Role { get; set; }
    public string RegNo { get; set; }
    public string Type { get; set; }
    public string Color { get; set; }
    public string Make { get; set; }
    public string Status { get; set; }
    public string Password { get; set; }
}
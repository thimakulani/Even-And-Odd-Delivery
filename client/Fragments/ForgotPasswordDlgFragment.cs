using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.Fragment.App;
using client.Classes;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using ID.IonBit.IonAlertLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using static Firebase.Firestore.Local.LruGarbageCollector;

namespace client.Fragments
{
    public class ForgotPasswordDlgFragment : DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        Context context;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.reset_password_dialog, container, false);
            context = view.Context;
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            var InputEmail = view.FindViewById<TextInputEditText>(Resource.Id.ResetInputEmail);
            var InputNewPassword = view.FindViewById<TextInputEditText>(Resource.Id.ResetInputPassword);
            var BtnReset = view.FindViewById<MaterialButton>(Resource.Id.BtnReset);
            var FabClose = view.FindViewById<FloatingActionButton>(Resource.Id.FabCloseResetDialog);
            var loadingDialog = new IonAlert(view.Context, IonAlert.ProgressType);

            BtnReset.Click += async (s, e) =>
            {
                loadingDialog.SetSpinKit("FoldingCube")
                    .SetSpinColor("#008D91")
                    .ShowCancelButton(false)
                    .Show();
                if (string.IsNullOrEmpty(InputEmail.Text))
                {
                    InputEmail.Error = "provide your email";
                    return;
                }
                if (string.IsNullOrEmpty(InputNewPassword.Text))
                {
                    InputNewPassword.Error = "provide your password";
                    return;
                }
                UserLogin userLogin = new UserLogin()
                {
                    Email = InputEmail.Text.Trim(),
                    Password = InputNewPassword.Text.Trim()
                };
                try
                {
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(userLogin);
                    HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpClient httpClient = new HttpClient();
                    var response = await httpClient.PostAsync($"{API.ApiUrl}/account/resetpassword", httpContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var results = await response.Content.ReadAsStringAsync();
                        AndHUD.Shared.ShowSuccess(context, $"{results}", MaskType.Black, TimeSpan.FromSeconds(2));
                        Dismiss();
                    }
                    else
                    {
                        var results = await response.Content.ReadAsStringAsync();
                        AndHUD.Shared.ShowError(context, $"{results}", MaskType.Black, TimeSpan.FromSeconds(2));
                    }
                }
                catch (Exception ex)
                {
                    AndHUD.Shared.ShowError(context, $"{ex.Message}", MaskType.Black, TimeSpan.FromSeconds(2));

                }
                finally
                {
                    loadingDialog.Dismiss();
                }
            };
            FabClose.Click += (s, e) =>
            {
                Dismiss();
            };


        }
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
    }
}
using Android.OS;
using Android.Views;
using Google.Android.Material.Button;
using System;

namespace driver.Fragments
{
    public class WelcomeFragment : Android.Support.V4.App.Fragment
    {



        private MaterialButton RequestBtn;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.welcome_fragment, container, false);
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View view)
        {
            RequestBtn = view.FindViewById<MaterialButton>(Resource.Id.BtnGoOnline);
            RequestBtn.Click += RequestBtn_Click;
        }
        public event EventHandler RequestEventHandler;
        private void RequestBtn_Click(object sender, EventArgs e)
        {
            RequestEventHandler(sender, e);
        }
    }
}
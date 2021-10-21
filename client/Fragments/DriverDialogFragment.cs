using Android.OS;
using AndroidX.Fragment.App;
using Android.Views;
using Android.Widget;
using client.Classes;
using Google.Android.Material.FloatingActionButton;
using Plugin.CloudFirestore;
using System;

namespace client.Fragments
{
    public class DriverDialogFragment : DialogFragment
    {
        private readonly string driverId;
        private string PhoneNo;

        public DriverDialogFragment(string driverId)
        {
            this.driverId = driverId;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        private TextView Names { get; set; }
        private TextView Surname { get; set; }
        private TextView Make { get; set; }
        private TextView Color { get; set; }
        private TextView RegNo { get; set; }
        private TextView Type { get; set; }

        private FloatingActionButton FabMakeCall { get; set; }
        private FloatingActionButton FabClose { get; set; }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment

            base.OnCreateView(inflater, container, savedInstanceState);
            View view = inflater.Inflate(Resource.Layout.driver_info_dialog, container, false);
            ConnectViews(view);
            return view;
        }

        private void ConnectViews(View itemView)
        {
            Make = itemView.FindViewById<TextView>(Resource.Id.RowUserMake);
            Color = itemView.FindViewById<TextView>(Resource.Id.RowUserColor);
            Type = itemView.FindViewById<TextView>(Resource.Id.RowUserCarType);
            RegNo = itemView.FindViewById<TextView>(Resource.Id.RowUserRegNo);

            //TextView = v;
            Surname = itemView.FindViewById<TextView>(Resource.Id.RowSurname);
            Names = itemView.FindViewById<TextView>(Resource.Id.RowNames);
            FabMakeCall = itemView.FindViewById<FloatingActionButton>(Resource.Id.RowFabCall);
            FabClose = itemView.FindViewById<FloatingActionButton>(Resource.Id.FabClose);
            FabMakeCall.Click += FabMakeCall_Click;
            FabClose.Click += FabClose_Click;
            
            CrossCloudFirestore.
                Current
                .Instance
                .Collection("USERS")
                .Document(driverId)
                .AddSnapshotListener((value, error) =>
                {
                    if (value.Exists)
                    {
                        var user = value.ToObject<AppUsers>();
                        Make.Text = user.Make;
                        Color.Text = user.Color;
                        Type.Text = user.Type;
                        RegNo.Text = user.RegNo;
                        Surname.Text = user.Surname;
                        Names.Text = user.Name;
                        Make.Text = user.Make;
                        PhoneNo = user.Phone;
                    }
                });



        }
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
        }
        private void FabMakeCall_Click(object sender, EventArgs e)
        {
            try
            {

                Xamarin.Essentials.PhoneDialer.Open(PhoneNo);
            }
            catch
            {

            }
        }

        private void FabClose_Click(object sender, EventArgs e)
        {
            Dismiss();
        }


    }
}
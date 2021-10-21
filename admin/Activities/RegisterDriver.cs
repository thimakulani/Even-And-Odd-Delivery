using admin.Adapters;
using admin.Fragments;
using admin.Models;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.TextView;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using AlertDialog = Android.App.AlertDialog;

namespace admin.Activities
{
    [Activity(Label = "RegisterDriver", MainLauncher = false)]
    public class RegisterDriver : AppCompatActivity
    {

        private readonly List<AppUsers> UseritemsList = new List<AppUsers>();
        // private RecyclerView RecyclerUserList;
        private AppUsersAdapter adapter;
        private MaterialButton txtCreateDriver;
        /*views*/

        private Android.Support.V7.Widget.SearchView InputSearchUser;

        private ImageView ImgBack;
        private RecyclerView recyclerUsersList;
        private MaterialTextView txt_user_count;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_driver_registration);

            ConnectViews();
        }
        private void ConnectViews()
        {
            ImgBack = FindViewById<ImageView>(Resource.Id.imgCloseSignUp);

            // FabSearch = FindViewById<FloatingActionButton>(Resource.Id.FabRearch);
            InputSearchUser = FindViewById<Android.Support.V7.Widget.SearchView>(Resource.Id.InputSearchUsers);
            InputSearchUser.Visibility = ViewStates.Visible;
            txt_user_count = FindViewById<MaterialTextView>(Resource.Id.txt_user_count);
            recyclerUsersList = FindViewById<RecyclerView>(Resource.Id.recyclerUsersList);
            InputSearchUser.Visibility = ViewStates.Gone;
            // InputSearchUser.QueryTextChange += InputSearchUser_QueryTextChange;
            txtCreateDriver = FindViewById<MaterialButton>(Resource.Id.txtCreateDriver);


            ImgBack.Click += ImgBack_Click;
            // FabSearch.Click += FabSearch_Click;
            txtCreateDriver.Click += TxtCreateDriver_Click;
            txtCreateDriver.Visibility = ViewStates.Gone;
            SetUpRecycler(UseritemsList);

            CrossCloudFirestore
                 .Current
                 .Instance
                 .Collection("USERS")
                 .WhereIn("Role", new object[] { "D", "C"})
                 .AddSnapshotListener((values, error) =>
                 {
                     if (!values.IsEmpty)
                     {
                         foreach (var item in values.DocumentChanges)
                         {
                             AppUsers users = new AppUsers();
                             switch (item.Type)
                             {
                                 case DocumentChangeType.Added:
                                     users = item.Document.ToObject<AppUsers>();
                                     UseritemsList.Add(users);
                                     adapter.NotifyDataSetChanged();
                                     break;
                                 case DocumentChangeType.Modified:
                                     users = item.Document.ToObject<AppUsers>();
                                     UseritemsList[item.OldIndex] = users;
                                     adapter.NotifyDataSetChanged();
                                     break;
                                 case DocumentChangeType.Removed:
                                     break;
                                 default:
                                     break;
                             }
                         }
                         txt_user_count.Text = $"Number of Users: {values.Count}";

                     }
                 });



        }

        //private void InputSearchUser_QueryTextChange(object sender, Android.Support.V7.Widget.SearchView.QueryTextChangeEventArgs e)
        //{
        //    var users = (from data in UseritemsList
        //                 where
        //                data.Name.Contains(e.NewText) ||
        //                data.Role.Contains(e.NewText) ||
        //                data.Surname.Contains(e.NewText) ||
        //                data.Email.Contains(e.NewText) ||
        //                data.Phone.Contains(e.NewText)
        //                 select data).ToList<AppUsers>();
        //    UseritemsList = users;
        //    SetUpRecycler(users);
        //}



        private void TxtCreateDriver_Click(object sender, EventArgs e)
        {

            DriverRegistrationDialog dlg = new DriverRegistrationDialog();
            dlg.Show(SupportFragmentManager.BeginTransaction(), "Driver Reg");
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
        }
        private void ImgBack_Click(object sender, EventArgs e)
        {
            base.OnBackPressed();
            OverridePendingTransition(Resource.Animation.Side_in_left, Resource.Animation.Side_out_right);
        }
        private void SetUpRecycler(List<AppUsers> users)
        {
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(this);
            recyclerUsersList.SetLayoutManager(linearLayoutManager);
            adapter = new AppUsersAdapter(users);
            recyclerUsersList.SetAdapter(adapter);
            adapter.FabCallClick += Adapter_FabCallClick;
            adapter.FabEmailClick += Adapter_FabEmailClick;
            adapter.CreateDriverClick += Adapter_CreateDriverClick;

        }

        private void Adapter_CreateDriverClick(object sender, AppUsersAdapterClickEventArgs e)
        {
            if (UseritemsList[e.Position].Role != "D")
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Confirm");
                // builder.SetMessage("Reset password link has been sent to your email address");
                builder.SetMessage($"Are you sure you want to activate user: { UseritemsList[e.Position].Name}  {UseritemsList[e.Position].Surname} as a driver?");
                builder.SetPositiveButton("Yes", delegate
                {

                    UpdateExistingUserFragment userFragment = new UpdateExistingUserFragment(UseritemsList[e.Position])
                    {
                        Cancelable = false
                    };
                    userFragment.Show(SupportFragmentManager.BeginTransaction(), "Update User");
                    builder.Dispose();

                });
                builder.SetNegativeButton("No", delegate
                {
                    builder.Dispose();
                    return;
                });
                builder.Show();
            }
            else
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Confirm");
                builder.SetMessage($"Are you sure you want to change role to client for: {UseritemsList[e.Position].Name} { UseritemsList[e.Position].Surname} ?");
                builder.SetPositiveButton("Yes", async delegate
                 {
                     await CrossCloudFirestore
                         .Current
                         .Instance
                         .Collection("USERS")
                         .Document(UseritemsList[e.Position].Uid)
                         .UpdateAsync("Role", "C");
                     builder.Dispose();

                 });
                builder.SetNegativeButton("No", delegate
                {
                    builder.Dispose();
                    return;
                });
                builder.Show();

            }
        }

        private async void Adapter_FabEmailClick(object sender, AppUsersAdapterClickEventArgs e)
        {

            try
            {
                List<string> to = new List<string>
                {
                    UseritemsList[e.Position].Email
                };// 
                var message = new EmailMessage
                {
                    Subject = "Even & Odds Team",
                    Body = "Even & Odds Team",
                    To = to,

                    //Cc = ,
                    //Bcc = bccRecipients            
                };
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage(fbsEx.Message);
                builder.SetNeutralButton("Ok", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }
            catch (Exception ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage(ex.Message);
                builder.SetNeutralButton("Ok", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }

        }

        private void Adapter_FabCallClick(object sender, AppUsersAdapterClickEventArgs e)
        {
            try
            {
                PhoneDialer.Open(UseritemsList[e.Position].Phone);
            }
            catch (ArgumentNullException anEx)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage(anEx.Message);
                builder.SetNeutralButton("OK", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }
            catch (FeatureNotSupportedException ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage(ex.Message);
                builder.SetNeutralButton("OK", delegate
                {
                    builder.Dispose();
                });
                builder.Show();
            }
            catch (Exception ex)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetTitle("Error");
                builder.SetMessage(ex.Message);
                builder.SetNeutralButton("OK", delegate
                {
                    builder.Dispose();
                });
                builder.Show();

            }
        }
    }
}
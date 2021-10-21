using admin.Models;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.FloatingActionButton;
using System;
using System.Collections.Generic;

namespace admin.Adapters
{
    class AppUsersAdapter : RecyclerView.Adapter
    {
        public event EventHandler<AppUsersAdapterClickEventArgs> ItemClick;
        public event EventHandler<AppUsersAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<AppUsersAdapterClickEventArgs> FabCallClick;
        public event EventHandler<AppUsersAdapterClickEventArgs> FabEmailClick;
        public event EventHandler<AppUsersAdapterClickEventArgs> CreateDriverClick;

        private readonly List<AppUsers> items = new List<AppUsers>();
        public AppUsersAdapter(List<AppUsers> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;


            var id = Resource.Layout.users_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);
            var vh = new AppUsersAdapterViewHolder(itemView, OnClick, OnLongClick, OnCallClick, OnEmailClick, OnCreateDriverClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {


            // Replace the contents of the view with that element
            var holder = viewHolder as AppUsersAdapterViewHolder;
            //holder.TextView.Text = items[position];
            holder.Email.Text = items[position].Email;
            holder.Names.Text = items[position].Name;
            holder.Surname.Text = items[position].Surname;
            
            holder.PhoneNumber.Text = items[position].Phone;
           // holder.FabSendEmail.Visibility = ViewStates.Gone;

            if (items[position].Role == "D")
            {
                holder.BtnCreateDriver.Text = "Deactivate Driver";
                holder.RegNo.Text = items[position].RegNo;
                holder.Make.Text = items[position].Make;
                holder.Color.Text = items[position].Color;
                holder.Type.Text = items[position].Type;
                holder.UserType.Text = "Driver";
                holder.LinearLayoutCarInformatio.Visibility = ViewStates.Visible;

            }
            else
            {
                holder.BtnCreateDriver.Text = "Activate Driver";
                holder.LinearLayoutCarInformatio.Visibility = ViewStates.Gone;
                holder.UserType.Text = "Client";
            }
            //ViewAnimationUtils(viewHolder.ItemView, position);


        }

        public override int ItemCount => items.Count;

        void OnClick(AppUsersAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(AppUsersAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnCallClick(AppUsersAdapterClickEventArgs args) => FabCallClick.Invoke(this, args);
        void OnEmailClick(AppUsersAdapterClickEventArgs args) => FabEmailClick.Invoke(this, args);
        void OnCreateDriverClick(AppUsersAdapterClickEventArgs args) => CreateDriverClick.Invoke(this, args);

    }

    public class AppUsersAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView Names { get; set; }
        public TextView Surname { get; set; }
        public TextView PhoneNumber { get; set; }
        public TextView UserType { get; set; }
        public TextView Email { get; set; }
        public TextView Make { get; set; }
        public TextView Color { get; set; }
        public TextView RegNo { get; set; }
        public TextView Type { get; set; }
        public MaterialButton BtnCreateDriver { get; set; }
        //public FloatingActionButton FabSendEmail { get; set; }
        public FloatingActionButton FabMakeCall { get; set; }
        public LinearLayout LinearLayoutCarInformatio { get; set; }


        public AppUsersAdapterViewHolder(View itemView, Action<AppUsersAdapterClickEventArgs> clickListener,
                            Action<AppUsersAdapterClickEventArgs> longClickListener, Action<AppUsersAdapterClickEventArgs> MakeCallClickListener, Action<AppUsersAdapterClickEventArgs> SendEmailClickListener, Action<AppUsersAdapterClickEventArgs> CreateDriverClickListener) : base(itemView)
        {

            Make = itemView.FindViewById<TextView>(Resource.Id.RowUserMake);
            Color = itemView.FindViewById<TextView>(Resource.Id.RowUserColor);
            Type = itemView.FindViewById<TextView>(Resource.Id.RowUserCarType);
            RegNo = itemView.FindViewById<TextView>(Resource.Id.RowUserRegNo);
            LinearLayoutCarInformatio = itemView.FindViewById<LinearLayout>(Resource.Id.linearLayoutCarInformatio);

            //TextView = v;
            UserType = itemView.FindViewById<TextView>(Resource.Id.RowUserType);
            Surname = itemView.FindViewById<TextView>(Resource.Id.RowSurname);
            Names = itemView.FindViewById<TextView>(Resource.Id.RowNames);
            PhoneNumber = itemView.FindViewById<TextView>(Resource.Id.RowPhoneNumber);
            Email = itemView.FindViewById<TextView>(Resource.Id.RowEmail);
            //FabSendEmail = itemView.FindViewById<FloatingActionButton>(Resource.Id.RowFabEmail);
            FabMakeCall = itemView.FindViewById<FloatingActionButton>(Resource.Id.RowFabCall);
            BtnCreateDriver = itemView.FindViewById<MaterialButton>(Resource.Id.RowMakeDriver);


            itemView.Click += (sender, e) => clickListener(new AppUsersAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new AppUsersAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            FabMakeCall.Click += (sender, e) => MakeCallClickListener(new AppUsersAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            //FabSendEmail.Click += (sender, e) => SendEmailClickListener(new AppUsersAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            BtnCreateDriver.Click += (sender, e) => CreateDriverClickListener(new AppUsersAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });

        }
    }

    public class AppUsersAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}
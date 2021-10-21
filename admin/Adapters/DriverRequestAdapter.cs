using admin.Models;
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using System;
using System.Collections.Generic;

namespace admin.Adapters
{
    class DriverRequestAdapter : RecyclerView.Adapter
    {
        public event EventHandler<DriverRequestAdapterClickEventArgs> ItemClick;
        public event EventHandler<DriverRequestAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<DriverRequestAdapterClickEventArgs> BtnApproveClick;
        public event EventHandler<DriverRequestAdapterClickEventArgs> BtnDeclinelick;
        private readonly List<AppUsers> items = new List<AppUsers>();

        public DriverRequestAdapter(List<AppUsers> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.requests_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new DriverRequestAdapterViewHolder(itemView, OnClick, OnLongClick, OnBtnAcceptClick, OnBtnDeclineClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var holder = viewHolder as DriverRequestAdapterViewHolder;
            //holder.TextView.Text = items[position];
            holder.Email.Text = items[position].Email;
            holder.PhoneNumber.Text = items[position].Phone;
            holder.Surname.Text = items[position].Surname;
            holder.Names.Text = items[position].Name;
            holder.Make.Text = items[position].Make;
            holder.Color.Text = items[position].Color;
            holder.Type.Text = items[position].Type;
            holder.RegNo.Text = items[position].RegNo;

            if (string.IsNullOrEmpty(items[position].Role))
            {
                holder.Status.Text = "Pendding";
            }
            else
            {
                holder.Status.Text = "Approved";
                holder.BtnAccept.Visibility = ViewStates.Gone;
                holder.BtnDecline.Visibility = ViewStates.Gone;
            }


        }

        public override int ItemCount => items.Count;

        void OnBtnAcceptClick(DriverRequestAdapterClickEventArgs args) => BtnApproveClick?.Invoke(this, args);
        void OnBtnDeclineClick(DriverRequestAdapterClickEventArgs args) => BtnDeclinelick?.Invoke(this, args);
        void OnClick(DriverRequestAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(DriverRequestAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class DriverRequestAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView Names { get; set; }
        public TextView Surname { get; set; }
        public TextView PhoneNumber { get; set; }
        public TextView Status { get; set; }
        public TextView Email { get; set; }
        public TextView Make { get; set; }
        public TextView Color { get; set; }
        public TextView RegNo { get; set; }
        public TextView Type { get; set; }
        public MaterialButton BtnAccept { get; set; }
        public MaterialButton BtnDecline { get; set; }




        public DriverRequestAdapterViewHolder(View itemView, Action<DriverRequestAdapterClickEventArgs> clickListener,
                            Action<DriverRequestAdapterClickEventArgs> longClickListener,
                            Action<DriverRequestAdapterClickEventArgs> AcceptClickListener,
                            Action<DriverRequestAdapterClickEventArgs> DeclineClickListener
                            ) : base(itemView)
        {

            Make = itemView.FindViewById<TextView>(Resource.Id.DR_RowMake);
            Color = itemView.FindViewById<TextView>(Resource.Id.DR_RowColor);
            Type = itemView.FindViewById<TextView>(Resource.Id.DR_RowType);
            RegNo = itemView.FindViewById<TextView>(Resource.Id.DR_RowRegNo);



            Status = itemView.FindViewById<TextView>(Resource.Id.DR_RowStatus);
            Surname = itemView.FindViewById<TextView>(Resource.Id.DR_RowSurname);
            Names = itemView.FindViewById<TextView>(Resource.Id.DR_RowNames);
            PhoneNumber = itemView.FindViewById<TextView>(Resource.Id.DR_RowPhoneNumber);
            Email = itemView.FindViewById<TextView>(Resource.Id.DR_RowEmail);
            BtnDecline = itemView.FindViewById<MaterialButton>(Resource.Id.DR_RowDecline);
            BtnAccept = itemView.FindViewById<MaterialButton>(Resource.Id.DR_RowApprove);


            //TextView = v;
            BtnDecline.Click += (sender, e) => DeclineClickListener(new DriverRequestAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            BtnAccept.Click += (sender, e) => AcceptClickListener(new DriverRequestAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.Click += (sender, e) => clickListener(new DriverRequestAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new DriverRequestAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class DriverRequestAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}
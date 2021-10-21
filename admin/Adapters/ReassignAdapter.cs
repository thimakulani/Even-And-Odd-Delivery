using admin.Models;
using Android.Views;
using AndroidX.RecyclerView.Widget;
using Android.Widget;
using Google.Android.Material.Button;
using System;
using System.Collections.Generic;

namespace admin.Adapters
{
    class ReassignAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ReassignAdapterClickEventArgs> BtnRejectClick;
        public event EventHandler<ReassignAdapterClickEventArgs> BtnClick;
        public event EventHandler<ReassignAdapterClickEventArgs> ItemClick;
        public event EventHandler<ReassignAdapterClickEventArgs> ItemLongClick;
        private List<ReasignModel> items = new List<ReasignModel>();

        public ReassignAdapter(List<ReasignModel> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.reasign_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new ReassignAdapterViewHolder(itemView, OnClick, OnLongClick, OnBtnClick, OnBtnRejectClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as ReassignAdapterViewHolder;
            holder.TxtDriver.Text = items[position].DriverNames;
            holder.TxtReason.Text = items[position].DriverNames;
            holder.TxtLocation.Text = items[position].DriverNames;
        }

        public override int ItemCount => items.Count;

        void OnBtnRejectClick(ReassignAdapterClickEventArgs args) => BtnRejectClick?.Invoke(this, args);
        void OnBtnClick(ReassignAdapterClickEventArgs args) => BtnClick?.Invoke(this, args);
        void OnClick(ReassignAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ReassignAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class ReassignAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TxtReason { get; set; }
        public TextView TxtDriver { get; set; }
        public TextView TxtLocation { get; set; }
        public MaterialButton BtnReassign { get; set; }
        public MaterialButton BtnReject { get; set; }

        public ReassignAdapterViewHolder(View itemView, Action<ReassignAdapterClickEventArgs> clickListener,
                            Action<ReassignAdapterClickEventArgs> longClickListener, Action<ReassignAdapterClickEventArgs> reassignClic, Action<ReassignAdapterClickEventArgs> rejectClick) : base(itemView)
        {
            //TextView = v;
            TxtReason = itemView.FindViewById<TextView>(Resource.Id.txtReason);
            TxtDriver = itemView.FindViewById<TextView>(Resource.Id.txtDriverName);
            TxtLocation = itemView.FindViewById<TextView>(Resource.Id.txtLocation);
            BtnReject = itemView.FindViewById<MaterialButton>(Resource.Id.BtnReject);
            BtnReassign = itemView.FindViewById<MaterialButton>(Resource.Id.BtnReassign);
            BtnReject.Click += (sender, e) => rejectClick(new ReassignAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            BtnReassign.Click += (sender, e) => reassignClic(new ReassignAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.Click += (sender, e) => clickListener(new ReassignAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new ReassignAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class ReassignAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}
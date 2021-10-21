using admin.Models;
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Button;
using System;
using System.Collections.Generic;

namespace admin.Adapters
{
    class AnnouncementAdapter : RecyclerView.Adapter
    {
        public event EventHandler<AnnouncementAdapterClickEventArgs> ItemClick;
        public event EventHandler<AnnouncementAdapterClickEventArgs> ItemDeleteClick;
        public event EventHandler<AnnouncementAdapterClickEventArgs> ItemLongClick;
        public List<AnnouncementModel> items = new List<AnnouncementModel>();

        public AnnouncementAdapter(List<AnnouncementModel> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.announcement_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new AnnouncementAdapterViewHolder(itemView, OnClick, OnLongClick, OnDeleteClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {

            // Replace the contents of the view with that element
            var holder = viewHolder as AnnouncementAdapterViewHolder;
            holder.AnouncementMsg.Text = items[position].Message;
            holder.DatesPosted.Text = $"{items[position].TimeStamp.ToDateTime():dddd, dd MMMM yyyy, HH:mm tt}";

        }

        public override int ItemCount => items.Count;

        void OnDeleteClick(AnnouncementAdapterClickEventArgs args) => ItemDeleteClick?.Invoke(this, args);
        void OnClick(AnnouncementAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(AnnouncementAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class AnnouncementAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView AnouncementMsg { get; set; }
        public TextView DatesPosted { get; set; }

        public MaterialButton BtnDelete { get; set; }

        public AnnouncementAdapterViewHolder(View itemView, Action<AnnouncementAdapterClickEventArgs> clickListener,
                            Action<AnnouncementAdapterClickEventArgs> longClickListener, Action<AnnouncementAdapterClickEventArgs> deleteClickListener) : base(itemView)
        {
            //TextView = v;
            DatesPosted = itemView.FindViewById<TextView>(Resource.Id.txtDateAnnounced);
            AnouncementMsg = itemView.FindViewById<TextView>(Resource.Id.txtAnnouncementMsg);
            BtnDelete = itemView.FindViewById<MaterialButton>(Resource.Id.BtnDeleteAnouncement);

            BtnDelete.Click += (sender, e) => deleteClickListener(new AnnouncementAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.Click += (sender, e) => clickListener(new AnnouncementAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new AnnouncementAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class AnnouncementAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}
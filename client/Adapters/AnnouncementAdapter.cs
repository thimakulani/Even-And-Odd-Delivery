using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using client.Classes;
using System;
using System.Collections.Generic;

namespace client.Adapters
{
    class AnnouncementAdapter : RecyclerView.Adapter
    {
        public event EventHandler<AnnouncementAdapterClickEventArgs> ItemClick;
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

            var vh = new AnnouncementAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {

            // Replace the contents of the view with that element
            var holder = viewHolder as AnnouncementAdapterViewHolder;
            holder.AnouncementMsg.Text = items[position].Message;
            holder.DatesPosted.Text = items[position].TimeStamp.ToDateTime().ToString("dddd, dd MMMM yyyy, HH: mm tt");

            //if (items[position].TimeStamp.ToString("dd/MMM/yyyy") == DateTime.Now.ToString("dd/MMM/yyyy"))
            //{


            //}
            //else
            //{
            //    holder.DatesPosted.Text = items[position].TimeStamp.ToString("ddd, dd/MMM/yyyy HH:mm tt");
            //}
        }

        public override int ItemCount => items.Count;

        void OnClick(AnnouncementAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(AnnouncementAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class AnnouncementAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView AnouncementMsg { get; set; }
        public TextView DatesPosted { get; set; }


        public AnnouncementAdapterViewHolder(View itemView, Action<AnnouncementAdapterClickEventArgs> clickListener,
                            Action<AnnouncementAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            DatesPosted = itemView.FindViewById<TextView>(Resource.Id.txtDateAnnounced);
            AnouncementMsg = itemView.FindViewById<TextView>(Resource.Id.txtAnnouncementMsg);

            itemView.Click += (sender, e) => clickListener(new AnnouncementAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new AnnouncementAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
        }
    }

    public class AnnouncementAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}
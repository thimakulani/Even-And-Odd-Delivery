using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using driver.Models;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace driver.Adapters
{
    class HistoryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<HistoryAdapterClickEventArgs> ItemClick;
        public event EventHandler<HistoryAdapterClickEventArgs> ItemLongClick;
        private readonly List<DeliveryModal> items = new List<DeliveryModal>();

        public HistoryAdapter(List<DeliveryModal> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.history_row, parent, false);

            var vh = new HistoryAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {

            // Replace the contents of the view with that element
            var holder = viewHolder as HistoryAdapterViewHolder;
            int indexPos =  position;
            //holder.TextView.Text = items[position];
            //holder.TxtClientName.Text = items[indexPos].Name;
            holder.TxtDestination.Text = items[indexPos].DestinationAddress;
            holder.TxtDistance.Text = items[indexPos].Distance;
            holder.TxtPickUploaction.Text = items[indexPos].PickupAddress;
            holder.TxtPrice.Text = items[indexPos].Price;
            holder.TxtStatus.Text = items[indexPos].Status;
            holder.TxtHistoryRequestDatesTimeCreated.Text = $"{items[indexPos].TimeStamp.ToDateTime():dddd, dd MMMM yyyy, HH:mm tt}";
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(items[indexPos].UserId)
                .AddSnapshotListener((snapshot, value) =>
                {
                    if (snapshot.Exists)
                    {
                        var user = snapshot.ToObject<ClientModel>();
                        holder.TxtClientName.Text = $"{user.Name} {user.Surname}";
                    }
                });

        }

        public override int ItemCount => items.Count;

        void OnClick(HistoryAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(HistoryAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class HistoryAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public TextView TxtClientName { get; set; }
        public TextView TxtDestination { get; set; }
        public TextView TxtPickUploaction { get; set; }
        public TextView TxtStatus { get; set; }
        public TextView TxtDistance { get; set; }
        public TextView TxtPrice { get; set; }
        public TextView TxtHistoryRequestDatesTimeCreated { get; set; }


        public HistoryAdapterViewHolder(View itemView, Action<HistoryAdapterClickEventArgs> clickListener,
                            Action<HistoryAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            TxtClientName = itemView.FindViewById<TextView>(Resource.Id.HistoryClientName);
            TxtDestination = itemView.FindViewById<TextView>(Resource.Id.HistoryDestination);
            TxtPickUploaction = itemView.FindViewById<TextView>(Resource.Id.HistoryPickupLocation);
            TxtStatus = itemView.FindViewById<TextView>(Resource.Id.HistoryRequestStatus);
            TxtDistance = itemView.FindViewById<TextView>(Resource.Id.HistoryDistance);
            TxtPrice = itemView.FindViewById<TextView>(Resource.Id.HistoryPrice);
            TxtHistoryRequestDatesTimeCreated = itemView.FindViewById<TextView>(Resource.Id.HistoryRequestDatesTimeCreated);

            itemView.Click += (sender, e) => clickListener(new HistoryAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new HistoryAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class HistoryAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}
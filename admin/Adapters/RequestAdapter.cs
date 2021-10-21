using admin.Models;
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace admin.Adapters
{
    class RequestAdapter : RecyclerView.Adapter
    {
        public event EventHandler<RequestAdapterClickEventArgs> ItemClick;
        public event EventHandler<RequestAdapterClickEventArgs> ItemLongClick;
        private readonly List<DeliveryModal> items = new List<DeliveryModal>();
        public List<string> Driver_Name = new List<string>();
        public RequestAdapter(ref List<DeliveryModal> data)
        {
            items = data;
            Driver_Name.Clear();
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;

            itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.history_row, parent, false);

            //var id = Resource.Layout.__YOUR_ITEM_HERE;
            //itemView = LayoutInflater.From(parent.Context).
            //       Inflate(id, parent, false);

            var vh = new RequestAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {

            // Replace the contents of the view with that element
            var holder = viewHolder as RequestAdapterViewHolder;


            holder.PickupLocation.Text = items[position].PickupAddress;
            holder.Destination.Text = items[position].DestinationAddress;
            holder.Price.Text = items[position].Price;
            holder.Distance.Text = items[position].Distance;

            switch (items[position].Status)
            {
                case "W":
                    holder.HistoryRequestStatus.Text = "Waiting for driver";
                    break;
                case "A":
                    holder.HistoryRequestStatus.Text = "Accepted";
                    break;
                case "P":
                    holder.HistoryRequestStatus.Text = "Picked up";
                    break;
                case "D":
                    holder.HistoryRequestStatus.Text = "Delivered";
                    break;
            }
            
            if (items[position].DriverId != null)
            {
                CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("USERS")
                    .Document(items[position].DriverId)
                    .AddSnapshotListener((value, errors) =>
                    {
                        if (value.Exists)
                        {
                            var user = value.ToObject<AppUsers>();
                            holder.DriverName.Text = $"{user.Name}  {user.Surname}";
                            
                        }
                    });

            }
            else
            {
                this.Driver_Name.Add(null);
            }
            holder.HistoryDatesTimeCreated.Text = $"{items[position].TimeStamp.ToDateTime():dddd, dd MMMM yyyy, HH:mm tt}";

        }


        public override int ItemCount => items.Count;

        void OnClick(RequestAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(RequestAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

  
    }

    public class RequestAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView DriverName { get; set; }
        public TextView PickupLocation { get; set; }
        public TextView Destination { get; set; }
        public TextView Distance { get; set; }
        public TextView Price { get; set; }
        public TextView HistoryRequestStatus { get; set; }
        //       public Button BtnCancelRequest { get; set; }
        public TextView HistoryDatesTimeCreated { get; set; }

        public RequestAdapterViewHolder(View itemView, Action<RequestAdapterClickEventArgs> clickListener,
                            Action<RequestAdapterClickEventArgs> longClickListener) : base(itemView)
        {

            DriverName = itemView.FindViewById<TextView>(Resource.Id.HistoryDriverName);
            PickupLocation = itemView.FindViewById<TextView>(Resource.Id.HistoryPickupLocation);
            Destination = itemView.FindViewById<TextView>(Resource.Id.HistoryDestination);
            HistoryDatesTimeCreated = itemView.FindViewById<TextView>(Resource.Id.HistoryDatesTimeCreated);
            Distance = itemView.FindViewById<TextView>(Resource.Id.HistoryDistance);
            Price = itemView.FindViewById<TextView>(Resource.Id.HistoryPrice);
            HistoryRequestStatus = itemView.FindViewById<TextView>(Resource.Id.HistoryRequestStatus);

            itemView.Click += (sender, e) => clickListener(new RequestAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new RequestAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }


    }

    public class RequestAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public List<string> Driver_Name { get; set; }
    }
}
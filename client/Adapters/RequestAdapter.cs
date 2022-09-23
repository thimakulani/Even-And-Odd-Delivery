using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using client.Classes;
using Google.Android.Material.Button;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace client.Adapters
{
    class RequestAdapter : RecyclerView.Adapter
    {
        public event EventHandler<RequestAdapterClickEventArgs> ItemClick;
        public event EventHandler<RequestAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<RequestAdapterClickEventArgs> BtnCancelClick;
        public event EventHandler<RequestAdapterClickEventArgs> BtnViewDriverClick;
        private readonly List<DeliveryRequestModel> items = new List<DeliveryRequestModel>();

        public RequestAdapter(List<DeliveryRequestModel> data)
        {
            items = data;
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

            var vh = new RequestAdapterViewHolder(itemView, OnClick, OnLongClick, OnCancelClick, OnViewDriverClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {

            // Replace the contents of the view with that element
            var holder = viewHolder as RequestAdapterViewHolder;
            int indexPos = position;
            holder.PickupLocation.Text = items[indexPos].PickupAddress;
            holder.Destination.Text = items[indexPos].DestinationAddress;
            holder.Price.Text = items[indexPos].Price;
            holder.Distance.Text = items[indexPos].Distance;
            

            holder.HistoryDatesTimeCreated.Text = $"{items[indexPos].RequestTime}";
            if (!string.IsNullOrWhiteSpace(items[indexPos].DriverId))
            {
                holder.BtnCancelRequest.Visibility = ViewStates.Gone;
                // holder.DriverName.Text = items[indexPos].DriverName;
                holder.BtnViewDriver.Visibility = ViewStates.Visible;



            }
            switch (items[indexPos].Status)
            {
                case "W":
                    holder.HistoryRequestStatus.Text = "Waiting for driver";
                    //holder.DriverName.Text = "==== No driver ====";
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
                case "C":
                    holder.BtnCancelRequest.Visibility = ViewStates.Gone;
                    //holder.DriverName.Text = "==== No driver ====";
                    holder.HistoryRequestStatus.Text = "Canceled";
                    break;
            }
            //holder.TextView.Text = items[position];
        }


        public override int ItemCount => items.Count;

        void OnClick(RequestAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(RequestAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void OnCancelClick(RequestAdapterClickEventArgs args) => BtnCancelClick?.Invoke(this, args);
        void OnViewDriverClick(RequestAdapterClickEventArgs args) => BtnViewDriverClick?.Invoke(this, args);


    }

    public class RequestAdapterViewHolder : RecyclerView.ViewHolder
    {
        //public TextView DriverName { get; set; }
        public TextView PickupLocation { get; set; }
        public TextView Destination { get; set; }
        public TextView Distance { get; set; }
        public TextView Price { get; set; }
        public TextView HistoryRequestStatus { get; set; }
        public MaterialButton BtnCancelRequest { get; set; }
        public MaterialButton BtnViewDriver { get; set; }
        public TextView HistoryDatesTimeCreated { get; set; }

        public RequestAdapterViewHolder(View itemView, Action<RequestAdapterClickEventArgs> clickListener,
                            Action<RequestAdapterClickEventArgs> longClickListener, Action<RequestAdapterClickEventArgs> CancelClickListener,
                            Action<RequestAdapterClickEventArgs> viewDriverClickListener) : base(itemView)
        {

            //DriverName = itemView.FindViewById<TextView>(Resource.Id.HistoryDriverName);
            PickupLocation = itemView.FindViewById<TextView>(Resource.Id.HistoryPickupLocation);
            Destination = itemView.FindViewById<TextView>(Resource.Id.HistoryDestination);
            HistoryDatesTimeCreated = itemView.FindViewById<TextView>(Resource.Id.HistoryDatesTimeCreated);
            Distance = itemView.FindViewById<TextView>(Resource.Id.HistoryDistance);
            Price = itemView.FindViewById<TextView>(Resource.Id.HistoryPrice);
            HistoryRequestStatus = itemView.FindViewById<TextView>(Resource.Id.HistoryRequestStatus);
            BtnCancelRequest = itemView.FindViewById<MaterialButton>(Resource.Id.BtnCancelRequest);
            BtnViewDriver = itemView.FindViewById<MaterialButton>(Resource.Id.BtnViewDriver);
            //TextView = v;



            itemView.Click += (sender, e) => clickListener(new RequestAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new RequestAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            BtnCancelRequest.Click += (sender, e) => CancelClickListener(new RequestAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            BtnViewDriver.Click += (sender, e) => viewDriverClickListener(new RequestAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
        }


    }

    public class RequestAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}
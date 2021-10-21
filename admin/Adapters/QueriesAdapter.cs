using admin.Models;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace admin.Adapters
{
    class QueriesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<QuerySendersAdapterClickEventArgs> ItemClick;
        public event EventHandler<QuerySendersAdapterClickEventArgs> ItemLongClick;
        private readonly List<QueriesModel> items = new List<QueriesModel>();

        public QueriesAdapter(List<QueriesModel> data)
        {
            items = data;

        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.query_view_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);
            var vh = new QuerySendersAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {

            // Replace the contents of the view with that element
            var holder = viewHolder as QuerySendersAdapterViewHolder;

            holder.TimeSent.Text = $"{items[position].TimeStamp.ToDateTime():dddd, dd MMMM yyyy, HH:mm tt}";


            CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(items[position].Uid)
                .AddSnapshotListener((value, error) =>
                {
                    if (value.Exists)
                    {
                        var user = value.ToObject<AppUsers>();
                        holder.SenderName.Text = $"{user.Name} {user.Surname}";
                    }
                });

        }

        public override int ItemCount => items.Count;

        void OnClick(QuerySendersAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(QuerySendersAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);


    }

    public class QuerySendersAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView SenderName { get; set; }
        public TextView TimeSent { get; set; }


        public QuerySendersAdapterViewHolder(View itemView, Action<QuerySendersAdapterClickEventArgs> clickListener,
                            Action<QuerySendersAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            SenderName = itemView.FindViewById<TextView>(Resource.Id.QueryRowName);
            TimeSent = itemView.FindViewById<TextView>(Resource.Id.QueryRowDates);

            itemView.Click += (sender, e) => clickListener(new QuerySendersAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            itemView.LongClick += (sender, e) =>
            {
                longClickListener(new QuerySendersAdapterClickEventArgs { View = itemView, Position = BindingAdapterPosition });
            };
        }
    }

    public class QuerySendersAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}
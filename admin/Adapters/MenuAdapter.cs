using admin.Models;
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace admin.Adapters
{
    class MenuAdapter : RecyclerView.Adapter
    {
        public event EventHandler<MenuAdapterClickEventArgs> ItemClick;
        public event EventHandler<MenuAdapterClickEventArgs> ItemLongClick;
        List<Menu_Items> items = new List<Menu_Items>();

        public MenuAdapter(List<Menu_Items> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.menu_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new MenuAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {

            // Replace the contents of the view with that element
            var holder = viewHolder as MenuAdapterViewHolder;
            holder.txt_title.Text = items[position].Title;
            holder.img_icon.SetImageResource(items[position].Icon);
        }

        public override int ItemCount => items.Count;

        void OnClick(MenuAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(MenuAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class MenuAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView txt_title { get; set; }
        public ImageView img_icon { get; set; }


        public MenuAdapterViewHolder(View itemView, Action<MenuAdapterClickEventArgs> clickListener,
                            Action<MenuAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TextView = v;
            txt_title = itemView.FindViewById<TextView>(Resource.Id.txtTitle);
            img_icon = itemView.FindViewById<ImageView>(Resource.Id.imgIcon);
            itemView.Click += (sender, e) => clickListener(new MenuAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new MenuAdapterClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class MenuAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}
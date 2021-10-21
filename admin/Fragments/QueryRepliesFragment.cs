using admin.Models;
using Android.Content;
using Android.OS;
using AndroidX.RecyclerView.Widget;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using Com.Github.Library.Bubbleview;
using FirebaseAdmin.Messaging;
using Google.Android.Material.AppBar;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;

namespace admin.Fragments
{
    public class QueryRepliesFragment : DialogFragment
    {
        private RecyclerView recyler;
        private TextInputEditText InputMessage;
        private FloatingActionButton FabSend;
        private readonly List<Replies> Items = new List<Replies>();
        private readonly string queryId;

        private MaterialToolbar toolbar_reply_queries;

        public QueryRepliesFragment(string queryId)
        {
            this.queryId = queryId;
            //this.senderName = senderName;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment her968e6
            SetStyle(StyleNoFrame, Resource.Style.FullScreenDialogStyle);
        }
        private Context context;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.activity_query_replay, container, false);
            context = view.Context;
            ConnectView(view);
            return view;
        }

        private void ConnectView(View view)
        {
            recyler = view.FindViewById<RecyclerView>(Resource.Id.TRecyclerChatMessage);
            FabSend = view.FindViewById<FloatingActionButton>(Resource.Id.TBtnSendMessage);
            InputMessage = view.FindViewById<TextInputEditText>(Resource.Id.TInputMessage);
            InputMessage.SetTextColor(Android.Graphics.Color.Black);
            toolbar_reply_queries = view.FindViewById<MaterialToolbar>(Resource.Id.toolbar_reply_queries);

            toolbar_reply_queries.SetNavigationIcon(Resource.Mipmap.ic_arrow_back_black_18dp);
            FabSend.Click += FabSend_Click;
            toolbar_reply_queries.NavigationClick += Toolbar_reply_queries_NavigationClick1;

            ChatAdapter adapter = new ChatAdapter(Items);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(context);
            recyler.SetLayoutManager(linearLayoutManager);
            recyler.SetAdapter(adapter);

            CrossCloudFirestore.Current
                .Instance
                .Collection("USERS")
                .Document(queryId)
                .AddSnapshotListener((value, error) =>
                {
                    if (value.Exists)
                    {
                        var user = value.ToObject<AppUsers>();
                        toolbar_reply_queries.Title = $"{user.Name} {user.Surname}";
                    }
                });

            CrossCloudFirestore.Current
                .Instance
                .Collection("QUERIES")
                .Document(queryId)
                .Collection("Messages")
                .OrderBy("TimeStamp")
                .AddSnapshotListener((value, error) =>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var dc in value.DocumentChanges)
                        {
                            var reply = new Replies();

                            switch (dc.Type)
                            {

                                case DocumentChangeType.Added:
                                    reply = dc.Document.ToObject<Replies>();
                                    reply.Id = dc.Document.Id;
                                    Items.Add(reply);
                                    adapter.NotifyItemInserted(dc.NewIndex);
                                    break;
                                case DocumentChangeType.Modified:
                                    reply = dc.Document.ToObject<Replies>();
                                    reply.Id = dc.Document.Id;
                                    Items[dc.OldIndex] = reply;
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Removed:
                                    break;
                            }
                        }
                    }
                });




        }

        private void Toolbar_reply_queries_NavigationClick1(object sender, AndroidX.AppCompat.Widget.Toolbar.NavigationClickEventArgs e)
        {
            Dismiss();
        }



        private async void FabSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputMessage.Text))
            {
                InputMessage.Error = "Type in a message";
                InputMessage.RequestFocus();
                return;
            }
            Dictionary<string, object> chat = new Dictionary<string, object>
            {
                { "Uid", "A" },
                { "Message", InputMessage.Text },
                { "TimeStamp", FieldValue.ServerTimestamp }
            };

            await CrossCloudFirestore
                .Current
                .Instance
                .Collection("QUERIES")
                .Document(queryId)
                .Collection("Messages")
                .AddAsync(chat);

            Dictionary<string, object> dates = new Dictionary<string, object>
            {
                { "TimeStamp", FieldValue.ServerTimestamp }
            };
            await CrossCloudFirestore
                .Current
                .Instance
                .Collection("QUERIES")
                .Document(queryId)
                .SetAsync(dates);
            InputMessage.Text = string.Empty;

            var stream = Resources.Assets.Open("service_account.json");
            var fcm = new FirebaseHelper.FirebaseData().GetFirebaseMessaging(stream);
            FirebaseAdmin.Messaging.Message message = new FirebaseAdmin.Messaging.Message()
            {
                Topic = queryId,
                Notification = new Notification()
                {
                    Title = "Reply",
                    Body = $"{InputMessage.Text}",

                },
            };
            await fcm.SendAsync(message);
        }




    }
    public class ChatAdapter : RecyclerView.Adapter
    {
        readonly List<Replies> items = new List<Replies>();
        public ChatAdapter(List<Replies> data)
        {
            items = data;
        }

        public override int ItemCount
        {
            get
            {
                return items.Count;
            }
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is ChatsView)
            {
                ChatsView chatsView = holder as ChatsView;
                chatsView.TxtMessage.Text = items[position].Message;
                //chatsView.TxtName.Text = items[position].SenderName;
            }
            else
            {
                SenderChats senderView = holder as SenderChats;
                senderView.SenderTxtMessage.Text = items[position].Message;
            }


        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == Resource.Layout.message_sent_row)
            {
                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.message_sent_row, parent, false);
                BubbleTextView TxtMessage = row.FindViewById<BubbleTextView>(Resource.Id.sender_row);
                //TextView TxtName = row.FindViewById<TextView>(Resource.Id.TxtMsgSenderName);
                ChatsView view = new ChatsView(row)
                {
                    TxtMessage = TxtMessage,
                    //TxtName = TxtName

                };
                return view;
            }
            else
            {
                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.message_reply_row, parent, false);
                BubbleTextView TxtMessage = row.FindViewById<BubbleTextView>(Resource.Id.reply_row);
                SenderChats view = new SenderChats(row)
                {
                    SenderTxtMessage = TxtMessage,

                };
                return view;
            }

        }
        public override int GetItemViewType(int position)
        {
            if (items[position].Uid == "A")
            {
                return Resource.Layout.message_reply_row;
            }
            else
            {
                return Resource.Layout.message_sent_row;
            }

        }
        public class ChatsView : RecyclerView.ViewHolder
        {
            public View Myview { get; set; }
            public TextView TxtMessage { get; set; }
            public TextView TxtName { get; set; }

            public ChatsView(View itemView) : base(itemView)
            {
                Myview = itemView;
            }
        }
        public class SenderChats : RecyclerView.ViewHolder
        {
            public View Myview { get; set; }
            public TextView SenderTxtMessage { get; set; }
            public SenderChats(View itemView) : base(itemView)
            {
                Myview = itemView;
            }
        }
    }
}
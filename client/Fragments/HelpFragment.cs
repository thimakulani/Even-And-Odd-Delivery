using AndroidX.Fragment.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using client.Classes;
using Com.Github.Library.Bubbleview;
using Firebase.Auth;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.TextField;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using AndroidX.RecyclerView.Widget;
using Firebase.Messaging;
using FirebaseAdmin.Messaging;

namespace client.Fragments
{
    public class HelpFragment : Fragment
    {
        private TextInputEditText InputMessage;
        private FloatingActionButton BtnSend;
        private RecyclerView recyclerView;
        private readonly List<Queries> items = new List<Queries>();

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            var view = inflater.Inflate(Resource.Layout.activity_help_queries, container, false);
            ConnectViews(view);
            
            return view;
        }

        private void ConnectViews(View view)
        {
            InputMessage = view.FindViewById<TextInputEditText>(Resource.Id.TInputMessage);

            BtnSend = view.FindViewById<FloatingActionButton>(Resource.Id.TBtnSendMessage);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.TRecyclerChatMessage);
            BtnSend.Click += BtnSend_Click;
            InputMessage.SetTextColor(Android.Graphics.Color.Black);

            LinearLayoutManager linear = new LinearLayoutManager(view.Context);
            QueriesAdapter adapter = new QueriesAdapter(items);
            recyclerView.SetLayoutManager(linear);
            recyclerView.SetAdapter(adapter);
            CrossCloudFirestore
                .Current
                .Instance
                .Collection("QUERIES")
                .Document(FirebaseAuth.Instance.Uid)
                .Collection("Messages")
                .OrderBy("TimeStamp")
                .AddSnapshotListener((value, error)=>
                {
                    if (!value.IsEmpty)
                    {
                        foreach (var item in value.DocumentChanges)
                        {
                            switch (item.Type)
                            {
                                case DocumentChangeType.Added:
                                    items.Add(item.Document.ToObject<Queries>());
                                    adapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Modified:
                                    break;
                                case DocumentChangeType.Removed:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                });

        }



        private async void BtnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(InputMessage.Text))
            {
                InputMessage.Error = "Type in a message";
                return;
            }

            Dictionary<string, object> dates = new Dictionary<string, object>
            {
                { "TimeStamp", FieldValue.ServerTimestamp }
            };

            Dictionary<string, object> chat = new Dictionary<string, object>
            {
                { "Uid", FirebaseAuth.Instance.Uid },
                { "Message", InputMessage.Text },
                { "TimeStamp", FieldValue.ServerTimestamp }
            };

            await CrossCloudFirestore
                .Current
                .Instance
                .Collection("QUERIES")
                .Document(FirebaseAuth.Instance.Uid)
                .Collection("Messages")
                .AddAsync(chat);
            await CrossCloudFirestore
                .Current
                .Instance
                .Collection("QUERIES")
                .Document(FirebaseAuth.Instance.Uid)
                .SetAsync(dates);
            InputMessage.Text = string.Empty;

            var stream = Resources.Assets.Open("service_account.json");
            var fcm = FirebaseHelper.FirebaseAdminSDK.GetFirebaseMessaging(stream);
            FirebaseAdmin.Messaging.Message message = new FirebaseAdmin.Messaging.Message()
            {
                Topic = "QUERIES",
                Notification = new Notification()
                {
                    Title = "New Query",
                    Body = $"{InputMessage.Text}",

                },
            };
            await fcm.SendAsync(message);


            //InputMessage.Text = string.Empty;



            //Dictionary<string, string> data = new Dictionary<string, string>();
            //string sender_id = GetString(Resource.String.sender_id);
            //var msg = new RemoteMessage
            //    .Builder(sender_id + "@fcm.googleapis.com")
            //    .AddData("Message", "New Message")
            //    .AddData("Topic", "QUERIES")
            //    .Build();
                
            
            //FirebaseMessaging.Instance
            //    .Send(msg);
            
            //query.Collection("TimeStamp")
            //    .Document("TimeStamp")
            //    .SetAsync(dates); 

        }

    }
    class QueriesAdapter : RecyclerView.Adapter
    {
        readonly List<Queries> items = new List<Queries>();
        public QueriesAdapter(List<Queries> data)
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
                chatsView.TxtTimeDate.Text = $"{items[position].TimeStamp.ToDateTime():dddd, dd MMMM yyyy, HH:mm tt}";
                chatsView.TxtMessage.Text = items[position].Message;
                //chatsView.TxtName.Text = items[position].SenderName;
            }
            else
            {
                SenderChats senderView = holder as SenderChats;
                senderView.SenderTxtMessage.Text = items[position].Message;
                senderView.SenderTxtTimeDate.Text = $"{items[position].TimeStamp.ToDateTime():dddd, dd MMMM yyyy, HH:mm tt}";
            }

        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == Resource.Layout.chat_message_sender_row)
            {
                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.chat_message_reciever_row, parent, false);
                BubbleTextView TxtMessage = row.FindViewById<BubbleTextView>(Resource.Id.TxtMessage);
                TextView TxtDt = row.FindViewById<TextView>(Resource.Id.TxtMsgTime);
                //TextView TxtName = row.FindViewById<TextView>(Resource.Id.TxtMsgSenderName);
                ChatsView view = new ChatsView(row)
                {
                    TxtMessage = TxtMessage,
                    TxtTimeDate = TxtDt,
                    //TxtName = TxtName

                };
                return view;
            }
            else
            {
                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.chat_message_sender_row, parent, false);
                BubbleTextView TxtMessage = row.FindViewById<BubbleTextView>(Resource.Id.SenderTxtMessage);
                TextView TxtDt = row.FindViewById<TextView>(Resource.Id.SenderTxtMsgTime);
                SenderChats view = new SenderChats(row)
                {
                    SenderTxtMessage = TxtMessage,
                    SenderTxtTimeDate = TxtDt,

                };
                return view;
            }

        }
        public override int GetItemViewType(int position)
        {
            if (items[position].Uid == FirebaseAuth.Instance.Uid)
            {
                return Resource.Layout.chat_message_reciever_row;
            }
            else
            {
                return Resource.Layout.chat_message_sender_row;
            }

        }
        public class ChatsView : RecyclerView.ViewHolder
        {
            public View Myview { get; set; }
            public BubbleTextView TxtMessage { get; set; }
            public TextView TxtName { get; set; }
            public TextView TxtTimeDate { get; set; }

            public ChatsView(View itemView) : base(itemView)
            {
                Myview = itemView;
            }
        }
        public class SenderChats : RecyclerView.ViewHolder
        {
            public View Myview { get; set; }
            public BubbleTextView SenderTxtMessage { get; set; }
            public TextView SenderTxtTimeDate { get; set; }
            public SenderChats(View itemView) : base(itemView)
            {
                Myview = itemView;
            }
        }
    }
}
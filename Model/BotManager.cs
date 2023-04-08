using System.Threading;
using System.Windows;
using Radio_Leech.ViewModel;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace Radio_Leech.Model
{
    public class BotManager
    {

        static BotManager manager;

        static public BotManager GetInstance() => manager;

        static public void Init(string botToken, string userName, LeechVM vm, string channel)
        {
            manager = new BotManager(botToken, userName, vm, channel);
            manager.Listen();
        }


        readonly TwitchClient client = new();
        private readonly string UserName;
        private readonly string BotToken;
        private readonly string ChannelName;
        private readonly LeechVM VM;
        public BotManager(string botToken, string userName, LeechVM vm, string channel)
        {
            VM = vm;
            UserName = userName;
            BotToken = botToken;
            //ChannelName = "TuberTugger"; // Used for testing purposes. 
            ChannelName = channel;

            ConnectionCredentials credentials = new(UserName, BotToken);
            client.Initialize(credentials, ChannelName);
            client.Connect();
            client.JoinChannel(ChannelName);
        }
        public void Listen() => client.OnMessageReceived += MessageRecieved;
        private void MessageRecieved(object? sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.Length > 3 && e.ChatMessage.Message.ToLower()[..4] == "!sr ")
                HandleSongRequest(e.ChatMessage.Message[4..]);
        }

        private void HandleSongRequest(string request) => VM.SongRequest(request);

        public void SendChatMessage(string targetString)
        {
            client.JoinChannel(ChannelName);
            client.SendMessage(ChannelName, targetString);
            
        }





    }
}

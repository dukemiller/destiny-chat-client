using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using destiny_chat_client.Enums;
using destiny_chat_client.Models;
using destiny_chat_client.Models.Socket;
using destiny_chat_client.Repositories.Interfaces;
using destiny_chat_client.Services.Interfaces;
using Newtonsoft.Json;
using WebSocketSharp;

namespace destiny_chat_client.Services
{
    public class ChatService : IChatService
    {
        private const string MeUrl = "https://www.destiny.gg/api/chat/me";

        private readonly ISettingsRepository _settingsRepository;

        private readonly ICookieFinderService _cookieFinderService;

        private WebSocket _ws;

        // 

        public ChatService(ISettingsRepository settingsRepository, ICookieFinderService cookieFinderService)
        {
            _settingsRepository = settingsRepository;
            _cookieFinderService = cookieFinderService;
            _ws = new WebSocket("ws://www.destiny.gg/ws");

            if (_settingsRepository.LoggedIn)
            {
                _ws.SetCookie(new WebSocketSharp.Net.Cookie("sid", _settingsRepository.Sid));
                _ws.SetCookie(new WebSocketSharp.Net.Cookie("rememberme", _settingsRepository.RememberMe));
            }
        }

        // 

        public Action<Data> MessageReceived { get; set; }

        public Action<ServerError> ErrorReceived { get; set; }

        public ObservableCollection<UserData> Users { get; set; } = new ObservableCollection<UserData>();

        public ObservableCollection<UserData> OrderedUsers => new ObservableCollection<UserData>(Users
            .OrderByDescending(user => user.Features.Contains(Feature.Admin))
            .ThenBy(user => user.Username)
        );

        // 

        public void StartReceivingMessages()
        {
            if (MessageReceived == null || ErrorReceived == null)
                throw new Exception("MessageReceived or ErrorReceived are unset.");
            _ws.OnOpen += OnOpen;
            _ws.OnError += OnError;
            _ws.OnMessage += OnMessage;
            _ws.Connect();
        }

        public void StopReceivingMessages()
        {
            _ws.OnOpen -= OnOpen;
            _ws.OnError -= OnError;
            _ws.OnMessage -= OnMessage;
            _ws.Close();
            _ws = new WebSocket("ws://www.destiny.gg/ws");
        }

        public void SendMessage(string text) => _ws.SendAsync(
            "MSG " + JsonConvert.SerializeObject(new SendMessage(text)), _ => { });

        public async Task<(bool successful, string username)> GetUsername()
        {
            var response = "";

            var address = new Uri(MeUrl);
            var cookies = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = cookies,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            var client = new HttpClient(handler) { BaseAddress = address };

            if (_settingsRepository.Sid.Length > 0)
                cookies.Add(address, new Cookie("sid", _settingsRepository.Sid));

            if (_settingsRepository.RememberMe.Length > 0)
                cookies.Add(address, new Cookie("rememberme", _settingsRepository.RememberMe));

            client.DefaultRequestHeaders.Add("Host", "www.destiny.gg");
            client.DefaultRequestHeaders.Add("User-Agent", "destiny.gg windows client");
            client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.5");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            client.DefaultRequestHeaders.Add("DNT", "1");
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            
            // connection error
            try
            {
                var request = await client.GetAsync("");

                // Apparently you don't need your SID to login if you already created a rememberme token
                if (_settingsRepository.Sid.Length == 0)
                    _settingsRepository.Sid = request.Headers
                        .First(k => k.Key.Equals("Set-Cookie"))
                        .Value
                        .First(v => v.Contains("sid="))
                        .Split('=')[1]
                        .Split(';')[0];

                response = await request.Content.ReadAsStringAsync();
            }

            catch
            {
                return (false, response);
            }

            // wrong information error
            if (!response.Contains("{\"nick\":"))
                return (false, response);

            // deserialization error
            try
            {
                return (true, JsonConvert.DeserializeObject<dynamic>(response).nick);
            }

            catch
            {
                return (false, "");
            }
        }

        public async Task FindDetails()
        {
            (var sid, var rememberme) = await _cookieFinderService.FindDetails();
            // TODO: pop up an error here
            // if (sid.Length == 0 || rememberme.Length == 0) ;
            //  else
            {
                _settingsRepository.Sid = sid;
                _settingsRepository.RememberMe = rememberme;
                await _settingsRepository.Save();
            }
        }

        public void Login(string sid, string rememberme)
        {
            StopReceivingMessages();
            _ws.SetCookie(new WebSocketSharp.Net.Cookie("sid", sid));
            _ws.SetCookie(new WebSocketSharp.Net.Cookie("rememberme", rememberme));
            StartReceivingMessages();
        }

        // 

        private static void OnOpen(object sender, EventArgs args) => Console.WriteLine(args);

        private static void OnError(object sender, ErrorEventArgs args) => Console.WriteLine(args);

        private void OnMessage(object sender, MessageEventArgs dataMessage)
        {
            if (dataMessage.IsText)
            {
                var data = new Data(dataMessage);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (data.IsMessage)
                        MessageReceived(data);
                    else if (data.IsNames)
                        Users = new ObservableCollection<UserData>(data.Names.Users);
                    else if (data.IsJoin)
                        Users.Add(data.UserData);
                    else if (data.IsQuit)
                        Users.Remove(Users.FirstOrDefault(user => user.Username.Equals(data.UserData.Username)));
                    else if (data.IsError)
                        ErrorReceived(data.ChatError);
                });
            }
        }
    }
}
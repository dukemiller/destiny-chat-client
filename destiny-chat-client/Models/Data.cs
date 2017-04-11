using destiny_chat_client.Enums;
using destiny_chat_client.Models.Socket;
using Newtonsoft.Json;
using WebSocketSharp;

namespace destiny_chat_client.Models
{
    /// <summary>
    ///     A wrapper model around any generic response from the websocket server
    /// </summary>
    public class Data
    {
        private readonly string _content;

        private Message _message;

        private UserData _userData;

        public Data(string content) => _content = content;

        public Data(MessageEventArgs dataMessage) : this(dataMessage.Data) { }

        public Data(Message message)
        {
            _content = "MSG";
            _message = message;
        }

        public bool IsNames => _content.StartsWith("NAMES");

        public bool IsMessage => _content.StartsWith("MSG");

        public bool IsQuit => _content.StartsWith("QUIT");

        public bool IsJoin => _content.StartsWith("JOIN");

        public bool IsError => _content.StartsWith("ERR");

        public OnConnect Names => IsNames
            ? JsonConvert.DeserializeObject<OnConnect>(_content.Substring(6))
            : null;

        public Message Message
        {
            get
            {
                if (_message == null && IsMessage)
                    _message = JsonConvert.DeserializeObject<Message>(_content.Substring(4));
                return _message;
            }
            set => _message = value;
        }

        public UserData UserData
        {
            get
            {
                if (_userData == null && (IsQuit || IsJoin))
                    _userData = JsonConvert.DeserializeObject<UserData>(_content.Substring(4));
                return _userData;
            }
            set => _userData = value;
        }

        public ServerError ChatError
        {
            get
            {
                if (!IsError)
                    return ServerError.None;

                switch (_content.Substring(4).Replace("\"", ""))
                {
                    case "needlogin":
                        return ServerError.NeedLogin;
                    default:
                        return ServerError.None;
                }
            }
        }
    }
}
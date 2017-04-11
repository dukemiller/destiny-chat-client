using Newtonsoft.Json;

namespace destiny_chat_client.Models.Socket
{
    /// <summary>
    ///     The model for sending a message to the websocket server.
    /// </summary>
    public class SendMessage
    {
        [JsonProperty("data")]
        public string Data { get; set; }

        public SendMessage() { }

        public SendMessage(string data) => Data = data;
    }
}
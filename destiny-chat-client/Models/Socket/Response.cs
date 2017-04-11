using Newtonsoft.Json;

namespace destiny_chat_client.Models.Socket
{
    /// <summary>
    ///     The response model from the websocket server.
    /// </summary>
    public class Response
    {
        public string Action { get; set; }
        public Message Message { get; set; }
        public string ToJson() => $"{Action} {JsonConvert.SerializeObject(Message)}";
    }
}
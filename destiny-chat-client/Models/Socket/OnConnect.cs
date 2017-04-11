using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace destiny_chat_client.Models.Socket
{
    /// <summary>
    ///     The object that gets sent on successful connection from the websocket server.
    /// </summary>
    [Serializable]
    public class OnConnect
    {
        [JsonProperty("connectioncount")]
        public int ConnectionCount { get; set; }

        [JsonProperty("users")]
        public List<UserData> Users { get; set; }
    }
}
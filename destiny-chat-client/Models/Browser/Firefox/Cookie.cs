using System;
using Newtonsoft.Json;

namespace destiny_chat_client.Models.Browser.Firefox
{
    [Serializable]
    public class Cookie
    {
        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("originAttributes")]
        public OriginAttributes OriginAttributes { get; set; }
    }
}
using System.Collections.Generic;
using System.Linq;
using destiny_chat_client.Classes;
using destiny_chat_client.Enums;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace destiny_chat_client.Models
{
    /// <summary>
    ///     The JSON response of a user chat message
    /// </summary>
    public class UserData : ObservableObject
    {
        [JsonProperty("nick")]
        public string Username { get; set; }

        [JsonProperty("features")]
        public List<string> FeaturesAsStrings { get; set; } = new List<string>();

        [JsonIgnore]
        public List<Feature> Features => Extensions.GetValues<Feature>()
            .Where(emote => FeaturesAsStrings.Contains(emote.Description()))
            .ToList();

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
    }
}
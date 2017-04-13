using System;
using Newtonsoft.Json;

namespace destiny_chat_client.Models.Browser.Firefox
{
    [Serializable]
    public class OriginAttributes
    {
        [JsonProperty("addonId")]
        public string AddonId { get; set; }

        [JsonProperty("appId")]
        public int AppId { get; set; }

        [JsonProperty("firstPartyDomain")]
        public string FirstPartyDomain { get; set; }

        [JsonProperty("inIsolatedMozBrowser")]
        public bool InIsolatedMozBrowser { get; set; }

        [JsonProperty("privateBrowsingId")]
        public int PrivateBrowsingId { get; set; }

        [JsonProperty("userContextId")]
        public int UserContextId { get; set; }
    }
}
using Newtonsoft.Json;

namespace destiny_chat_client.Models
{
    /// <summary>
    ///     A qualified parsed destiny.gg message from a user
    /// </summary>
    public class Message : UserData
    {
        private bool _dimmed;

        private bool _mention;

        private bool _isUser;

        // 

        /// <summary>
        ///     Stating if the message belongs to the user of the client.
        /// </summary>
        [JsonIgnore]
        public bool IsUser
        {
            get => _isUser;
            set => Set(() => IsUser, ref _isUser, value);
        }

        /// <summary>
        ///     The text content of the message.
        /// </summary>
        [JsonProperty("data")]
        public string Text { get; set; }

        /// <summary>
        ///     Stating if the message is dimmed by request of user, 
        ///     everything but what the user selected will be slightly faded out
        /// </summary>
        [JsonIgnore]
        public bool Dimmed
        {
            get => _dimmed;
            set => Set(() => Dimmed, ref _dimmed, value);
        }

        /// <summary>
        ///     Stating if user of the client was mentioned in the text content
        ///     of this post, or if a key word the user has written was mentioned.
        /// </summary>
        [JsonIgnore]
        public bool Mention
        {
            get => _mention;
            set => Set(() => Mention, ref _mention, value);
        }
    }
}
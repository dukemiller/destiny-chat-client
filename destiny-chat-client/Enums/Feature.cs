using System.ComponentModel;

namespace destiny_chat_client.Enums
{
    /// <summary>
    ///     All features and their associated flairs available to a user
    /// </summary>
    public enum Feature
    {
        [Description("")]
        None,

        [Description("protected")]
        Protected,

        [Description("subscriber")]
        Subscriber,

        [Description("flair9")]
        SubscriberT0,

        [Description("flair13")]
        SubscriberT1,

        [Description("flair1")]
        SubscriberT2,

        [Description("flair3")]
        SubscriberT3,

        [Description("flair8")]
        SubscriberT4,

        [Description("vip")]
        Vip,

        [Description("moderator")]
        Moderator,

        [Description("admin")]
        Admin,

        [Description("flair12")]
        Broadcaster,

        [Description("bot")]
        Bot,

        [Description("flair11")]
        Bot2,

        [Description("flair2")]
        Notable,

        [Description("flair4")]
        Trusted,

        [Description("flair5")]
        Contributor,

        [Description("flair6")]
        CompChallenge,

        [Description("flair7")]
        Eve,

        [Description("flair10")]
        Sc2
    }
}
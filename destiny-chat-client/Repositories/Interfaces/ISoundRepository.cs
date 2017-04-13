namespace destiny_chat_client.Repositories.Interfaces
{
    /// <summary>
    ///     An interface to accessing and playing sounds.
    /// </summary>
    public interface ISoundRepository
    {
        /// <summary>
        ///     Play the sound that would signify a notification.
        /// </summary>
        void PlayNotificationSound();
    }
}
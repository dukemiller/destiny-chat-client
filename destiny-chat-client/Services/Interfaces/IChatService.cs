using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using destiny_chat_client.Enums;
using destiny_chat_client.Models;

namespace destiny_chat_client.Services.Interfaces
{
    /// <summary>
    ///     The main chat service. Handles receiving and sending all messages, as well as login
    ///     and authentication.
    /// </summary>
    public interface IChatService
    {
        /// <summary>
        ///     The callback for when a message is received
        /// </summary>
        Action<Data> MessageReceived { get; set; }

        /// <summary>
        ///     The callback for when an error is received
        /// </summary>
        Action<ServerError> ErrorReceived { get; set; }

        /// <summary>
        ///     Essentially the start of the message read loop with a callback
        ///     to what to do when a message is received or when an error is received
        /// </summary>
        void StartReceivingMessages();

        /// <summary>
        ///     Stop receiving messages, for the purpose of re-starting the connection
        ///     with new credentials.
        /// </summary>
        void StopReceivingMessages();

        /// <summary>
        ///     Send a message to the chat
        /// </summary>
        void SendMessage(string text);

        /// <summary>
        ///     Do a login.
        /// </summary>
        void Login(string sid, string rememberme);

        /// <summary>
        ///     Send a request to the 'init' endpoint at destiny.gg to retrieve the username
        ///     of the associated 'sid' and/or 'rememberme' tokens
        /// </summary>
        /// <returns></returns>
        Task<(bool successful, string username)> GetUsername();

        /// <summary>
        ///     Find 'sid' and 'rememberme' tokens from cookies stored somewhere on the hard drive 
        ///     and save it to settings.
        /// </summary>
        Task FindDetails();

        /// <summary>
        ///     The list of users, updated by join/leave messages from the server.
        /// </summary>
        ObservableCollection<UserData> Users { get; set; }

        /// <summary>
        ///     The ordered list of users for use in the user popup.
        /// </summary>
        ObservableCollection<UserData> OrderedUsers { get; }
    }
}
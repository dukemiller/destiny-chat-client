using System.Collections.Generic;

namespace destiny_chat_client.Enums
{
    /// <summary>
    ///     The websocket server error states.
    /// </summary>
    public enum ServerError
    {
        None,
        unknown,
        nopermission,
        protocolerror,
        needlogin,
        invalidmsg,
        throttled,
        duplicate,
        muted,
        submode,
        needbanreason,
        banned,
        requiresocket,
        toomanyconnections,
        socketerror,
        privmsgbanned,
        privmsgaccounttooyoung,
        notfound
    }

    internal static class ServerErrorExtension
    {
        private static readonly Dictionary<ServerError, string> _reason;

        static ServerErrorExtension()
        {
            _reason = new Dictionary<ServerError, string>
            {
                {ServerError.unknown, "Unknown error, this usually indicates an internal problem :("},
                {ServerError.nopermission, "You do not have the required permissions to use that"},
                {ServerError.protocolerror, "Invalid or badly formatted"},
                {ServerError.needlogin, "You have to be logged in to use that"},
                {ServerError.invalidmsg, "The message was invalid"},
                {ServerError.throttled, "Throttled! You were trying to send messages too fast"},
                {ServerError.duplicate, "The message is identical to the last one you sent"},
                {ServerError.muted, "You are muted (subscribing auto-removes mutes)"},
                {ServerError.submode, "The channel is currently in subscriber only mode"},
                {ServerError.needbanreason, "Providing a reason for the ban is mandatory"},
                {ServerError.banned, "You have been banned (subscribing auto-removes non-permanent bans), disconnecting"},
                {ServerError.requiresocket, "This chat requires WebSockets"},
                {ServerError.toomanyconnections, "Only 5 concurrent connections allowed"},
                {ServerError.socketerror, "Error contacting server"},
                {ServerError.privmsgbanned, "Cannot send private messages while banned"},
                {ServerError.privmsgaccounttooyoung, "Your account is too recent to send private messages"},
                {ServerError.notfound, "The user was not found"},
            };
        }

        public static string Reason(this ServerError error) => _reason[error];
    }
}
using System.ComponentModel;

namespace destiny_chat_client.Enums
{
    /// <summary>
    ///     The websocket server error states.
    /// </summary>
    public enum ServerError
    {
        None,

        [Description("Needs login")]
        NeedLogin
    }
}
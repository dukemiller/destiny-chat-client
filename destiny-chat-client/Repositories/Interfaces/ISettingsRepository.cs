using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace destiny_chat_client.Repositories.Interfaces
{
    /// <summary>
    ///     The user saved settings.
    /// </summary>
    public interface ISettingsRepository
    {
        /// <summary>
        ///     Exit to tray instead of closing the main window.
        /// </summary>
        bool ExitToTray { get; set; }

        /// <summary>
        ///     Minimize to tray instead of minimizing to the startbar.
        /// </summary>
        bool MinimizeToTray { get; set; }

        /// <summary>
        ///     Always keep the tray open.
        /// </summary>
        bool TrayAlwaysOpen { get; set; }

        /// <summary>
        ///     The user preferenced words to be highlighted
        /// </summary>
        string HighlightWords { get; set; }

        IEnumerable<string> HighlightWordsList { get; }

        /// <summary>
        ///     The 'sid' key used for login authentication.
        /// </summary>
        string Sid { get; set; }

        /// <summary>
        ///     The 'rememberme' key used for login authentication.
        /// </summary>
        string RememberMe { get; set; }

        /// <summary>
        ///     Show flair icons of every message in the chat.
        /// </summary>
        bool ShowFlair { get; set; }

        /// <summary>
        ///     Show timestamps of every message in the chat.
        /// </summary>
        bool ShowTimestamp { get; set; }

        /// <summary>
        ///     Flash the application on mention of the user of the client.
        /// </summary>
        bool FlashOnMention { get; set; }

        /// <summary>
        ///     Play a sound on mention of the user of the client.
        /// </summary>
        bool SoundOnMention { get; set; }

        /// <summary>
        ///     Toast to the desktop on mention of the user of the client.
        /// </summary>
        bool ToastOnMention { get; set; }

        /// <summary>
        ///     The username of the user of the client.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        ///     Save to disk.
        /// </summary>
        Task Save();

        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Checking if SID, RememberMe and Username fields are filled in.
        /// </summary>
        bool LoggedIn { get; }
    }
}
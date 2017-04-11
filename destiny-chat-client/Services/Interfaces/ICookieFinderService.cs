using System.Threading.Tasks;

namespace destiny_chat_client.Services.Interfaces
{
    /// <summary>
    ///     The service that probes at user internals to get confidential information.
    /// </summary>
    public interface ICookieFinderService
    {
        /// <summary>
        ///     Find the system's default browser for the purpose finding the cookie details.
        /// </summary>
        string GetSystemDefaultBrowser();

        /// <summary>
        ///     Get the sid and rememberme found in the cookie storage of the system's browser.
        /// </summary>
        Task<(string sid, string rememberme)> FindDetails();
    }
}
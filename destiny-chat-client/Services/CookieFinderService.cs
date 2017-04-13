using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using destiny_chat_client.Models.Browser.Firefox;
using destiny_chat_client.Services.Interfaces;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace destiny_chat_client.Services
{
    public class CookieFinderService : ICookieFinderService
    {
        private static readonly string FireFoxProfilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Mozilla", "Firefox", "Profiles");

        private static string FirefoxProfile => Directory.GetDirectories(FireFoxProfilePath)
            .OrderBy(dir => new FileInfo(dir).LastAccessTimeUtc)
            .FirstOrDefault();

        private static readonly string ChromeProfile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Google", "Chrome", "User Data", "Default");

        // 

        public string GetSystemDefaultBrowser()
        {
            var name = string.Empty;
            RegistryKey regKey = null;

            try
            {
                regKey = Registry.ClassesRoot.OpenSubKey("HTTP\\shell\\open\\command", false);
                if (regKey != null)
                    name = regKey.GetValue(null).ToString().ToLower().Replace("" + (char)34, "");
                if (!name.EndsWith("exe"))
                    name = name.Substring(0, name.LastIndexOf(".exe", StringComparison.Ordinal) + 4);
            }

            catch (Exception)
            {
                // ignored
            }

            finally
            {
                regKey?.Close();
            }

            return name;
        }

        public async Task<(string sid, string rememberme)> FindDetails()
        {
            var sid = "";
            var rememberme = "";

            var browser = GetSystemDefaultBrowser();

            if (browser.Contains("firefox"))
            {
                var profile = await FirefoxProfileRecovery();
                var cookies = await FirefoxCookiesSqlite();

                if (profile.sid.Length > 0)
                    sid = profile.sid;

                else if (cookies.sid.Length > 0)
                    sid = cookies.sid;

                if (profile.rememberme.Length > 0)
                    rememberme = profile.rememberme;

                else if (cookies.rememberme.Length > 0)
                    rememberme = cookies.rememberme;
            }

            else if (browser.Contains("chrome"))
            {
                var profile = await ChromeCookiesSqlite();

                if (profile.sid.Length > 0)
                    sid = profile.sid;

                if (profile.rememberme.Length > 0)
                    rememberme = profile.rememberme;
            }

            return (sid, rememberme);
        }

        // 

        private static async Task<(string sid, string rememberme)> ChromeCookiesSqlite()
        {
            // Details
            var sid = "";
            var rememberme = "";
            var path = Path.Combine(ChromeProfile, "Cookies");

            // DB connection
            using (var db = new SQLiteConnection($"Data Source={path};pooling=false"))
            {
                db.Open();
                var command =
                    new SQLiteCommand("SELECT creation_utc, host_key, name, encrypted_value " +
                                      "FROM cookies " +
                                      "WHERE host_key LIKE '%destiny.gg%' AND name = 'sid' OR name = 'rememberme';",
                        db);
                var reader = await command.ExecuteReaderAsync();

                // Parsing
                while (await reader.ReadAsync())
                {
                    var line = ChromeCookie(reader);
                    if (line.name.ToLower().Equals("rememberme"))
                        rememberme = line.value;
                    else if (line.name.ToLower().Equals("sid"))
                        sid = line.value;
                }
            }
            return (sid, rememberme);
        }

        private static async Task<(string sid, string rememberme)> FirefoxCookiesSqlite()
        {
            // Details
            var sid = "";
            var rememberme = "";
            var path = Path.Combine(FirefoxProfile, "cookies.sqlite");

            // DB connection
            using (var db = new SQLiteConnection($"Data Source={path}"))
            {
                db.Open();
                var command =
                    new SQLiteCommand("SELECT id, host, name, value " +
                                      "FROM moz_cookies " +
                                      "WHERE host LIKE '%destiny.gg%' AND name = 'sid' OR name = 'rememberme';", db);
                var reader = await command.ExecuteReaderAsync();

                // Parsing
                while (await reader.ReadAsync())
                {
                    var line = FirefoxCookie(reader);
                    if (line.name.ToLower().Equals("rememberme"))
                        rememberme = line.value;
                    else if (line.name.ToLower().Equals("sid"))
                        sid = line.value;
                }
            }

            return (sid, rememberme);
        }

        private static async Task<(string sid, string rememberme)> FirefoxProfileRecovery()
        {
            // Details
            var sid = "";
            var rememberme = "";
            var path = Path.Combine(FirefoxProfile, "sessionstore-backups", "recovery.js");
            const string startStr = "\"cookies\":[";
            const string endStr = "],\"selectedWindow\"";

            using (var streamreader = new StreamReader(path))
            {
                // Getting relevant lines
                var lines = await streamreader.ReadToEndAsync();
                var start = lines.IndexOf(startStr, StringComparison.Ordinal) + startStr.Length - 1;
                var end = lines.Substring(start).IndexOf(endStr, StringComparison.Ordinal);
                var json = lines.Substring(start, end - 1);

                // Conversion
                var firefoxCookies = JsonConvert.DeserializeObject<List<Cookie>>(json);

                // Parsing
                foreach (var cookie in firefoxCookies)
                    if (cookie.Name.ToLower().Equals("rememberme"))
                        rememberme = cookie.Value;
                    else if (cookie.Name.ToLower().Equals("sid"))
                        sid = cookie.Value;
            }

            return (sid, rememberme);
        }

        // 

        // http://stackoverflow.com/questions/22532870/encrypted-cookies-in-chrome
        private static (string name, string value) ChromeCookie(IDataRecord reader)
        {
            var name = Convert.ToString(reader["name"]);
            var encrypted = (byte[])reader["encrypted_value"];
            var decoded = System.Security.Cryptography.ProtectedData.Unprotect(encrypted, null,
                System.Security.Cryptography.DataProtectionScope.CurrentUser);
            var value = Encoding.ASCII.GetString(decoded);
            return (name, value);
        }

        private static (string host, string name, string value) FirefoxCookie(IDataRecord reader)
        {
            var name = Convert.ToString(reader["name"]);
            var host = Convert.ToString(reader["host"]);
            var value = Convert.ToString(reader["value"]);
            return (host, name, value);
        }
    }
}
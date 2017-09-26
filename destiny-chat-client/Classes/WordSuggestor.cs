using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using destiny_chat_client.Repositories;
using destiny_chat_client.Repositories.Interfaces;
using destiny_chat_client.Services;
using destiny_chat_client.Services.Interfaces;
using destiny_chat_client.ViewModels;
using GalaSoft.MvvmLight.Ioc;

namespace destiny_chat_client.Classes
{
    public static class WordSuggestor
    {
        public static bool Suggesting;

        private static readonly ChatViewModel ChatViewModel = SimpleIoc.Default.GetInstance<ChatViewModel>();

        private static readonly IChatService ChatService = SimpleIoc.Default.GetInstance<IChatService>();

        private static readonly IEmoteRepository EmoteRepository = SimpleIoc.Default.GetInstance<IEmoteRepository>();

        private static List<string> _options = new List<string>();

        private static int _optionsPosition;

        private static async Task FindSuggestions(string word)
        {
            var emotes = await Task.Run(() => EmoteRepository.Emotes
                .Where(e => e.ToString().ToLower().StartsWith(word.ToLower()))
                .Select(e => e.ToString()));

            var names = await Task.Run(() => ChatViewModel.ActiveUsers
                .Where(user => user.ToLower().StartsWith(word.ToLower()))
                .ToList());

            // BAD
            if (!names.Any())
                names = await Task.Run(() => ChatService.Users
                    .Where(user => user.Username.ToLower().StartsWith(word.ToLower()))
                    .Select(user => user.Username)
                    .ToList());

            _options = await Task.Run(() => names.Concat(emotes)
                .OrderBy(thing => Distance.LevenshteinDistance(thing.ToLower(), word.ToLower()))
                .ToList());
        }

        public static async Task<string> CorrectMessage(string message)
        {
            // I need some more weighting for active users
            if (message?.Trim().Length > 0)
            {
                var sentence = message.TrimEnd().Split(' ').ToList();
                var word = sentence.LastOrDefault();

                if (Suggesting)
                {
                    _optionsPosition = _optionsPosition + 1 >= _options.Count 
                        ? 0 
                        : _optionsPosition + 1;
                }

                else
                {
                    _optionsPosition = 0;
                    _options = new List<string>();
                    Suggesting = true;
                    await FindSuggestions(word);
                }

                sentence.RemoveAt(sentence.Count - 1);

                if (sentence.Count >= 1)
                    return string.Join(" ", sentence) + " " + _options[_optionsPosition];

                return _options[_optionsPosition] + " ";
            }

            return message;
        }
    }
}
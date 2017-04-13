using System;
using System.Collections.Generic;
using System.Linq;
using destiny_chat_client.Classes;
using destiny_chat_client.Enums;
using destiny_chat_client.Repositories.Interfaces;

namespace destiny_chat_client.Repositories
{
    public class EmoteRepository: IEmoteRepository
    {
        public EmoteRepository() => Emotes = Extensions.GetValues<Emote>().ToList();

        public List<Emote> Emotes { get; }

        public bool HasEmote(string name) => Emotes.Any(emoticon => emoticon.ToString().Equals(name));

        public Emote GetEmote(string name) => Emotes.FirstOrDefault(emoticon => emoticon.ToString().Equals(name.ToString()));

        public Uri GetSourceUri(Emote emote) => new Uri($"pack://application:,,,/Resources/Emoticons/{emote}.png");
    }
}
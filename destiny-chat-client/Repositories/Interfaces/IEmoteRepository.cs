using System;
using System.Collections.Generic;
using destiny_chat_client.Enums;

namespace destiny_chat_client.Repositories.Interfaces
{
    /// <summary>
    ///     An abstracted way to get emotes from memory.
    /// </summary>
    public interface IEmoteRepository
    {
        List<Emote> Emotes { get; }

        Uri GetSourceUri(Emote emote);

        bool HasEmote(string name);

        Emote GetEmote(string name);
    }
}
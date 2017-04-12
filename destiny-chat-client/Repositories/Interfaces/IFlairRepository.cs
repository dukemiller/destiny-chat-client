using System;
using System.Collections.Generic;
using destiny_chat_client.Enums;

namespace destiny_chat_client.Repositories.Interfaces
{
    /// <summary>
    ///     An abstracted way to get flair icons from memory.
    /// </summary>
    public interface IFlairRepository
    {
        List<Flair> Flairs { get; }

        Uri GetSourceUri(Flair flair);

        bool TryGetFlair(Feature feature, out Flair flair);
    }
}
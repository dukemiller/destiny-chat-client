using System;
using System.Collections.Generic;
using System.Linq;
using destiny_chat_client.Classes;
using destiny_chat_client.Enums;
using destiny_chat_client.Repositories.Interfaces;

namespace destiny_chat_client.Repositories
{
    public class FlairRepository: IFlairRepository
    {
        public FlairRepository() => Flairs = Extensions.GetValues<Flair>().ToList();

        public bool TryGetFlair(Feature feature, out Flair flair)
        {
            if (feature == Feature.SubscriberT0)
                flair = Flair.minitwitch;
            else
                flair = Flairs.FirstOrDefault(icon => icon.ToString().ToLower().Equals(feature.ToString().ToLower()));
            return flair != Flair.NONE;
        }

        public Uri GetSourceUri(Flair flair) => new Uri($"pack://application:,,,/Resources/Icons/{flair}.png");

        public List<Flair> Flairs { get; }
    }
}
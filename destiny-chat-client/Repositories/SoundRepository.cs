using System.Media;
using System.Reflection;
using destiny_chat_client.Repositories.Interfaces;

namespace destiny_chat_client.Repositories
{
    public class SoundRepository : ISoundRepository
    {
        private readonly Assembly _assembly;

        public SoundRepository() => _assembly = Assembly.GetExecutingAssembly();

        public void PlayNotificationSound()
        {
            using (var online = _assembly.GetManifestResourceStream("destiny_chat_client.Resources.Sfx.notification.wav"))
            using (var player = new SoundPlayer(online))
                player.Play();
        }
    }
}
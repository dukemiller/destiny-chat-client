using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using destiny_chat_client.Enums;
using destiny_chat_client.Models;
using destiny_chat_client.Repositories.Interfaces;
using destiny_chat_client.Services.Interfaces;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

namespace destiny_chat_client.Services
{
    public class MockChatService : IChatService
    {
        private readonly ISettingsRepository _settingsRepository = SimpleIoc.Default.GetInstance<ISettingsRepository>();

        public static DebugScenario DebugScenario = DebugScenario.Silence;

        public MockChatService()
        {
            // Debug popup binding inject
            Application.Current.MainWindow.InputBindings.Add(new KeyBinding
            {
                Command = new RelayCommand(() => new Views.Dialogs.Debug().ShowDialog()),
                Gesture = new KeyGesture(Key.D, ModifierKeys.Control)
            });
        }

        private async void MessageLoop()
        {
            Users.Add(new UserData { Username = _settingsRepository.Username });
            Users.Add(new UserData { Username = "Destiny", Features = { Feature.Admin, Feature.Broadcaster } });
            Users.Add(new UserData { Username = "The_memer" });
            Users.Add(new UserData { Username = "CaptainAutismo" });
            Users.Add(new UserData { Username = "I_have_osteoperosis" });

            MessageReceived(new Data(new Message
            {
                Username = "",
                Text = "Ctrl-D to select a test scenario."
            }));


            var random = new Random();
            var number = 1;

            string RandomMessage()
            {
                const string alphabet = "ABCDEFGHJIKLMNOPQRSTUVWXYZabcdefghijklmnopqrstvuwxyz";
                var message = "";
                for (var _ = 0; _ < random.Next(20, 80); _++)
                    message += alphabet[random.Next(alphabet.Length - 1)];
                for (var _ = 1; _ < 6; _++)
                    message = message.Insert((message.Length / 5) * _, " ");
                return message;
            }

            while (true)
            {
                switch (DebugScenario)
                {
                    case DebugScenario.Emotes:
                    {
                        MessageReceived(new Data(new Message
                        {
                            Username = "Destiny",
                            Text = "die kids",
                            Features = { Feature.Admin, Feature.Broadcaster }
                        }));

                        foreach (var _ in Enumerable.Range(0, random.Next(32, 132)))
                        {
                            MessageReceived(new Data(new Message
                            {
                                Username = "Destiny",
                                Text = "nathanRustle",
                                FeaturesAsStrings = { "admin", "flair8", "flair12", }
                            }));

                            await Task.Delay(random.Next(10, 355));
                        }
                        foreach (var _ in Enumerable.Range(0, 2))
                        {
                            MessageReceived(new Data(
                                new Message
                                {
                                    Username = "Desto",
                                    Text = "SOTRIGGERED",
                                    FeaturesAsStrings = { "flair2", "flair7" }
                                }));
                            await Task.Delay(random.Next(400, 900));

                            MessageReceived(new Data(new Message { Username = "Besto", Text = "SOTRIGGERED" }));
                            await Task.Delay(random.Next(400, 500));
                        }
                        break;
                    }

                    case DebugScenario.LongMessages:
                    {
                        await Task.Delay(random.Next(1000, 2102));

                        MessageReceived(new Data(new Message
                        {
                            Username = "sunstar-",
                            Text = "buddy, did you know what you just did? you committed a strawman argument."
                        }));

                        await Task.Delay(random.Next(2000, 2102));

                        MessageReceived(new Data(new Message
                        {
                            Username = "sunstar-",
                            Text =
                                "Not sure what that is? heh, didn\'t expect so. basically its when you disagree with my opinion. What\'s that? You just cited a highly-qualified economist to back up your point? Buddy, that\'s an appeal to authority. Obviously any layman can just open up a 200-page paper of economic analysis and understand it in all its complexity, and even comment on it."
                        }));

                        await Task.Delay(random.Next(2000, 5102));

                        MessageReceived(new Data(new Message
                        {
                            Username = "sunstar-",
                            Text =
                                "In the future try to be more rational and smart like me. You can check out my youtube channel if u want, I\'m part of the \"skeptic community\" you see, so you can tell I have the best opinions."
                        }));

                        await Task.Delay(random.Next(2000, 5102));

                        MessageReceived(new Data(new Message
                        {
                            Username = "harveychocolatemilk",
                            Text = "/me laughs nervously"
                        }));

                        await Task.Delay(random.Next(1000, 5102));

                        MessageReceived(new Data(new Message
                        {
                            Username = "harveychocolatemilk",
                            Text =
                                "uh, you're not allowed to interrupt my 9 minute rant to ask me to clarify something. You're gaslighting me right now and I won't stand for this gotcha journalism. When I practice debating alone in my bedroom nobody ever interrupts or questions me, what gives you the right? SHUT UP SHUT UP SHUT UP!!!"
                        }));

                        break;
                    }

                    case DebugScenario.Mention:
                    {
                        MessageReceived(new Data(new Message
                        {
                            Username = $"Username_{number}",
                            Text = $"Hello {_settingsRepository.Username}"
                        }));

                        await Task.Delay(3000);

                        MessageReceived(new Data(new Message
                        {
                            Username = $"Username_{number}",
                            Text = "A basic message.",
                            Features = { Feature.Admin, Feature.Broadcaster }
                        }));

                        await Task.Delay(3000);

                        MessageReceived(new Data(new Message
                        {
                            Username = $"Username_{number++}",
                            Text = $"Goodbye {_settingsRepository.Username}",
                            Features = { Feature.Admin, Feature.Broadcaster }
                        }));

                        await Task.Delay(3000);

                        MessageReceived(new Data(new Message
                        {
                            Username = $"Username_{number}",
                            Text = $"Hello.",
                            Features = { Feature.Admin, Feature.Broadcaster }
                        }));

                        await Task.Delay(3000);

                        MessageReceived(new Data(new Message
                        {
                            Username = $"Username_{number++}",
                            Text = $"{_settingsRepository.Username} HMMStiny",
                            Features = { Feature.Admin, Feature.Broadcaster }
                        }));

                        await Task.Delay(3000);
                        break;
                    }

                    case DebugScenario.Links:
                    {
                        MessageReceived(new Data(new Message
                        {
                            Username = "Destiny",
                            Text = "https://www.google.com",
                            FeaturesAsStrings = { "admin", "flair8", "flair12", }
                        }));

                        await Task.Delay(1000);

                        MessageReceived(new Data(new Message
                        {
                            Username = "Destiny",
                            Text = "http://www.google.com",
                            FeaturesAsStrings = { "admin", "flair8", "flair12", }
                        }));

                        await Task.Delay(1000);

                        MessageReceived(new Data(new Message
                        {
                            Username = "Destiny",
                            Text = "https://google.com",
                            FeaturesAsStrings = { "admin", "flair8", "flair12", }
                        }));

                        await Task.Delay(1000);

                        MessageReceived(new Data(new Message
                        {
                            Username = "Destiny",
                            Text = "https://www.destiny.gg",
                            FeaturesAsStrings = { "admin", "flair8", "flair12", }
                        }));

                        await Task.Delay(1000);

                        MessageReceived(new Data(new Message
                        {
                            Username = "Destiny",
                            Text = "destiny.com",
                            FeaturesAsStrings = { "admin", "flair8", "flair12", }
                        }));

                        await Task.Delay(1000);

                        MessageReceived(new Data(new Message
                        {
                            Username = "Destiny",
                            Text = "www.destiny.gg",
                            FeaturesAsStrings = { "admin", "flair8", "flair12", }
                        }));

                        await Task.Delay(1000);

                        break;
                    }

                    case DebugScenario.UserMentionOtherUsers:
                    {
                        MessageReceived(new Data(new Message
                        {
                            Username = $"Destiny",
                            Text = "man i'm really good at videogames",
                        }));

                        await Task.Delay(2000);

                        MessageReceived(new Data(new Message
                        {
                            Username = $"The_memer",
                            Text = "Destiny could have played that better.",
                        }));

                        await Task.Delay(2000);

                        MessageReceived(new Data(new Message
                        {
                            Username = $"CaptainAutismo",
                            Text = "The_memer Klappa",
                        }));

                        await Task.Delay(2000);

                        MessageReceived(new Data(new Message
                        {
                            Username = $"I_have_osteoperosis",
                            Text = $"The_memer Destiny HmmStiny",
                        }));

                        await Task.Delay(2000);

                        MessageReceived(new Data(new Message
                        {
                            Username = $"Destiny",
                            Text = $"!ip2048 The_memer",
                        }));

                        await Task.Delay(2000);


                        MessageReceived(new Data(new Message
                        {
                            Username = $"Destiny",
                            Text = $"!ip2048 CaptainAutismo",
                        }));

                        await Task.Delay(2000);

                        MessageReceived(new Data(new Message
                        {
                            Username = $"I_have_osteoperosis",
                            Text = $"DaFeels",
                        }));

                        await Task.Delay(2000);


                        break;
                    }

                    case DebugScenario.Spam:
                    {
                        MessageReceived(new Data(new Message
                        {
                            Username = $"username_{number++}",
                            Text = RandomMessage(),
                        }));

                        await Task.Delay(100);
                        break;
                    }

                    case DebugScenario.SuperSpam:
                    {
                        MessageReceived(new Data(new Message
                        {
                            Username = $"username_{number++}",
                            Text = RandomMessage(),
                        }));

                        await Task.Delay(1);
                        break;
                    }

                    case DebugScenario.GreenText:
                    {
                        MessageReceived(new Data(new Message
                        {
                            Username = $"username_{number++}",
                            Text = ">" + RandomMessage(),
                        }));

                        await Task.Delay(1000);
                        break;
                    }

                    case DebugScenario.Silence:
                    default:
                    {
                        await Task.Delay(10);
                        break;
                    }
                }
            }
        }

        // 

        public Action<Data> MessageReceived { get; set; }

        public Action<ServerError> ErrorReceived { get; set; }

        public void StartReceivingMessages() => MessageLoop();

        public void StopReceivingMessages() {}

        public void SendMessage(string text) => MessageReceived(new Data(new Message { Username = "Me", Text = text }));

        void IChatService.Login(string sid, string rememberme) => throw new NotImplementedException();

        public bool Login(string sid, string rememberme) => true;

        public Task FindDetails() => Task.CompletedTask;

        public async Task<(bool successful, string username)> GetUsername() => await Task.Run(() => (true, "Me"));

        public ObservableCollection<UserData> Users { get; set; } = new ObservableCollection<UserData>();

        public ObservableCollection<UserData> OrderedUsers { get; } = new ObservableCollection<UserData>();
    }
}
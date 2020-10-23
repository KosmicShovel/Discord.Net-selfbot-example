
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Discord;
using Discord.WebSocket;
using Discord_Selfbot_Example.Properties;

namespace Discord_Selfbot_Example
{
    class Program
    {
        public static DiscordSocketClient SelfClient;
        [Obsolete]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new SelfBotContext());
            }
            catch (Exception)
            {
                Application.Exit();
            }
        }

        [Obsolete]
        public class SelfBotContext : ApplicationContext
        {
            private readonly NotifyIcon trayIcon;

            public SelfBotContext()
            {
                Console.WriteLine("Starting Bot!");
                SelfClient = new DiscordSocketClient();
                SelfClient.LoginAsync(TokenType.User, File.ReadAllText("D:\\token.txt").Split(' ')[1]);
                SelfClient.StartAsync();
                SelfClient.Ready += () =>
                {
                    Console.WriteLine("Logged in as " + SelfClient.CurrentUser.Username + "!");
                    return Task.CompletedTask;
                };

                SelfClient.MessageReceived += message =>
                {
                    if (message.Author.Username == SelfClient.CurrentUser.Username)
                    {
                        if (message.Content.StartsWith(".embed"))
                        {
                            string content = message.Content.Replace(".embed", "");
                            message.Channel.SendMessageAsync(null, false, new EmbedBuilder
                            {
                                Color = Color.Gold,
                                Description = content
                            }.Build(), RequestOptions.Default);
                        }
                    }
                    return Task.CompletedTask;
                };

                trayIcon = new NotifyIcon()
                {
                    Text = "Self Bot",
                    Icon = Resources.Icon,
                    ContextMenu = new ContextMenu(new MenuItem[] {
                        new MenuItem("Exit", Exit)
                    }),
                    Visible = true
                };
            }

            async void Exit(object sender, EventArgs e)
            {
                trayIcon.Visible = false;

                await SelfClient.LogoutAsync();
                SelfClient.Dispose();
                Application.Exit();
            }
        }
    }
}

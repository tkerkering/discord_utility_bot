using System.IO;
using System.Threading.Tasks;
using Anotar.Serilog;
using Discord;
using Discord.WebSocket;
using DiscordUtilityBot.Commands;

// TODO: Create config handler that reads/writes all configurations to text files (until there is a db) 

namespace DiscordUtilityBot
{
    public class Program
    {
        private DiscordSocketClient _client;

        public static void Main()
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var token = File.ReadAllText("token.txt");
            LogTo.Information("Creating discord client with token {0}", token);
            _client = new DiscordSocketClient();
            _client.Log += DiscordLogHandle;
            _client.Ready += Client_Ready;

            LogTo.Information("Trying to login..");
            await _client.LoginAsync(TokenType.Bot, token).ConfigureAwait(false);
            await _client.StartAsync().ConfigureAwait(false);

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public async Task Client_Ready()
        {
            await CommandCreator.CreateAllCommands(_client).ConfigureAwait(false);
        }

        private static Task DiscordLogHandle(LogMessage msg)
        {
            LogTo.Information(msg.ToString());
            return Task.CompletedTask;
        }
    }
}

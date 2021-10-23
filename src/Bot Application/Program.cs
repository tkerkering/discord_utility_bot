using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Anotar.Serilog;
using Discord;
using Discord.WebSocket;
using DiscordUtilityBot.Commands.SlashCommands;
using Serilog;

// TODO: Create config handler that reads/writes all configurations to text files (until there is a db) 

namespace DiscordUtilityBot
{
    public class Program
    {
        // List of all child classes of the BaseSlashDiscordCommand
        private readonly IEnumerable<Type> SlashChilds =
            Assembly.GetAssembly(typeof(BaseSlashDiscordCommand))
            .GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseSlashDiscordCommand)));

        // Instance of the discord bot
        private DiscordSocketClient _client;

        public static void Main(string[] args)
            => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

            var token = File.ReadAllText(args[0]);
            LogTo.Information("Creating discord client with token {0}", token);
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true
            });
            _client.Log += DiscordLogHandle;
            _client.Ready += Client_Ready;
            _client.InteractionCreated += Client_InteractionCreated;

            LogTo.Information("Trying to login..");
            await _client.LoginAsync(TokenType.Bot, token).ConfigureAwait(false);
            await _client.StartAsync().ConfigureAwait(false);

            // Block this task until the program is closed.
            await Task.Delay(-1);
        }

        public async Task Client_Ready()
        {
            await CreateSlashCommands().ConfigureAwait(false);
        }

        private async Task CreateSlashCommands()
        {
            foreach (var type in SlashChilds)
            {
                var commandConstructor = type.GetConstructor(Type.EmptyTypes);
                var instance = commandConstructor.Invoke(Array.Empty<object>());

                var createMethod = type.GetMethod("Create");
                var task = (Task)createMethod.Invoke(instance, new object[] { _client });
                await task.ConfigureAwait(false);
            }
        }

        private async Task Client_InteractionCreated(SocketInteraction arg)
        {
            if (arg is SocketSlashCommand command)
            {
                await HandleSlashCommand(command).ConfigureAwait(false);
            }
        }

        private async Task HandleSlashCommand(SocketSlashCommand command)
        {
            var commandType = SlashChilds.Where(t => (string)t.GetField("Name").GetValue(null) == command.Data.Name).FirstOrDefault();

            var magicConstructor = commandType.GetConstructor(Type.EmptyTypes);
            var instance = magicConstructor.Invoke(Array.Empty<object>());

            var magicMethod = commandType.GetMethod("Respond");
            var result = (Task)magicMethod.Invoke(instance, new object[] { command, _client });
            await result.ConfigureAwait(false);
        }

        private static Task DiscordLogHandle(LogMessage msg)
        {
            LogTo.Information(msg.ToString());
            return Task.CompletedTask;
        }
    }
}

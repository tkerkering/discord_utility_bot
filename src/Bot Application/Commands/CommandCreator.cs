using System;
using System.IO;
using System.Threading.Tasks;
using Anotar.Serilog;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace DiscordUtilityBot.Commands
{
    public static class CommandCreator
    {
        public static async Task CreateAllCommands(DiscordSocketClient client)
        {
            foreach (var line in File.ReadLines("CommandDefinition.csv"))
            {
                var seperatedValues = line.Split(';');
                var serverId = Convert.ToUInt64(seperatedValues[0]);
                _ = serverId == 0
                    ? await AddGlobalCommand(seperatedValues[1], seperatedValues[2], client).ConfigureAwait(false)
                    : await AddGuildCommand(serverId, seperatedValues[1], seperatedValues[2], client).ConfigureAwait(false);
            }
        }

        private static async Task<bool> AddGuildCommand(ulong guildId, string name, string description, DiscordSocketClient client)
        {
            var guild = client.GetGuild(guildId);
            var guildCommand = new SlashCommandBuilder();

            // Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
            guildCommand.WithName(name);
            // Descriptions can have a max length of 100.
            guildCommand.WithDescription(description);

            var success = false;
            try
            {
                // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
                await guild.CreateApplicationCommandAsync(guildCommand.Build()).ConfigureAwait(false);

                success = true;
            }
            catch (ApplicationCommandException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException.
                // This exception contains the path of the error as well as the error message.
                // We can serialize the Error field in the exception to get a visual of where the error is.
                var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                LogTo.Error("Error while trying to add guild command for guild {guild} with the name {name}", guildId, name, json);
            }

            return success;
        }

        private static async Task<bool> AddGlobalCommand(string name, string description, DiscordSocketClient client)
        {
            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName(name);
            globalCommand.WithDescription(description);

            var success = false;
            try
            {
                await client.CreateGlobalApplicationCommandAsync(globalCommand.Build()).ConfigureAwait(false);
                success = true;
            }
            catch (ApplicationCommandException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);
                LogTo.Error("Error while trying to add global command with the name {name}", name, json);
            }

            return success;
        }
    }
}

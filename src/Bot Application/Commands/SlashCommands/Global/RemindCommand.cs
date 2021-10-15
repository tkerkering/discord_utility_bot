using System.Threading.Tasks;
using Anotar.Serilog;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordUtilityBot.Constants;
using Newtonsoft.Json;

namespace DiscordUtilityBot.Commands.SlashCommands.Global
{
    public class RemindCommand : BaseSlashDiscordCommand
    {
        public static readonly new string Name = "remind";

        public override async Task Create(DiscordSocketClient client)
        {
            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName(Name);
            globalCommand.WithDescription(Strings.RemindCommandDescription);

            try
            {
                await client.CreateGlobalApplicationCommandAsync(globalCommand.Build()).ConfigureAwait(false);
            }
            catch (ApplicationCommandException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);
                LogTo.Error("Error while trying to add remind command! {0}", json);
            }
        }

        public override async Task Respond(SocketSlashCommand command, DiscordSocketClient client)
        {
            await command.RespondAsync($"Not implemented yet but you used {command.Data.Name}");
        }
    }
}

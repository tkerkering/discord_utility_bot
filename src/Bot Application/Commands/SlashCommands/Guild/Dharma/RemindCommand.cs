using Anotar.Serilog;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordUtilityBot.Constants;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DiscordUtilityBot.Commands.SlashCommands.Guild.Dharma
{
#if DEBUG
    public class RemindCommand : BaseSlashDiscordCommand
    {
        public static readonly new string Name = "remind";

        public override async Task Create(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder();
            guildCommand.WithName(Name);
            guildCommand.WithDescription(Strings.RemindCommandDescription);

            try
            {
                await client.Rest.CreateGuildCommand(guildCommand.Build(), DharmaConstants.GuildId).ConfigureAwait(false);
            }
            catch (HttpException exception)
            {
                LogTo.Error("Error while trying to add remind command! {0}", exception.Message);
            }
        }

        public override async Task Respond(SocketSlashCommand command, DiscordSocketClient client)
        {
            await command.RespondAsync($"Not implemented yet but you used {command.Data.Name}").ConfigureAwait(false);
        }
    }
#endif
}

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
    public class NamedInvitesCommand : BaseSlashDiscordCommand
    {
        public static readonly new string Name = "name-invite";

        public override async Task Create(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder();
            guildCommand.WithName(Name);
            guildCommand.WithDescription(Strings.NameInviteDescription);
            guildCommand.AddOption(Strings.NameInviteParameter, ApplicationCommandOptionType.String, Strings.NameInviteInviteDescription, required: true);
            guildCommand.AddOption(Strings.NameInviteNameParameter, ApplicationCommandOptionType.String, Strings.NameInviteNameDescription, required: true);

            try
            {
                await client.Rest.CreateGuildCommand(guildCommand.Build(), DharmaConstants.GuildId).ConfigureAwait(false);
            }
            catch (HttpException exception)
            {
                LogTo.Error("Error while creating grant command! {0}", exception.Message);
            }
        }

        public override async Task Respond(SocketSlashCommand command, DiscordSocketClient client)
        {
            await command.RespondAsync($"Not implemented yet but you used {command.Data.Name}").ConfigureAwait(false);
        }
    }
#endif
}

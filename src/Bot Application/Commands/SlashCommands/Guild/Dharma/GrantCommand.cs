using System.Linq;
using System.Threading.Tasks;
using Anotar.Serilog;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordUtilityBot.Constants;
using Newtonsoft.Json;

namespace DiscordUtilityBot.Commands.SlashCommands.Guild.Dharma
{
    public class GrantCommand : BaseSlashDiscordCommand
    {
        public static readonly new string Name = "grant";

        public override async Task Create(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder();
            guildCommand.WithName(Name);
            guildCommand.WithDescription(Strings.GrantCommandDescription);
            guildCommand.AddOption(Strings.GrantCommandUserParameterName, ApplicationCommandOptionType.User, Strings.GrantCommandUserParameterDescription, required: true);

            try
            {
                await client.Rest.CreateGuildCommand(guildCommand.Build(), DharmaConstants.GuildId).ConfigureAwait(false);
            }
            catch (ApplicationCommandException exception)
            {
                var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);
                LogTo.Error("Error while creating grant command! {0}", json);
            }
        }

        public override async Task Respond(SocketSlashCommand command, DiscordSocketClient client)
        {
            var guild = client.GetGuild(DharmaConstants.GuildId);

            var author = guild.GetUser(command.User.Id);
            var mentionedUser = (SocketGuildUser)command.Data.Options.First().Value;

            var allowedToUseGrant = false;
            foreach (var role in author.Roles)
            {
                foreach (var officerRole in DharmaConstants.CanGrantRoles)
                {
                    if (role.Id == officerRole)
                    {
                        allowedToUseGrant = true;
                        break;
                    }
                }

                if (allowedToUseGrant)
                {
                    break;
                }
            }

            if (!allowedToUseGrant)
            {
                await command.RespondAsync(Strings.GrantCommandNotAllowed).ConfigureAwait(false);
                return;
            }

            var hasArksRole = mentionedUser.Roles.Any(x => x.Id == DharmaConstants.ArksOperative);

            if (!hasArksRole)
            {
                await mentionedUser.AddRoleAsync(DharmaConstants.ArksOperative);
                await command.RespondAsync(Strings.GrantCommandResponse).ConfigureAwait(false);
            }
            else
            {
                await command.RespondAsync(Strings.GrantCommandAlreadyGrantedResponse).ConfigureAwait(false);
            }
        }
    }
}

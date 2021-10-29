using Anotar.Serilog;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using DiscordUtilityBot.Constants;
using DiscordUtilityBot.Extensions;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

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
            LogTo.Information("Grant command is being processed..");

            await command.DeferAsync(true).ConfigureAwait(false);
            var guild = (IGuild)client.Guilds.Where(g => g.Id == DharmaConstants.GuildId).First();
            var author = await guild.GetUserAsync(command.User.Id).ConfigureAwait(false);
            if (!author.IsOfficer())
            {
                LogTo.Information("{0} tried to use the grant command but is not an officer", author.Username);
                await command.ModifyOriginalResponseAsync((msg) => msg.Content = Strings.GrantCommandNotAllowed).ConfigureAwait(false);
                return;
            }

            // Get the mentioned user
            var mentionedUser = (SocketGuildUser)command.Data.Options.First().Value;
            var hasHomieRole = mentionedUser.Roles.Any(x => x.Id == DharmaConstants.HomieId);

            if(!hasHomieRole)
            {
                LogTo.Information("{0} tried to grant {1}, but {1} didn't accept the rules yet", author.Username, mentionedUser.Username);
                await command.ModifyOriginalResponseAsync((msg) => msg.Content = Strings.GrantCommandNeedsToAcceptRules).ConfigureAwait(false);
                return;
            }
            
            var hasArksRole = mentionedUser.Roles.Any(x => x.Id == DharmaConstants.ArksOperative);
            if (!hasArksRole)
            {
                await mentionedUser.AddRoleAsync(DharmaConstants.ArksOperative).ConfigureAwait(false);
                LogTo.Information("{0} was granted the arks operative role", mentionedUser.Username);
                await command.ModifyOriginalResponseAsync((msg) => msg.Content = Strings.GrantCommandResponse).ConfigureAwait(false);
            }
            else
            {
                LogTo.Information("{0} already is an arks operative", mentionedUser.Username);
                await command.ModifyOriginalResponseAsync((msg) => msg.Content = Strings.GrantCommandAlreadyGrantedResponse).ConfigureAwait(false);
            }
        }
    }
}

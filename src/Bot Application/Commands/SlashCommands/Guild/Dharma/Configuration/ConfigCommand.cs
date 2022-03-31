using Anotar.Serilog;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using System.Collections.Generic;
using DiscordUtilityBot.Constants;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordUtilityBot.Commands.SlashCommands.Guild.Dharma.Configuration
{
#if DEBUG
    // TODO: Save channel to file
    // Invite updates to invite channel (implement it)
    public class ConfigCommand : BaseSlashDiscordCommand
    {
        public static readonly new string Name = "config";

        public static readonly Configuration DharmaConfiguration = new();

        public override async Task Create(DiscordSocketClient client)
        {
            var guildCommand = new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription(Strings.ConfigureDescription)
                .WithDefaultPermission(false)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName(Strings.ConfigSubCmdNameInviteChannel)
                    .WithDescription(Strings.ConfigSubCmdNameInviteDescription)
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName(Strings.ConfigSetParam)
                        .WithDescription(Strings.ConfigSubCmdSetInviteDescription)
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption(Strings.ConfigSubCmdSetInviteChannelParam, ApplicationCommandOptionType.Channel, Strings.ConfigSubCmdSetInviteChannelDescription, required: true))
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName(Strings.ConfigGetParam)
                        .WithDescription(Strings.ConfigSubCmdGetInviteChannelDescription)
                        .WithType(ApplicationCommandOptionType.SubCommand)));

            try
            {
                var command = await client.Rest.CreateGuildCommand(guildCommand.Build(), DharmaConstants.GuildId).ConfigureAwait(false);
                //var dreamerPerm = new ApplicationCommandPermission(DharmaConstants.Dreamer, ApplicationCommandPermissionTarget.Role, true);
                //var discordOfficerPerm = new ApplicationCommandPermission(DharmaConstants.DiscordOfficer, ApplicationCommandPermissionTarget.Role, true);
                //await command.ModifyCommandPermissions(new [] { dreamerPerm, discordOfficerPerm }).ConfigureAwait(false);
            }
            catch (HttpException exception)
            {
                LogTo.Error("Error while creating grant command! {0}", exception.Message);
            }
        }

        public override async Task Respond(SocketSlashCommand command, DiscordSocketClient client)
        {
            await command.DeferAsync().ConfigureAwait(false);
            var fieldName = command.Data.Options.First().Name;
            var getOrSet = command.Data.Options.First().Options.First().Name;

            var value = command.Data.Options.First().Options.FirstOrDefault().Options?.FirstOrDefault()?.Value;
            var response = "";
            switch (fieldName)
            {
                case Strings.ConfigSubCmdNameInviteChannel:
                    {
                        if (getOrSet == Strings.ConfigGetParam)
                        {
                            response = DharmaConfiguration.InviteUpdateChannel != null
                                ? Strings.GetInviteChannelResponse.Replace("{0}", DharmaConfiguration.InviteUpdateChannel.Mention)
                                : Strings.GetInviteChannelNotSetResponse;
                        }
                        else if (getOrSet == Strings.ConfigSetParam)
                        {
                            if(value is SocketTextChannel channel)
                            {
                                DharmaConfiguration.InviteUpdateChannel = channel;
                                response = Strings.SetInviteChannelResponse.Replace("{0}", DharmaConfiguration.InviteUpdateChannel.Mention);
                            }
                            else
                            {
                                var guildChannel = (IGuildChannel)value;
                                response = Strings.InviteChannelNeedsToBeTextChannel.Replace("{0}", "'<#" + guildChannel.Id + ">'");
                            }
                        }
                    }
                    break;
            }

            await command.ModifyOriginalResponseAsync((msg) => msg.Content = response).ConfigureAwait(false);
        }

        public class Configuration
        {
            public SocketTextChannel InviteUpdateChannel { get; set; }
        }
    }
#endif
}

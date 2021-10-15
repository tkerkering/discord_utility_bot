using System.Threading.Tasks;
using Discord.WebSocket;

namespace DiscordUtilityBot.Commands.SlashCommands
{
    public abstract class BaseSlashDiscordCommand
    {
        // THE NAME SHOULD NEVER HAVE REFERENCES! => Look at the commands to see how to implement the name.
        public static readonly string Name;
        public abstract Task Create(DiscordSocketClient client);
        public abstract Task Respond(SocketSlashCommand command, DiscordSocketClient client);
    }
}

using System;
using System.Linq;
using Discord;
using DiscordUtilityBot.Constants;

namespace DiscordUtilityBot.Extensions
{
    public static class DharmaUserExtension
    {
        public static bool IsAdministrativeOfficer(this IGuildUser user) => user.RoleIds.Any(userRole => DharmaConstants.AdminstrativeOfficers.Contains(userRole));

        public static bool IsOfficer(this IGuildUser user) => user.RoleIds.Any(userRole => DharmaConstants.OfficerRoles.Contains(userRole));
    }
}

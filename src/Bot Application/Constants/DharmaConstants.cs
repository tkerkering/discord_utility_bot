using System.Collections.Generic;

namespace DiscordUtilityBot.Constants
{
    public static class DharmaConstants
    {
        public static readonly ulong GuildId = 551594114454519828;

        // Officer Id's
        public static readonly ulong Dreamer = 743659766064349264;
        public static readonly ulong ExecutiveOfficer = 752244674726723644;
        public static readonly ulong DiscordOfficer = 703775448001151078;
        public static readonly ulong ArtDirector = 874192921958711306;
        public static readonly ulong Recruiter = 787349884999041044;
        public static readonly ulong SupportOfficer = 754836832268582932;

        public static readonly IEnumerable<ulong> CanGrantRoles = new List<ulong>()
        {
            Dreamer,
            ExecutiveOfficer,
            DiscordOfficer,
            ArtDirector,
            Recruiter,
            SupportOfficer
        };

        // The role that will be granted
        public static readonly ulong ArksOperative = 703775060833599599;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents a match in the tournament.
    /// </summary>
    class MatchupModel
    {
        /// <summary>
        /// The two competing teams in this matchup.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        
        /// <summary>
        /// The winner of this matchup.
        /// </summary>
        public TeamModel Winner { get; set; }
        
        /// <summary>
        /// The round number this matchup is part of.
        /// </summary>
        public int MatchupRound { get; set; }
    }
}

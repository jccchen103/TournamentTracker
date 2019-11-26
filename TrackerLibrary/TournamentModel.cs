using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents one tournament.
    /// </summary>
    class TournamentModel
    {
        /// <summary>
        /// The name of this tournament.
        /// </summary>
        public string TournamentName { get; set; }
        /// <summary>
        /// The entry fee for each of the entered teams.
        /// </summary>
        public decimal EntryFee { get; set; }

        /// <summary>
        /// Represents the teams competing in this tournament.
        /// </summary>
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();
        
        /// <summary>
        /// Represents the prizes from this tournament.
        /// </summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();
        
        /// <summary>
        /// Represents the rounds of this tournament, where each round is a
        /// set of matchups.
        /// </summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();
    }
}

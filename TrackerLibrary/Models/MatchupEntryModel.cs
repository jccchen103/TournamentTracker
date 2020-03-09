using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one entry in a matchup.
    /// </summary>
    public class MatchupEntryModel
    {
        public int Id { get; set; }

        /// <summary>
        /// The unique identifier for the competing team.
        /// </summary>
        public int TeamCompetingId { get; internal set; }

        /// <summary>
        /// The team for this matchup entry.
        /// </summary>
        public TeamModel TeamCompeting { get; set; }

        /// <summary>
        /// The score for this team in this matchup.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// The unique identifier for the parent matchup.
        /// </summary>
        public int ParentMatchupId { get; internal set; }

        /// <summary>
        /// The matchup this team came from as the winner.
        /// </summary>
        public MatchupModel ParentMatchup { get; set; }
    }
}

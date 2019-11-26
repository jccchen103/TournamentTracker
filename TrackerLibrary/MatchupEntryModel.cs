using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents one entry in a matchup.
    /// </summary>
    class MatchupEntryModel
    {
        /// <summary>
        /// The team for this matchup entry.
        /// </summary>
        public TeamModel TeamCompeting { get; set; }

        /// <summary>
        /// The score for this team in this matchup.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// The matchup this team came from as the winner.
        /// </summary>
        public MatchupModel ParentMatchup { get; set; }
    }
}

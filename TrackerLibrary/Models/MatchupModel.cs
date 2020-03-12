using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents a match in the tournament.
    /// </summary>
    public class MatchupModel
    {
        /// <summary>
        /// The unique identifier for the matchup.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The two competing teams in this matchup.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
        
        /// <summary>
        /// The unique identifier for the winning team.
        /// </summary>
        public int WinnerId { get; internal set; }

        /// <summary>
        /// The winning team of this matchup.
        /// </summary>
        public TeamModel Winner { get; set; }
        
        /// <summary>
        /// The round number this matchup is part of.
        /// </summary>
        public int MatchupRound { get; set; }

        public string DisplayName { 
            get
            {
                string display = "";
                foreach (MatchupEntryModel me in Entries)
                {
                    if (me.TeamCompeting is null)
                    {
                        display = "Matchup not yet determined";
                        break;
                    }
                    else if (display.Length == 0)
                    {
                        display = me.TeamCompeting.TeamName;
                    }
                    else
                    {
                        display += $" vs. { me.TeamCompeting.TeamName }";
                    }
                }
                return display;
            }
        }
    }
}

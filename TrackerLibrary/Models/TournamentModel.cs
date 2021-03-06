﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one tournament.
    /// </summary>
    public class TournamentModel
    {
        public event EventHandler<DateTime> OnTournamentFinished;

        /// <summary>
        /// The unique identifier for the tournament.
        /// </summary>
        public int Id { get; set; }

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

        public void CompleteTournament()
        {
            // fires off the OnTournamentFinished event
            OnTournamentFinished?.Invoke(this, DateTime.Now);
        }
    }
}

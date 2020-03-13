using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary
{
    public static class TournamentLogic
    {
        // Order our list of teams randomly
        // If the number of teams is not a power of 2, add byes until it is
        // Create the first round of matchups
        // Create every round after that, filling out only the parentMatchup properties
        public static void CreateRounds(TournamentModel model)
        {
            // must have at least 2 teams in a tournament for a match
            if (model.EnteredTeams.Count < 2) { return; }

            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            int teamCount = randomizedTeams.Count;          // number of teams (excluding bys)
            int rounds = (int)Math.Ceiling(Math.Log(teamCount,2));
            int bys = (int)Math.Pow(2, rounds) - teamCount; // number of bypasses needed

            model.Rounds.Add(CreateFirstRound(bys, randomizedTeams));
            CreateOtherRounds(model, rounds);
        }

        public static void UpdateTournamentResults(TournamentModel tournament)
        {
            // set the starting round

            List<MatchupModel> matchupsToUpdate = new List<MatchupModel>();

            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                foreach (MatchupModel rm in round)
                {
                    if (rm.Winner == null  && (rm.Entries.Count == 1 || rm.Entries.Any(x => x.Score != 0)))
                    {
                        matchupsToUpdate.Add(rm);
                    }
                }
            }

            SetWinnersInMatchups(matchupsToUpdate);
            matchupsToUpdate.ForEach(x => GlobalConfig.Connections.UpdateMatchup(x));

            AdvanceWinners(matchupsToUpdate, tournament);

            // TODO: check if the current round is completed
            // set the ending round
            //if (endingRound > startingRound)
            //{
            //    // move on to the next round of the tournament and alert users
            //}
        }

        // TODO: Alert every player in a tournament about an upcoming round

        /// <summary>
        /// Advance the winning team from each of the given matchups to the next round 
        /// (i.e. set the winning team as the team competing in the following round).
        /// </summary>
        /// <param name="matchups">Matchups with a winner to advance.</param>
        /// <param name="tournament">Tournament in which the matchups are part of.</param>
        private static void AdvanceWinners(List<MatchupModel> matchups, TournamentModel tournament)
        {
            foreach (MatchupModel m in matchups)
            {
                foreach (List<MatchupModel> round in tournament.Rounds)
                {
                    foreach (MatchupModel rm in round)
                    {
                        foreach (MatchupEntryModel me in rm.Entries)
                        {
                            if (me.ParentMatchup != null && me.ParentMatchupId == m.Id)
                            {
                                me.TeamCompeting = m.Winner;
                                GlobalConfig.Connections.UpdateMatchup(rm);
                            }
                        }
                    }
                }
            }
        }

        private static void SetWinnersInMatchups(List<MatchupModel> matchups)
        {
            foreach (MatchupModel m in matchups)
            {
                // Check for bys
                if (m.Entries.Count == 1)
                {
                    m.Winner = m.Entries[0].TeamCompeting;
                    continue;
                }

                MatchupEntryModel entryOne = m.Entries[0];
                MatchupEntryModel entryTwo = m.Entries[1];

                if (entryOne.Score > entryTwo.Score)
                {
                    m.Winner = entryOne.TeamCompeting;
                }
                else if (entryOne.Score < entryTwo.Score)
                {
                    m.Winner = entryTwo.TeamCompeting;
                }
                else
                {
                    throw new Exception("This application does not allow ties.");
                }

            }
        }

        private static void CreateOtherRounds(TournamentModel model, int rounds)
        {
            int round = 2;
            List<MatchupModel> previousRound = model.Rounds[0];
            List<MatchupModel> currentRound = new List<MatchupModel>();
            MatchupModel currentMatchup = new MatchupModel();

            while (round <= rounds)
            {
                foreach (MatchupModel matchup in previousRound)
                {
                    currentMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = matchup });

                    if (currentMatchup.Entries.Count > 1)
                    {
                        currentMatchup.MatchupRound = round;
                        currentRound.Add(currentMatchup);
                        currentMatchup = new MatchupModel();
                    }
                }
                model.Rounds.Add(currentRound);

                previousRound = currentRound;
                currentRound = new List<MatchupModel>();
                round++;
            }
        }

        private static List<MatchupModel> CreateFirstRound(int bys, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            MatchupModel currentMatchup = new MatchupModel { MatchupRound = 1 };
            foreach (TeamModel team in teams)
            {
                currentMatchup.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

                if (bys > 0 || currentMatchup.Entries.Count > 1)
                {
                    if (bys > 0) { 
                        bys--;
                    }
                    output.Add(currentMatchup);
                    currentMatchup = new MatchupModel { MatchupRound = 1 };
                }
            }

            return output;
        }

        /// <summary>
        /// Randomize the order of a list of teams.
        /// </summary>
        /// <param name="teams">The team models to randomize.</param>
        /// <returns>The randomized list of teams.</returns>
        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            return teams.OrderBy(t => Guid.NewGuid()).ToList();
        }

    }
}

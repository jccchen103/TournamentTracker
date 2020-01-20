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
            // TODO: test CreateRounds method
        public static void CreateRounds(TournamentModel model)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            int teamCount = randomizedTeams.Count;          // number of teams (excluding bys)
            int rounds = (int)Math.Ceiling(Math.Log(teamCount,2));
            int bys = (int)Math.Pow(2, rounds) - teamCount; // number of bypasses needed

            model.Rounds.Add(CreateFirstRound(bys, randomizedTeams));
            CreateOtherRounds(model, rounds);
        }

        // TODO: Update the scores of the tournament model

        // TODO: Alert every player in a tournament about a upcoming round

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
                    if (bys > 0) { bys--; }
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

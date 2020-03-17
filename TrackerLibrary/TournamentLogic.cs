﻿using System;
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
        // Create the first round of matchups with the entered teams
        // Create every round after that with the parentMatchup property
        public static void CreateRounds(TournamentModel model)
        {
            // must have at least 2 teams in a tournament for a match
            if (model.EnteredTeams.Count < 2) { return; }

            List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
            int teamCount = randomizedTeams.Count;          // number of teams (excluding byes)
            int rounds = (int)Math.Ceiling(Math.Log(teamCount,2));
            int byes = (int)Math.Pow(2, rounds) - teamCount; // number of byes needed

            model.Rounds.Add(CreateFirstRound(byes, randomizedTeams));
            CreateOtherRounds(model, rounds);

            UpdateByes(model); // automatically advance teams that received a bye
        }

        /// <summary>
        /// Set and advance the default winner for all byes in a tournament.
        /// </summary>
        /// <param name="tournament">The tournament model to update.</param>
        private static void UpdateByes(TournamentModel tournament)
        {
            // get all the bye matchups (all are in the first round)
            List<MatchupModel> byeMatchups = tournament.Rounds[0].Where(x => x.Entries.Count == 1).ToList();

            foreach (MatchupModel m in byeMatchups)
            {
                SetMatchupWinner(m);
                AdvanceWinner(m, tournament);
            }
        }

        public static void UpdateTournamentResults(TournamentModel tournament, MatchupModel matchupToUpdate)
        {
            int startingRound = GetCurrentRound(tournament);

            SetMatchupWinner(matchupToUpdate);
            GlobalConfig.Connections.UpdateMatchup(matchupToUpdate);

            AdvanceWinner(matchupToUpdate, tournament);

            int endingRound = GetCurrentRound(tournament);
            if (endingRound > tournament.Rounds.Count)
            {
                // last round is finished
                CompleteTournament(tournament);
            }
            else if (endingRound > startingRound)
            {
                tournament.AlertUsersToNewRound();
            }
        }

        private static void AlertUsersToNewRound(this TournamentModel tournament)
        {
            int currRoundNum = GetCurrentRound(tournament);
            List<MatchupModel> currRound = tournament.Rounds.Where(x => x.First().MatchupRound == currRoundNum).First();

            // TODO: Alert every member competing in the current round of this tournament of their matchup
            foreach (MatchupModel matchup in currRound)
            {
                foreach (MatchupEntryModel me in matchup.Entries)
                {
                    string subject = $"Your Next {tournament.TournamentName} Matchup";
                    StringBuilder body = new StringBuilder();

                    TeamModel competitor = matchup.Entries.Where(x => x.TeamCompeting != me.TeamCompeting).FirstOrDefault()?.TeamCompeting;
                    if (competitor is null)
                    {
                        body.AppendLine("<h1>You have a bye week this round!</h1>");
                        body.AppendLine("<p>Enjoy your round off.</p>");
                        body.AppendLine("<p>~Tournament Tracker</p>");
                    }
                    else
                    {
                        body.AppendLine("<h1>You have a new matchup!</h1>");
                        body.Append("<p><strong>Competitor: </strong>");
                        body.AppendLine(competitor.TeamName + "</p>");
                        body.AppendLine("<p>Have a great time.</p>");
                        body.AppendLine("<p>~Tournament Tracker</p>");
                    }

                    List<string> to = new List<string>();
                    foreach (PersonModel member in me.TeamCompeting.TeamMembers)
                    {
                        to.Add(member.Email);
                    }
                    EmailLogic.SendEmail(to, subject, body.ToString());
                }
            }
        }

        /// <summary>
        /// Returns the round the tournament is in, or one more than its total number of rounds
        /// if all rounds of the tournament are finished.
        /// </summary>
        /// <param name="tournament">The tournament being checked.</param>
        /// <returns>The sum of 1 plus the number of completed rounds in the tournament.</returns>
        public static int GetCurrentRound(TournamentModel tournament)
        {
            int currRound = 1;
            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                if (round.Any(x => x.Winner is null))
                {
                    return currRound;   // first round in which not all matchups have a winner
                }
                currRound++;
            }

            return currRound;   // all rounds are completed
        }

        private static void CompleteTournament(TournamentModel tournament)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Advance the winning team of the given matchup to the next round 
        /// (i.e. set the winning team as the team competing in the following round).
        /// </summary>
        /// <param name="matchup">The matchup with a winner to advance.</param>
        /// <param name="tournament">Tournament in which the matchup is part of.</param>
        private static void AdvanceWinner(MatchupModel matchup, TournamentModel tournament)
        {
            for (int i = 1; i < tournament.Rounds.Count; i++)
            {
                foreach (MatchupModel rm in tournament.Rounds[i])
                {
                    foreach (MatchupEntryModel me in rm.Entries)
                    {
                        if (me.ParentMatchup == matchup)
                        {
                            me.TeamCompeting = matchup.Winner;
                            GlobalConfig.Connections.UpdateMatchup(rm);
                            return;
                        }
                    }
                }
            }
        }
        
        private static void SetMatchupWinner(MatchupModel matchup)
        {
            // Check for bye
            if (matchup.Entries.Count == 1)
            {
                matchup.Winner = matchup.Entries[0].TeamCompeting;
                return;
            }

            MatchupEntryModel entryOne = matchup.Entries[0];
            MatchupEntryModel entryTwo = matchup.Entries[1];

            if (entryOne.Score > entryTwo.Score)
            {
                matchup.Winner = entryOne.TeamCompeting;
            }
            else if (entryOne.Score < entryTwo.Score)
            {
                matchup.Winner = entryTwo.TeamCompeting;
            }
            else
            {
                throw new Exception("This application does not allow ties.");
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

        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            MatchupModel currentMatchup = new MatchupModel { MatchupRound = 1 };
            foreach (TeamModel team in teams)
            {
                currentMatchup.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

                if (byes > 0 || currentMatchup.Entries.Count > 1)
                {
                    if (byes > 0) { 
                        byes--;
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

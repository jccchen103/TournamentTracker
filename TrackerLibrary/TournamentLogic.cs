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

        private static void CompleteTournament(TournamentModel tournament)
        {
            GlobalConfig.Connections.CompleteTournament(tournament);

            // Announce tournament results and prize distributions
            MatchupModel lastMatchup = tournament.Rounds.Last().First();
            TeamModel winner = lastMatchup.Winner;
            TeamModel runnerUp = lastMatchup.Entries.Where(x => x.TeamCompeting != winner).First().TeamCompeting;
            string subject = $"{tournament.TournamentName} has concluded!";

            StringBuilder body = new StringBuilder();
            body.AppendLine("<h2>WE HAVE A WINNER!</h2>");
            body.AppendLine($"<p>Congratulations to <b>{winner.TeamName}</b> on a great tournament.</p>");
            body.AppendLine("<br>");

            if (tournament.Prizes.Count > 0)
            {
                decimal totalIncome = tournament.EnteredTeams.Count * tournament.EntryFee;
                PrizeModel firstPlacePrize = tournament.Prizes.Where(x => x.PlaceNumber == 1).FirstOrDefault();
                PrizeModel secondPlacePrize = tournament.Prizes.Where(x => x.PlaceNumber == 2).FirstOrDefault();

                if (firstPlacePrize != null)
                {
                    decimal winnerPrizeAmt = CalculatePayout(firstPlacePrize, totalIncome);
                    body.AppendLine($"<p>{winner.TeamName} will receive ${string.Format("{0:0.00}", winnerPrizeAmt)}!</p>");
                }

                if (secondPlacePrize != null)
                {
                    decimal runnerUpPrizeAmt = CalculatePayout(secondPlacePrize, totalIncome);
                    body.AppendLine($"<p>{runnerUp.TeamName} will receive ${string.Format("{0:0.00}", runnerUpPrizeAmt)}.</p>");
                }

                if (firstPlacePrize != null || secondPlacePrize != null)
                {
                    body.AppendLine("<br>");
                }
            }

            body.AppendLine("<p>Hope everyone had a great time.</p>");
            body.AppendLine("<p>~Tournament Tracker</p>");

            List<string> bcc = new List<string>();
            foreach (TeamModel t in tournament.EnteredTeams)
            {
                foreach (PersonModel player in t.TeamMembers)
                {
                    bcc.Add(player.Email);
                }
            }

            EmailLogic.SendEmail(new List<string>(), bcc, subject, body.ToString());

            // Complete tournament
            tournament.CompleteTournament();
        }

        private static decimal CalculatePayout(PrizeModel prize, decimal totalIncome)
        {
            decimal prizeAmount = prize.PrizeAmount;

            if (prize.PrizeAmount == 0)
            {
                prizeAmount = Decimal.Multiply(totalIncome, Convert.ToDecimal(prize.PrizePercentage / 100));
            }

            return prizeAmount;
        }

        public static void AlertUsersToNewRound(this TournamentModel tournament)
        {
            int currRoundNum = GetCurrentRound(tournament);
            List<MatchupModel> currRound = tournament.Rounds.Where(x => x.First().MatchupRound == currRoundNum).First();

            // Alert every member competing in the current round of this tournament of their matchup
            foreach (MatchupModel matchup in currRound)
            {
                foreach (MatchupEntryModel me in matchup.Entries)
                {
                    string subject = $"Your Next {tournament.TournamentName} Matchup";
                    string body = NewRoundEntryBody(matchup, me);

                    List<string> to = new List<string>();
                    me.TeamCompeting.TeamMembers.ForEach(x => to.Add(x.Email));

                    EmailLogic.SendEmail(to, subject, body);
                }
            }
        }

        /// <summary>
        /// Generates the body of an email to alert participating players of their matchup
        /// for the new round, tailored to the team competing in the given matchup entry.
        /// </summary>
        /// <returns>A HTML-formatted string for the body of an email.</returns>
        private static string NewRoundEntryBody(MatchupModel matchup, MatchupEntryModel me)
        {
            StringBuilder body = new StringBuilder();

            TeamModel competitor = matchup.Entries.Where(x => x.TeamCompeting != me.TeamCompeting).FirstOrDefault()?.TeamCompeting;
            if (competitor is null)
            {
                body.AppendLine("<h2>You have a bye week this round!</h2>");
                body.AppendLine("<p>Enjoy your round off.</p>");
                body.AppendLine("<p>~Tournament Tracker</p>");
            }
            else
            {
                body.AppendLine("<h2>You have a new matchup!</h2>");
                body.Append("<p><strong>Competitor: </strong>");
                body.AppendLine(competitor.TeamName + "</p>");
                body.AppendLine("<p>Have a great time.</p>");
                body.AppendLine("<p>~Tournament Tracker</p>");
            }

            return body.ToString();
        }

        /// <summary>
        /// Indicates the round the given tournament is in, or if it is finished.
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

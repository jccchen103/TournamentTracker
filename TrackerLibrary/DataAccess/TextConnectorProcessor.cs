using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        /// <summary>
        /// Gets the full file path of a file.
        /// </summary>
        /// <param name="filename">Name of the file.</param>
        /// <returns>The full path to the specified file.</returns>
        public static string FullFilePath(this string filename)
        {
            return ConfigurationManager.AppSettings["filePath"] + "\\" + filename;
        }

        /// <summary>
        /// Reads a given file and returns it as a list of strings.
        /// If the file does not exist, return an empty list of strings.
        /// </summary>
        /// <param name="file">The full path name to a file.</param>
        /// <returns>The lines in the given file.</returns>
        public static List<string> LoadFile(this string file)
        {
            // if file doesn't exist, return an empty list of strings
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        /// <summary>
        /// Convert the Prizes data from a text file to a list of PrizeModel.
        /// </summary>
        /// <param name="prizesFileLines">The lines of a text file with Prizes data.</param>
        /// <returns>The resulting list of PrizeModel.</returns>
        public static List<PrizeModel> ConvertToPrizeModels(this List<string> prizesFileLines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in prizesFileLines)
            {
                string[] cols = line.Split(',');

                // create a PrizeModel with the data from line
                PrizeModel p = new PrizeModel
                {
                    Id = int.Parse(cols[0]),
                    PlaceNumber = int.Parse(cols[1]),
                    PlaceName = cols[2],
                    PrizeAmount = decimal.Parse(cols[3]),
                    PrizePercentage = double.Parse(cols[4]),
                    TournamentId = int.Parse(cols[5])
                };

                output.Add(p);
            }

            return output;
        }

        /// <summary>
        /// Convert the data lines from a People text file to a list of PersonModel.
        /// </summary>
        /// <param name="peopleFileLines">The lines of a text file with People data.</param>
        /// <returns>The list of PersonModel.</returns>
        public static List<PersonModel> ConvertToPersonModels(this List<string> peopleFileLines)
        {
            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in peopleFileLines)
            {
                string[] cols = line.Split(',');

                // create a PersonModel with the data from line
                PersonModel p = new PersonModel
                {
                    Id = int.Parse(cols[0]),
                    FirstName = cols[1],
                    LastName = cols[2],
                    Email = cols[3]
                };

                output.Add(p);
            }

            return output;
        }

        /// <summary>
        /// Convert the data lines from a Teams csv file to a list of TeamModel.
        /// </summary>
        /// <param name="teamsFileLines">The lines of a file with the Teams data.</param>
        /// <returns>The list of TeamModel.</returns>
        public static List<TeamModel> ConvertToTeamModels(this List<string> teamsFileLines)
        {
            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in teamsFileLines)
            {
                string[] cols = line.Split(',');
                string[] personIds = cols[2].Split('|');  // ids of team members

                TeamModel t = new TeamModel
                {
                    Id = int.Parse(cols[0]),
                    TeamName = cols[1]
                };
                
                // convert team member id strings to a list of person model
                foreach (string id in personIds)
                {
                    int idNum = int.Parse(id);
                    t.TeamMembers.Add(people.Where(x => x.Id == idNum).First());
                }

                output.Add(t);
            }

            return output;
        }

        public static List<TournamentModel> ConvertToTournamentModels(this List<string> fileLines)
        {
            // id, TournamentName, EntryFee, pipe-separated team ids, pipe-separated prize ids, Rounds (id^id^id^id|id^id|id) 
            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            foreach (string line in fileLines)
            {
                string[] cols = line.Split(',');

                TournamentModel tm = new TournamentModel
                {
                    Id = int.Parse(cols[0]),
                    TournamentName = cols[1],
                    EntryFee = decimal.Parse(cols[2])
                };

                string[] teamIds = cols[3].Split('|');
                foreach (string id in teamIds)
                {
                    int idNum = int.Parse(id);
                    tm.EnteredTeams.Add(teams.Where(x => x.Id == idNum).First());
                }

                string[] prizeIds = cols[4].Split('|');
                foreach (string id in prizeIds)
                {
                    int idNum = int.Parse(id);
                    tm.Prizes.Add(prizes.Where(x => x.Id == idNum).First());
                }

                string[] rounds = cols[5].Split('|');
                // TODO: Capture Rounds information
                //foreach (string round in rounds)
                //{
                //    List<MatchupModel> roundMatchups = new List<MatchupModel>();
                //    string[] roundMatchupIds = round.Split('^');
                //    foreach (string id in roundMatchupIds)
                //    {
                //        int idNum = int.Parse(id);
                //        roundMatchups.Add(matchups.Where(x => x.Id == idNum).First());
                //    }
                //    // add list of matchups to tm.Rounds
                //    tm.Rounds.Add(roundMatchups);
                //}

                output.Add(tm);
            }

            return output;
        }

        /// <summary>
        /// Converts prize models to a list of string and writes it to the specified csv file.
        /// </summary>
        /// <param name="prizes">The list of prize models to be saved.</param>
        /// <param name="fileName">Name of file to be written to.</param>
        public static void SaveToPrizesFile(this List<PrizeModel> prizes, string fileName)
        {
            // convert prize models to lines in a list
            List<string> lines = new List<string>();

            foreach (PrizeModel p in prizes)
            {
                lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage},{p.TournamentId}");
            }

            // write all lines in the list to the text file
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        /// <summary>
        /// Saves person models to a csv file with the specified file name.
        /// </summary>
        /// <param name="people">The list of person models to be saved.</param>
        /// <param name="fileName">Name of the file that will be written to.</param>
        public static void SaveToPeopleFile(this List<PersonModel> people, string fileName)
        {
            // convert prize models to lines in a list
            List<string> lines = new List<string>();

            foreach (PersonModel p in people)
            {
                lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.Email}");
            }

            // write all lines in the list to the text file
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        /// <summary>
        /// Saves team models to a csv file with the given file name.
        /// A team model will be recorded in the following format: 
        /// team id, team name, team members' person id separated by the pipe.
        /// </summary>
        /// <param name="teams">The list of team models to be saved.</param>
        /// <param name="fileName">Name of the csv file to be written to.</param>
        public static void SaveToTeamsFile(this List<TeamModel> teams, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel t in teams)
            {
                List<string> memberIds = t.TeamMembers.Select(x => x.Id.ToString()).ToList();
                string record = $"{t.Id},{t.TeamName},{string.Join("|", memberIds)}";
                lines.Add(record);
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static void SaveRoundsToFile (this TournamentModel tournament, string matchupsFile, string matchupEntriesFile)
        {
            // loop through rounds of the tournament model
            // loop through each matchup in the round
            // get id for the new matchup and save the record
            // loop through each entry, get the id, and save it

            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    SaveToMatchupsFile(matchup, matchupsFile);
                }
            }
        }

        private static void SaveToMatchupsFile(this MatchupModel matchup)
        {
            // Set the id of matchup to be the top id + 1
            List<MatchupModel> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile().ConvertToMatchupModels();
            int nextId = matchups.Count() > 0 ? matchups.OrderByDescending(x => x.Id).First().Id : 1;
            matchup.Id = nextId;

            // Add the matchup record to be saved
            matchups.Add(matchup);

            foreach (MatchupEntryModel entry in matchup.Entries)
            {
                // TODO: Save each entry in this matchup
                entry.SaveToMatchupEntriesFile();
                // entry = matchup id, team id, score, parent matchup id
            }

            // Convert matchup models to a list of strings, then save to the matchup file
            List<string> lines = new List<string>();
            foreach (MatchupModel m in matchups)
            {
                lines.Add($"{ m.Id },{ ConvertMatchupEntryListToString(m.Entries) },{ m.Winner?.Id },{ m.MatchupRound }");
            }            
            File.WriteAllLines(GlobalConfig.MatchupsFile.FullFilePath(), lines);
        }

        private static void SaveToMatchupEntriesFile(this MatchupEntryModel entry, int matchupId)
        {
            List<MatchupEntryModel> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile().ConvertToMatchupEntryModels();

            // Add the matchup record to be saved
            entries.Add(entry);

            // Convert and save matchup entry models as lines of strings
            List<string> lines = new List<string>();
            foreach (MatchupEntryModel e in entries)
            {
                lines.Add($"{ matchupId },{ e.TeamCompeting?.Id },{ e.Score },{ e.ParentMatchup?.Id }");
            }
            File.WriteAllLines(GlobalConfig.MatchupEntriesFile.FullFilePath(), lines);
        }

        public static List<MatchupModel> ConvertToMatchupModels(this List<string> matchupsFileLines)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            foreach (string line in matchupsFileLines)
            {
                string[] cols = line.Split(',');
                // matchup id, team entries (pipe-delimited ids), winner (a team id), matchupRound
                MatchupModel m = new MatchupModel
                {
                    Id = int.Parse(cols[0]),
                    Entries = ConvertStringToMatchupEntryModels(cols[0], cols[1]),
                    Winner = cols[2].Length == 0 ? null : LookupTeamById(cols[2]),
                    MatchupRound = int.Parse(cols[3])
                };
                output.Add(m);
            }
            return output;
        }

        public static List<MatchupEntryModel> ConvertToMatchupEntryModels(this List<string> matchupEntriesFileLines)
        {
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();

            foreach (string line in matchupEntriesFileLines)
            {
                // matchup_id, team_competing_id, score, parent_matchup_id
                string[] cols = line.Split(',');

                MatchupEntryModel me = new MatchupEntryModel
                {
                    TeamCompeting = cols[1].Length == 0 ? null : LookupTeamById(cols[1]),
                    Score = double.Parse(cols[2]),
                    ParentMatchup = cols[3].Length == 0 ? null : LookupMatchupById(cols[3])
                };
            }

            return output;
        }

        private static MatchupModel LookupMatchupById(string id)
        {
            List<string> matchups = GlobalConfig.MatchupsFile.FullFilePath().LoadFile();

            foreach (string matchup in matchups)
            {
                string[] cols = matchup.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingMatchups = new List<string>();
                    matchingMatchups.Add(matchup);
                    return matchingMatchups.ConvertToMatchupModels().First();
                }
            }

            return null;
        }

        /// <summary>
        /// Finds the team model with the given team id.
        /// </summary>
        /// <param name="id">The id of the team to be looked up.</param>
        /// <returns>The team model with the passed-in id, or null if there isn't one.</returns>
        private static TeamModel LookupTeamById(string id)
        {
            List<string> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile();
            foreach (string team in teams)
            {
                string[] cols = team.Split(',');
                if (cols[0] == id.ToString())
                {
                    List<string> matchingTeams = new List<string>();
                    matchingTeams.Add(team);
                    return matchingTeams.ConvertToTeamModels().First();
                }
            }

            return null;    // no matching team

            //List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
            //return teams.Where(x => x.Id == id).FirstOrDefault();
        }

        private static List<MatchupEntryModel> ConvertStringToMatchupEntryModels(string matchupId, string competingTeamsIds)
        {
            string[] teamIds = competingTeamsIds.Split('|');

            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<string> entries = GlobalConfig.MatchupEntriesFile.FullFilePath().LoadFile();
            List<string> matchingEntries = new List<string>();

            foreach (string teamId in teamIds)
            {
                foreach (string entry in entries)
                {
                    string[] cols = entry.Split(',');

                    if (cols[0] == matchupId && cols[1] == teamId)
                    {
                        matchingEntries.Add(entry);
                    }
                }
            }

            output = matchingEntries.ConvertToMatchupEntryModels();

            return output;
        }

        public static void SaveToTournamentsFile(this List<TournamentModel> tournaments, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TournamentModel tm in tournaments)
            {
                List<string> teamIds = tm.EnteredTeams.Select(x => x.Id.ToString()).ToList();
                List<string> prizeIds = tm.Prizes.Select(x => x.Id.ToString()).ToList();
                string record = $"{tm.Id},{tm.TournamentName},{tm.EntryFee}," +
                        $"{string.Join("|", teamIds)},{string.Join("|", prizeIds)}," +
                        $"{ConvertRoundListToString(tm.Rounds)}";
                lines.Add(record);
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        /// <summary>
        /// Convert a list of matchup entry models to a (pipe-delimited) string of the team id for each entry model.
        /// </summary>
        /// <param name="entries">Matchup entry models to be converted.</param>
        /// <returns>A string of pipe-delimited ids, or the empty string if entries is null.</returns>
        private static object ConvertMatchupEntryListToString(List<MatchupEntryModel> entries)
        {
            List<string> teamIds = entries.Select(x => x.TeamCompeting.Id.ToString()).ToList();
            return string.Join("|", teamIds);
        }

        // TODO: Make sure rounds of a tournament model is saved properly
        /// <summary>
        /// Convert the Rounds of a Tournament to a string of matchup ids,
        /// in which each round is separated by '|', 
        /// and each matchup id of a round is separated by '^'.
        /// </summary>
        /// <param name="rounds">The tournament rounds to be converted.</param>
        /// <returns>A string of ids that represents the rounds of a tournament.</returns>
        private static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            if (rounds.Count == 0)
            {
                return "";
            }

            List<string> roundList = new List<string>();
            foreach (List<MatchupModel> round in rounds)
            {
                List<string> matchupIds = round.Select(x => x.Id.ToString()).ToList();
                roundList.Add(string.Join("^", matchupIds));
            }

            return string.Join("|", roundList);
        }
        
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    /// <summary>
    /// Class that connects to the database.
    /// </summary>
    public class SqlConnector : IDataConnection
    {
        private const string db = "Tournaments";

        /// <summary>
        /// Sets the id of a person model and saves the person to the database.
        /// </summary>
        /// <param name="model">The person model with the data to be inserted.</param>
        public void CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                var p = new DynamicParameters();
                p.Add("f_name", model.FirstName);
                p.Add("l_name", model.LastName);
                p.Add("email_address", model.Email);
                p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("people_insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");
            }
        }

        /// <summary>
        /// Sets the id of a prize model and saves the prize to the database.
        /// </summary>
        /// <param name="model">The prize model with the data to be inserted.</param>
        public void CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                var p = new DynamicParameters();
                p.Add("p_place_number", model.PlaceNumber);
                p.Add("p_place_name", model.PlaceName);
                p.Add("p_amount", model.PrizeAmount);
                p.Add("p_percentage", model.PrizePercentage);
                p.Add("p_tournament_id", model.TournamentId);
                p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("prizes_insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");
            }
        }

        /// <summary>
        /// Sets the id of a team model and saves the team to the database, 
        /// connecting the team members to their team in the database.
        /// </summary>
        /// <param name="model">The team with team members to the inserted.</param>
        public void CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                // Insert team name and set the team id
                var p = new DynamicParameters();
                p.Add("p_team_name", model.TeamName);
                p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("teams_insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@id");

                // Insert each team member with the team id that was just set
                foreach (PersonModel member in model.TeamMembers)
                {
                    p = new DynamicParameters();    // overwrite p
                    p.Add("p_team_id", model.Id);
                    p.Add("p_person_id", member.Id);

                    connection.Execute("team_members_insert", p, commandType: CommandType.StoredProcedure);
                }
            }
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                SaveTournament(model, connection);

                SaveTournamentPrizes(model);

                SaveTournamentEntries(model, connection);

                SaveTournamentRounds(model, connection);
            }
        }

        /// <summary>
        /// Save the matchups and matchup entries for a tournament to the database.
        /// </summary>
        /// <param name="model">The tournament whose matchups and entries are to be saved.</param>
        /// <param name="connection">A connection to the database to be saved to.</param>
        private void SaveTournamentRounds(TournamentModel model, IDbConnection connection)
        {
            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    var p = new DynamicParameters();
                    p.Add("round", matchup.MatchupRound);
                    p.Add("tournament", model.Id);
                    p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("matchups_insert", p, commandType: CommandType.StoredProcedure);

                    matchup.Id = p.Get<int>("@id");

                    foreach (MatchupEntryModel entry in matchup.Entries)
                    {
                        p = new DynamicParameters();
                        p.Add("matchup", matchup.Id);

                        if (entry.ParentMatchup is null)
                        {
                            p.Add("parent", null);
                        } 
                        else
                        {
                            p.Add("parent", entry.ParentMatchup.Id);
                        }
                        
                        if (entry.TeamCompeting is null)
                        {
                            p.Add("team_competing", null);
                        }
                        else
                        {
                            p.Add("team_competing", entry.TeamCompeting.Id);
                        }
                        p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("matchup_entries_insert", p, commandType: CommandType.StoredProcedure);
                    }
                }
            }
        }

        /// <summary>
        /// Save the entries of a given tournament.
        /// </summary>
        /// <param name="model">A tournament model with entries to be saved.</param>
        /// <param name="connection">A connection to the database to be saved to.</param>
        private void SaveTournamentEntries(TournamentModel model, IDbConnection connection)
        {
            DynamicParameters p;
            foreach (TeamModel team in model.EnteredTeams)
            {
                p = new DynamicParameters();
                p.Add("p_tournament_id", model.Id);
                p.Add("p_team_id", team.Id);

                connection.Execute("tournament_entries_insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentPrizes(TournamentModel model)
        {
            foreach (PrizeModel prize in model.Prizes)
            {
                prize.TournamentId = model.Id;
                CreatePrize(prize);
            }
        }

        /// <summary>
        /// Save a tournament and fill in the id property of the model.
        /// </summary>
        /// <param name="model">Model with tournament data and incomplete id property.</param>
        /// <param name="connection">A connection to the database to be saved to.</param>
        private void SaveTournament(TournamentModel model, IDbConnection connection)
        {
            var p = new DynamicParameters();
            p.Add("t_name", model.TournamentName);
            p.Add("fee", model.EntryFee);
            p.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("tournaments_insert", p, commandType: CommandType.StoredProcedure);

            model.Id = p.Get<int>("@id");
        }

        public List<PersonModel> GetPeople()
        {
            List<PersonModel> output = new List<PersonModel>();
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                output = connection.Query<PersonModel>("people_all").ToList();
            }
            return output;
        }

        public List<TeamModel> GetTeams()
        {
            List<TeamModel> output = new List<TeamModel>();
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                output = connection.Query<TeamModel>("teams_all").ToList();

                // Fill the team members of each team
                Dapper.DynamicParameters p;
                foreach (TeamModel t in output)
                {
                    p = new DynamicParameters();
                    p.Add("selected_team", t.Id);

                    t.TeamMembers = connection.Query<PersonModel>(
                            "team_members_by_team", p, commandType: CommandType.StoredProcedure).ToList();
                }
            }
            
            return output;
        }
    }
}

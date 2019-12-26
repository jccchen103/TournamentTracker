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
        /// Saves a person to the database.
        /// </summary>
        /// <param name="model">The person model with the data to be inserted.</param>
        /// <returns>The person model that was inserted with its id filled.</returns>
        public PersonModel CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                var p = new DynamicParameters();
                p.Add("p_first_name", model.FirstName);
                p.Add("p_last_name", model.LastName);
                p.Add("p_email", model.Email);
                p.Add("@p_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("people_insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@p_id");
            }

            return model;
        }

        /// <summary>
        /// Saves a prize to the database.
        /// </summary>
        /// <param name="model">The prize model with the data to be inserted.</param>
        /// <returns>The prize that was inserted with its id filled.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                var p = new DynamicParameters();
                p.Add("p_place_number", model.PlaceNumber);
                p.Add("p_place_name", model.PlaceName);
                p.Add("p_prize_amount", model.PrizeAmount);
                p.Add("p_percentage", model.PrizePercentage);
                p.Add("@p_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("prizes_insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@p_id");
            }

            return model;
        }

        /// <summary>
        /// Saves a team and connects the team members to their team in the database.
        /// </summary>
        /// <param name="model">The team with team members to the inserted.</param>
        /// <returns>The passed in team model with its team id set.</returns>
        public TeamModel CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.GetConnectionString(db)))
            {
                // insert team name and set the team id
                var p = new DynamicParameters();
                p.Add("p_team_name", model.TeamName);
                p.Add("@p_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("teams_insert", p, commandType: CommandType.StoredProcedure);

                model.Id = p.Get<int>("@p_id");

                // insert each team member with the team id that was just set
                foreach (PersonModel member in model.TeamMembers)
                {
                    p = new DynamicParameters();    // overwrite p
                    p.Add("p_team_id", model.Id);
                    p.Add("p_person_id", member.Id);

                    connection.Execute("team_members_insert", p, commandType: CommandType.StoredProcedure);
                }

                return model;
            }

            
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
    }
}

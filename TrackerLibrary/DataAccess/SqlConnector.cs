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
        /// <summary>
        /// Saves a prize to the database.
        /// </summary>
        /// <param name="model">The prize information to be inserted.</param>
        /// <returns>The prize information that was inserted, including the id.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.GetConnectionString("Tournaments")))
            {
                // TODO: Save to prizes table in db.
                var p = new DynamicParameters();
                p.Add("p_place_number", model.PlaceNumber);
                p.Add("p_place_name", model.PlaceName);
                p.Add("p_prize_amount", model.PrizeAmount);
                p.Add("p_percentage", model.PrizePercentage);
                p.Add("@p_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("prizes_insert", p, commandType: CommandType.StoredProcedure);

                // TODO: Get id of the inserted prize and set Id of model.
                model.Id = p.Get<int>("@p_id");
            }
            return model;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using TrackerLibrary.Models;

namespace TrackerLibrary
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
            // TODO: Save to prizes table in db.
            // TODO: Get id of the inserted prize and set Id of model.
            return model;
        }
    }
}

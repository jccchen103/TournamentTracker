using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    /// <summary>
    /// Class for global data source info.
    /// </summary>
    public static class GlobalConfig
    {
        public static IDataConnection Connections { get; private set; }

        /// <summary>
        /// Set connections to the specified data source(s).
        /// </summary>
        /// <param name="db">
        /// The type of database to use as the data source (sql or text file).
        /// </param>
        public static void InitializeConnections(DatabaseType db)
        {
            if (db == DatabaseType.Sql)
            {
                // TODO: Set up the SQL Connection
                SqlConnector sql = new SqlConnector();
                Connections = sql;
            }
            else if (db == DatabaseType.TextFile)
            {
                // TODO: Create the Text Connection
                TextConnector text = new TextConnector();
                Connections = text;
            }
        }

        public static string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}

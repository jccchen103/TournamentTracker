using System;
using System.Collections.Generic;
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
        public static List<IDataConnection> Connections { get; private set; }

        /// <summary>
        /// Set connections to the specified data source(s).
        /// </summary>
        /// <param name="mysqlDB">If true, use a mySQL database as a data source.</param>
        /// <param name="textFiles">If true, use text files as a data source.</param>
        public static void InitializeConnections(bool mysqlDB, bool textFiles)
        {
            Connections = new List<IDataConnection>();

            if (mysqlDB)
            {
                // TODO: Set up the SQL Connection
                SqlConnector sql = new SqlConnector();
                Connections.Add(sql);
            }

            if (textFiles)
            {
                // TODO: Create the Text Connection
                TextConnector text = new TextConnector();
                Connections.Add(text);
            }
        }
    }
}

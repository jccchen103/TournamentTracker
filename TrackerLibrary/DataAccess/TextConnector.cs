using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess.TextHelpers;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        // text files for text-based database
        private const string PrizesFile = "PrizeModels.csv";
        //private const string PeopleFile = "PeopleModels.csv";
        //private const string TeamsFile = "TeamsModels.csv";
        //private const string TournamentsFile = "TournamentsModels.csv";
        //private const string MatchupFile = "MatchupModels.csv";
        //private const string MatchupEntriesFile = "MatchupEntriesModels.csv";


        public PrizeModel CreatePrize(PrizeModel model)
        {
            // load the text file (a list of string)
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
            // find the next id (max id + 1)
            int nextId = prizes.Count() > 0 ? prizes.OrderByDescending(x => x.Id).First().Id + 1 : 1;
            // add the new record with the next id
            model.Id = nextId;
            prizes.Add(model);
            // convert prizes to strings and save to the prizes text file
            prizes.SaveToPrizesFile(PrizesFile);

            return model;
        }
    }
}

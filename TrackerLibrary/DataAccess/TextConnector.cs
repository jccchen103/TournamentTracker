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
        private const string PeopleFile = "PersonModels.csv";
        //private const string TeamsFile = "TeamsModels.csv";
        //private const string TournamentsFile = "TournamentsModels.csv";
        //private const string MatchupFile = "MatchupModels.csv";
        //private const string MatchupEntriesFile = "MatchupEntriesModels.csv";


        public PersonModel CreatePerson(PersonModel model)
        {
            // read file with people data
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            // add the new person model with the next id number
            int nextId = people.Count() > 0 ? people.OrderByDescending(x => x.Id).First().Id + 1 : 1;
            model.Id = nextId;
            people.Add(model);

            // convert person models to strings and save to the people text file
            people.SaveToPeopleFile(PeopleFile);

            return model; 
        }

        public PrizeModel CreatePrize(PrizeModel model)
        {
            // load the text file and convert to prize models
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // add the new record with the next id (max id + 1)
            int nextId = prizes.Count() > 0 ? prizes.OrderByDescending(x => x.Id).First().Id + 1 : 1;
            model.Id = nextId;
            prizes.Add(model);

            // convert prizes to strings and save to the prizes text file
            prizes.SaveToPrizesFile(PrizesFile);

            return model;
        }

        public List<PersonModel> GetPeople()
        {
            return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }
    }
}

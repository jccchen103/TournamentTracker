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
        private const string TeamsFile = "TeamsModels.csv";
        //private const string TournamentsFile = "TournamentsModels.csv";
        //private const string MatchupFile = "MatchupModels.csv";
        //private const string MatchupEntriesFile = "MatchupEntriesModels.csv";


        public void CreatePerson(PersonModel model)
        {
            // read file with people data
            List<PersonModel> people = GetPeople();

            // set the id of model
            int nextId = people.Count() > 0 ? people.OrderByDescending(x => x.Id).First().Id + 1 : 1;
            model.Id = nextId;

            // add the model to the list of person models
            people.Add(model);

            // convert person models to strings and save to the people text file
            people.SaveToPeopleFile(PeopleFile); 
        }

        public void CreatePrize(PrizeModel model)
        {
            // load the text file and convert to prize models
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // add the new record with the next id (max id + 1)
            int nextId = prizes.Count() > 0 ? prizes.OrderByDescending(x => x.Id).First().Id + 1 : 1;
            model.Id = nextId;

            prizes.Add(model);

            // convert prizes to strings and save to the prizes text file
            prizes.SaveToPrizesFile(PrizesFile);
        }

        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
            int nextId = teams.Count() > 0 ? teams.OrderByDescending(x => x.Id).First().Id + 1 : 1;
            model.Id = nextId;

            teams.Add(model);
            teams.SaveToTeamsFile(TeamsFile);
        }

        public List<PersonModel> GetPeople()
        {
            return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeams()
        {
            return TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
        }
    }
}

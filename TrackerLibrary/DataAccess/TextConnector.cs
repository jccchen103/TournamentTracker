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
            people.SaveToPeopleFile(GlobalConfig.PeopleFile); 
        }

        public void CreatePrize(PrizeModel model)
        {
            // load the text file and convert to prize models
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // add the new record with the next id (max id + 1)
            int nextId = prizes.Count() > 0 ? prizes.OrderByDescending(x => x.Id).First().Id + 1 : 1;
            model.Id = nextId;

            prizes.Add(model);

            // convert prizes to strings and save to the prizes text file
            prizes.SaveToPrizesFile(GlobalConfig.PrizesFile);
        }

        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
            int nextId = teams.Count() > 0 ? teams.OrderByDescending(x => x.Id).First().Id + 1 : 1;
            model.Id = nextId;

            teams.Add(model);
            teams.SaveToTeamsFile(GlobalConfig.TeamsFile);
        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();
            int nextId = tournaments.Count() > 0 ? tournaments.OrderByDescending(x => x.Id).First().Id + 1 : 1;
            model.Id = nextId;  // set id for this tournament

            // save prizes for this tournament
            foreach (PrizeModel prize in model.Prizes)
            {
                prize.TournamentId = model.Id;
                CreatePrize(prize);
            }

            model.SaveToRoundsFile(GlobalConfig.MatchupsFile, GlobalConfig.MatchupEntriesFile);
            tournaments.Add(model);
            tournaments.SaveToTournamentsFile(GlobalConfig.TournamentsFile);
        }

        public List<PersonModel> GetPeople()
        {
            return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeams()
        {
            return GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
        }
    }
}

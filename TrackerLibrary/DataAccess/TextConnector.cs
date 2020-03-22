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
        public void CompleteTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GlobalConfig.TournamentsFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels();

            // delete the prizes for this tournament
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();
            foreach (PrizeModel prize in model.Prizes)
            {
                prizes.Remove(prize);
            }
            prizes.SaveToPrizesFile();
 
            // Delete the finished tournament entry from the tournaments file
            tournaments.Remove(model);
            tournaments.SaveToTournamentsFile();
        }

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
            people.SaveToPeopleFile(); 
        }

        public void CreatePrize(PrizeModel model)
        {
            List<PrizeModel> prizes = GlobalConfig.PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            int nextId = prizes.Count() > 0 ? prizes.OrderByDescending(x => x.Id).First().Id + 1 : 1;
            model.Id = nextId;

            prizes.Add(model);

            prizes.SaveToPrizesFile();
        }

        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = GetTeams();
            int nextId = teams.Count() > 0 ? teams.OrderByDescending(x => x.Id).First().Id + 1 : 1;
            model.Id = nextId;

            teams.Add(model);
            teams.SaveToTeamsFile();
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

            model.SaveRoundsToFile();
            tournaments.Add(model);
            tournaments.SaveToTournamentsFile();
        }

        public List<PersonModel> GetPeople()
        {
            return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }

        public List<TeamModel> GetTeams()
        {
            return GlobalConfig.TeamsFile.FullFilePath().LoadFile().ConvertToTeamModels();
        }

        public List<TournamentModel> GetTournaments()
        {
            return GlobalConfig.TournamentsFile.FullFilePath().LoadFile().ConvertToTournamentModels();
        }

        public void UpdateMatchup(MatchupModel model)
        {
            model.UpdateMatchupsFile();
        }
    }
}

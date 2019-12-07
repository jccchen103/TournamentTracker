using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    /// <summary>
    /// An interface for data sources.
    /// </summary>
    public interface IDataConnection
    {
        PrizeModel CreatePrize(PrizeModel model);
        //PersonModel CreatePerson(PersonModel model);
        //TeamModel CreateTeam(TeamModel model);
        //void CreateTournament(TournamentModel model);
        //List<TeamModel> GetTeams();
        //List<PersonModel> GetPeople();
    }
}

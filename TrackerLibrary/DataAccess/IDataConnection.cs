﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    /// <summary>
    /// An interface for data sources.
    /// </summary>
    public interface IDataConnection
    {
        void CreatePrize(PrizeModel model);
        void CreatePerson(PersonModel model);
        void CreateTeam(TeamModel model);
        //void CreateTournament(TournamentModel model);
        List<PersonModel> GetPeople();
        List<TeamModel> GetTeams();
    }
}

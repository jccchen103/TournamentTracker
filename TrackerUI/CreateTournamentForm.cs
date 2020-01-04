using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTournamentForm : Form
    {
        public CreateTournamentForm()
        {
            InitializeComponent();

            InitializeLists();
        }

        private void InitializeLists()
        {
            // add all teams in the database to the select team drop down
            List<TeamModel> teams = GlobalConfig.Connections.GetTeams();
            foreach (TeamModel t in teams)
            {
                selectTeamDropDown.Items.Add(t);
            }

            // just display the team name of a team model
            selectTeamDropDown.DisplayMember = "TeamName";
            tournamentPlayersListBox.DisplayMember = "TeamName";
        }

        private void CreateTournamentButton_Click(object sender, EventArgs e)
        {
            // TODO: wire up the create tournament button
            throw new NotImplementedException();
        }
    }
}

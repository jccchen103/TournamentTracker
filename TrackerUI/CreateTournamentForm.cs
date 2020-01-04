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

            // link select team drop down list
            List<TeamModel> teams = GlobalConfig.Connections.GetTeams();
            foreach (TeamModel t in teams)
            {
                selectTeamDropDown.Items.Add(t);
            }

            // just display the team name of a team model
            selectTeamDropDown.DisplayMember = "TeamName";
            tournamentPlayersListBox.DisplayMember = "TeamName";
        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {

        }
    }
}

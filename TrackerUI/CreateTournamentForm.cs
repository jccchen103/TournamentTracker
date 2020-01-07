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
    public partial class CreateTournamentForm : Form, IPrizeRequestor
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

            // just display the name of the models
            selectTeamDropDown.DisplayMember = "TeamName";
            tournamentPlayersListBox.DisplayMember = "TeamName";
            prizesListBox.DisplayMember = "PrizeDisplay";
        }

        private void CreateTournamentButton_Click(object sender, EventArgs e)
        {
            // TODO: wire up the create tournament button
            throw new NotImplementedException();
        }

        /// <summary>
        /// Move the selected team from the drop down to the tournament players list box.
        /// </summary>
        private void AddTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel selected = (TeamModel)selectTeamDropDown.SelectedItem;
            if (!(selected is null))
            {
                // move item selected in the drop down to the list box
                selectTeamDropDown.Items.Remove(selected);
                tournamentPlayersListBox.Items.Add(selected);
            }
            selectTeamDropDown.Text = "";
        }

        private void CreatePrizeButton_Click(object sender, EventArgs e)
        {
            // call the create prize form
            CreatePrizeForm form = new CreatePrizeForm(this);
            form.Show();
        }

        /// <summary>
        /// Add the created prize to the prizes list box.
        /// </summary>
        /// <param name="model">A prize model from the Create Prize form.</param>
        public void PrizeComplete(PrizeModel model)
        {
            prizesListBox.Items.Add(model);
        }
    }
}

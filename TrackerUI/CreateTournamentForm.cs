using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTournamentForm : Form, IPrizeRequestor, ITeamRequestor
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
            AddPrizesToDB();
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

        private void CreateTeamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm form = new CreateTeamForm(this);
            form.Show();
        }

        public void TeamComplete(TeamModel model)
        {
            tournamentPlayersListBox.Items.Add(model);
        }

        private void RemovePlayersButton_Click(object sender, EventArgs e)
        {
            var teamsToRemove = tournamentPlayersListBox.SelectedItems;

            while (teamsToRemove.Count > 0)
            {
                TeamModel t = (TeamModel)teamsToRemove[0];
                tournamentPlayersListBox.Items.Remove(t);
                selectTeamDropDown.Items.Add(t);
            }
        }

        private void RemovePrizesButton_Click(object sender, EventArgs e)
        {
            PrizeModel p = (PrizeModel)prizesListBox.SelectedItem;
            if (p != null)
            {
                prizesListBox.Items.Remove(p);
            }
        }

        /// <summary>
        /// Save the prizes in the prizes list box to the database.
        /// </summary>
        private void AddPrizesToDB()
        {
            foreach (PrizeModel model in prizesListBox.Items)
            {
                GlobalConfig.Connections.CreatePrize(model);
            }

        }
    }
}

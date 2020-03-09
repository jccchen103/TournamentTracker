using System;
using System.Collections.Generic;
using System.Linq;
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
            if (ValidateTournamentInfo())
            {
                TournamentModel tm = new TournamentModel
                {
                    TournamentName = tournamentNameValue.Text,
                    EntryFee = decimal.Parse(entryFeeValue.Text),
                    EnteredTeams = tournamentPlayersListBox.Items.Cast<TeamModel>().ToList(),
                    Prizes = prizesListBox.Items.Cast<PrizeModel>().ToList()
                };

                TournamentLogic.CreateRounds(tm);

                GlobalConfig.Connections.CreateTournament(tm);

                // Show the tournament that was just created
                TournamentViewerForm viewerForm = new TournamentViewerForm(tm);
                viewerForm.Show();
                
                //this.Close();
            }
            else
            {
                MessageBox.Show("You have invalid tournament information. Please check and try again.",
                        "Error: Invalid Tournament", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateTournamentInfo()
        {
            tournamentNameValue.Text = tournamentNameValue.Text.Trim();
            if (tournamentNameValue.Text.Length == 0) { 
                return false; // the tournament must have a name
            }
            if (tournamentPlayersListBox.Items.Count < 2) { 
                return false; // must have at least two tournament players
            }

            decimal entryFee;
            bool entryFeeValid = decimal.TryParse(entryFeeValue.Text, out entryFee);
            if (!entryFeeValid || entryFee < 0)
            {
                return false;   // entry fee is not a number or negative
            }
            
            return true;
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


        //private void AddPrizesToDB(int tournamentId)
        //{
        //    foreach (PrizeModel model in prizesListBox.Items)
        //    {
        //        model.TournamentId = tournamentId;
        //        GlobalConfig.Connections.CreatePrize(model);
        //    }
        //}
    }
}

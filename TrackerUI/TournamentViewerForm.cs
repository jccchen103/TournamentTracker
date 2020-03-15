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
    public partial class TournamentViewerForm : Form
    {
        private TournamentModel tm;
        BindingList<int> rounds = new BindingList<int>();
        BindingList<MatchupModel> selectedMatchups = new BindingList<MatchupModel>();

        public TournamentViewerForm(TournamentModel tournament)
        {
            InitializeComponent();
            tm = tournament;

            tournamentName.Text = tm.TournamentName;

            WireUpLists();
            LoadRounds();
        }

        private void WireUpLists()
        {
            roundDropDown.DataSource = rounds;

            matchupListBox.DataSource = selectedMatchups;
            matchupListBox.DisplayMember = "DisplayName";
        }

        private void LoadRounds()
        {
            foreach (List<MatchupModel> round in tm.Rounds)
            {
                rounds.Add(round.First().MatchupRound);
            }

            LoadMatchups(1);
        }

        private void LoadMatchups(int round)
        {
            foreach (List<MatchupModel> roundMatchups in tm.Rounds)
            {
                if (roundMatchups.First().MatchupRound == round)
                {
                    selectedMatchups.Clear();

                    foreach (MatchupModel matchup in roundMatchups)
                    {
                        if (matchup.Winner is null || !unplayedCheckBox.Checked)
                        {
                            selectedMatchups.Add(matchup);
                        }
                    }

                    break;
                }
            }

            if (selectedMatchups.Count > 0)
            {
                LoadMatchupInfo(selectedMatchups.First());
            }

            DisplayMatchupInfo(selectedMatchups.Count > 0);
        }

        private void DisplayMatchupInfo(bool isVisible)
        {
            teamOneName.Visible = isVisible;
            scoreOneLabel.Visible = isVisible;
            scoreOneValue.Visible = isVisible;

            teamTwoName.Visible = isVisible;
            scoreTwoLabel.Visible = isVisible;
            scoreTwoValue.Visible = isVisible;

            versusLabel.Visible = isVisible;
            scoreButton.Visible = isVisible;
        }

        private void LoadMatchupInfo(MatchupModel m)
        {
            if (m is null)
            {
                return;
            }

            for (int i = 0; i < m.Entries.Count; i++)
            {
                MatchupEntryModel entry = m.Entries[i];

                if (i == 0)
                {
                    if (entry.TeamCompeting is null)
                    {
                        teamOneName.Text = "Not yet set";
                        scoreOneValue.Text = "";
                    }
                    else
                    {
                        teamOneName.Text = entry.TeamCompeting.TeamName;
                        scoreOneValue.Text = entry.Score.ToString();

                        teamTwoName.Text = "<None>";
                        scoreTwoValue.Text = "";
                    }
                }

                if (i == 1)
                {
                    if (entry.TeamCompeting is null)
                    {
                        teamTwoName.Text = "Not yet set";
                        scoreTwoValue.Text = "";
                    }
                    else
                    {
                        teamTwoName.Text = entry.TeamCompeting.TeamName;
                        scoreTwoValue.Text = entry.Score.ToString();
                    }
                }
            }

            // Disable the score button for finished rounds
            scoreButton.Enabled = (m.MatchupRound >= TournamentLogic.GetCurrentRound(tm));
        }

        private void RoundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private void MatchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchupInfo((MatchupModel)matchupListBox.SelectedItem);
        }

        private void UnplayedCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private void ScoreButton_Click(object sender, EventArgs e)
        {
            string errorMsg = ValidateScores();
            if (errorMsg.Length > 0)
            {
                MessageBox.Show(errorMsg, "Error: Invalid Score");
                return;
            }

            MatchupModel m = (MatchupModel)matchupListBox.SelectedItem;

            if (m.Entries[0].TeamCompeting != null)
            {
                m.Entries[0].Score = double.Parse(scoreOneValue.Text);
            }
            if (m.Entries.Count > 1 && m.Entries[1].TeamCompeting != null)
            {
                m.Entries[1].Score = double.Parse(scoreTwoValue.Text);
            }

            try
            {
                TournamentLogic.UpdateTournamentResults(tm, m);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ ex.Message }");
                return;
            }

            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private string ValidateScores()
        {
            string output = "";

            // check if the first score is valid
            double teamOneScore;
            bool scoreOneValid = double.TryParse(scoreOneValue.Text, out teamOneScore);
            if (!scoreOneValid)
            {
                output = $"The score for {teamOneName.Text} is invalid.";
            }

            if (((MatchupModel)matchupListBox.SelectedItem).Entries.Count > 1)
            {
                // check second score
                double teamTwoScore;
                bool scoreTwoValid = double.TryParse(scoreTwoValue.Text, out teamTwoScore);
                if (!scoreTwoValid)
                {
                    output = $"The score for {teamTwoName.Text} is invalid.";
                }

                // check if both scores are 0
                else if (teamOneScore == 0 && teamTwoScore == 0)
                {
                    output = "The scores cannot be both 0. Please change the scores.";
                }

                // check if both scores are equal (ties are not allowed)
                else if (teamOneScore == teamTwoScore)
                {
                    output = "Ties are not allowed. Please change the scores.";
                }

            }
            return output;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            rounds.Clear();

            rounds.Add(1);
            int currRound = 1;

            foreach (List<MatchupModel> round in tm.Rounds)
            {
                if (round.First().MatchupRound > currRound)
                {
                    currRound = round.First().MatchupRound;
                    rounds.Add(currRound);
                }
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
                LoadMatchup(selectedMatchups.First());
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

        private void LoadMatchup(MatchupModel m)
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

                        teamTwoName.Text = "<none>";
                        scoreTwoValue.Text = "0";
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
        }

        private void RoundDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchups((int)roundDropDown.SelectedItem);
        }

        private void MatchupListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMatchup((MatchupModel)matchupListBox.SelectedItem);
        }
    }
}

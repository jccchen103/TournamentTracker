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
    public partial class TournamentDashboardForm : Form
    {
        public TournamentDashboardForm()
        {
            InitializeComponent();

            InitializeList();
        }

        private void InitializeList()
        {
            List<TournamentModel> tournaments = GlobalConfig.Connections.GetTournaments();
            loadTournamentDropDown.DataSource = tournaments;
            loadTournamentDropDown.DisplayMember = "TournamentName";
        }

        private void CreateTournamentButton_Click(object sender, EventArgs e)
        {
            CreateTournamentForm form = new CreateTournamentForm();
            form.Show();
        }

        private void LoadTournamentButton_Click(object sender, EventArgs e)
        {
            TournamentModel tm = (TournamentModel)loadTournamentDropDown.SelectedItem;
            TournamentViewerForm tvf = new TournamentViewerForm(tm);
            tvf.Show();
        }
    }
}

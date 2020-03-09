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

namespace TrackerUI
{
    public partial class TournamentDashboardForm : Form
    {
        public TournamentDashboardForm()
        {
            InitializeComponent();
            // TODO: Wire up the tournament drop-down
            
        }

        private void CreateTournamentButton_Click(object sender, EventArgs e)
        {
            CreateTournamentForm form = new CreateTournamentForm();
            form.Show();
        }


        // TODO: Wire up the Load Tournament button
    }
}

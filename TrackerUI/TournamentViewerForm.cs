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
        public TournamentViewerForm(TournamentModel tournament)
        {
            InitializeComponent();
            tm = tournament;

            // show the name of the tournament
            tournamentName.Text = tm.TournamentName;

            //WireUpLists();
        }

        private void WireUpLists()
        {
            // wire up the round drop down list

            // have the matchup list box show the competing entries of a matchup
            throw new NotImplementedException();
        }
    }
}

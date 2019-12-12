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
    public partial class CreateTeamForm : Form
    {
        private List<PersonModel> availableMembers = GlobalConfig.Connections.GetPeople();
        private List<PersonModel> listBoxMembers = new List<PersonModel>();

        public CreateTeamForm()
        {
            InitializeComponent();
            WireUpLists();
            selectTeamMemberDropDown.SelectedItem = null;
        }

        // TODO: Implement functionality on clicking the Add Member button
        private void AddMemberButton_Click(object sender, EventArgs e)
        {
            // Move selected person from the drop down to the list box
            PersonModel selected = (PersonModel)selectTeamMemberDropDown.SelectedItem;
            if (!(selected is null))
            {
                listBoxMembers.Add(selected);
                availableMembers.Remove(selected);

                WireUpLists();
            }
        }

        private void WireUpLists()
        {
            // Set available members for drop down
            selectTeamMemberDropDown.DataSource = null;
            selectTeamMemberDropDown.DataSource = availableMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName";

            // Set list box members
            teamMembersListBox.DataSource = null;
            teamMembersListBox.DataSource = listBoxMembers;
            teamMembersListBox.DisplayMember = "FullName";

        }
    }
}

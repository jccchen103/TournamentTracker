﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {
        private List<PersonModel> availableMembers = GlobalConfig.Connections.GetPeople();
        private List<PersonModel> selectedTeam = new List<PersonModel>();

        public CreateTeamForm()
        {
            InitializeComponent();
            WireUpLists();
        }

        private void AddMemberButton_Click(object sender, EventArgs e)
        {
            // Move selected person from the drop down to the list box
            PersonModel selected = (PersonModel)selectTeamMemberDropDown.SelectedItem;
            if (!(selected is null))
            {
                selectedTeam.Add(selected);
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
            selectTeamMemberDropDown.SelectedItem = null;

            // Set list box members
            teamMembersListBox.DataSource = null;
            teamMembersListBox.DataSource = selectedTeam;
            teamMembersListBox.DisplayMember = "FullName";

        }

        private void CreateMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidInputs(out int errCode))
            {
                // create person model from input data and add to db
                PersonModel p = new PersonModel
                {
                    FirstName = firstNameValue.Text,
                    LastName = lastNameValue.Text,
                    Email = emailValue.Text
                };
                GlobalConfig.Connections.CreatePerson(p);

                // add the new person to the team and refresh the list box
                selectedTeam.Add(p);
                WireUpLists();

                // clear input fields
                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
            }
            else if (errCode == 1)
            {
                MessageBox.Show("You have missing new member information. Please fill in all fields.", "Error: Empty Field");
            }
            else if (errCode == 2)
            {
                MessageBox.Show("This email address is invalid. Please check it and try again.", "Error: Invalid Email");
            }
        }

        /// <summary>
        /// Validate user inputs for a new person.
        /// </summary>
        /// <param name="errorCode">
        /// Integer that represents the type of error, if there is one:
        ///     0 = No error,
        ///     1 = There is an empty field,
        ///     2 = The format of the email address is invalid.
        /// </param>
        /// <returns>Whether the inputs were valid.</returns>
        private bool ValidInputs(out int errorCode)
        {
            firstNameValue.Text = firstNameValue.Text.Trim();
            lastNameValue.Text = lastNameValue.Text.Trim();

            if (firstNameValue.Text.Length == 0 || 
                lastNameValue.Text.Length == 0 || emailValue.Text.Length == 0) {
                errorCode = 1;
                return false;   // no empty fields allowed
            }

            // check if email address format is valid (from Microsoft docs)
            bool isEmail = Regex.IsMatch(emailValue.Text,
                        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                        RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            if (!isEmail)
            {
                errorCode = 2;
                return false;
            }

            errorCode = 0;
            return true;
        }

        /// <summary>
        /// Move the people selected in the list box to the drop-down menu
        /// </summary>
        private void DeleteMembersButton_Click(object sender, EventArgs e)
        {
            var peopleToRemove = teamMembersListBox.SelectedItems;
            foreach (PersonModel p in peopleToRemove)
            {
                selectedTeam.Remove(p);
                availableMembers.Add(p);
            }

            WireUpLists();
        }

        private void CreateTeamButton_Click(object sender, EventArgs e)
        {
            // validate: team name, selectedTeam.size > 0

            // TeamModel t = new TeamModel(name = teamNameValue.Text, members = selectedTeam)
            // CreateTeam(t); -- add team to db, set id, and link t.Id with p.Id

            // pass team t to the caller form

            // close this form
        }
    }
}

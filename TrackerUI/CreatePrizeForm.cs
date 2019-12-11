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
using TrackerLibrary.DataAccess;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreatePrizeForm : Form
    {
        public CreatePrizeForm()
        {
            InitializeComponent();
        }

        private void CreatePrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidateFields())
            {
                // create prize model
                PrizeModel model = new PrizeModel(
                    placeNumberValue.Text,
                    placeNameValue.Text,
                    prizeAmountValue.Text,
                    prizePercentageValue.Text);

                // add prize model to the data source
                GlobalConfig.Connections.CreatePrize(model);

                // clear input fields to default values
                placeNumberValue.Text = "";
                placeNameValue.Text = "";
                prizeAmountValue.Text = "0";
                prizePercentageValue.Text = "0";
            }
            else
            {
                MessageBox.Show("This form has invalid information. Please check it and try again.");
            }
        }

        /// <summary>
        /// Validate user inputs on a Create Prize form.
        /// </summary>
        private bool ValidateFields()
        {
            bool output = true;
            int placeNumber;

            if (!int.TryParse(placeNumberValue.Text, out placeNumber) || placeNumber < 1)
            {
                 output = false; // place number input is not a valid positive integer
            }

            if (placeNameValue.Text.Length == 0)
            {
                output = false; // place name input is empty
            }

            decimal prizeAmount;
            double prizePercentage;
            bool prizeAmountValid = decimal.TryParse(prizeAmountValue.Text, out prizeAmount);
            bool prizePercentageValid = double.TryParse(prizePercentageValue.Text, out prizePercentage);
            
            if (!prizeAmountValid || !prizePercentageValid)
            {
                output = false;   // prize amount or prize percentage input is invalid
            }

            if (!(prizeAmount == 0 && prizePercentage > 0) && !(prizeAmount > 0 && prizePercentage == 0))
            {
                output = false;   // one should be positive while the other is 0
            }

            if (prizePercentage > 100)
            {
                output = false;   // percentage should not be over 100
            }
            
            return output;
        }

    }
}

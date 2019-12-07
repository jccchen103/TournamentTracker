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
    public partial class CreatePrizeForm : Form
    {
        public CreatePrizeForm()
        {
            InitializeComponent();
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidateFields())
            {
                // create prize
                PrizeModel model = new PrizeModel(placeNameValue.Text, placeNameValue.Text, prizeAmountValue.Text, prizePercentageValue.Text);
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

            // interpret an empty prize amount or prize percentage as 0
            if (prizeAmountValue.Text.Length == 0) { prizeAmountValue.Text = "0"; }
            if (prizePercentageValue.Text.Length == 0) { prizePercentageValue.Text = "0"; }

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

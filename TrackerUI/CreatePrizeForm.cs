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
        private IPrizeRequestor callingForm;

        public CreatePrizeForm(IPrizeRequestor caller)
        {
            InitializeComponent();
            callingForm = caller;
        }

        /// <summary>
        /// If all inputs are valid, create the specified prize model and 
        /// send it to the calling form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreatePrizeButton_Click(object sender, EventArgs e)
        {
            if (ValidInputs())
            {
                // create prize model
                PrizeModel model = new PrizeModel(
                    placeNumberValue.Text,
                    placeNameValue.Text,
                    prizeAmountValue.Text,
                    prizePercentageValue.Text);

                // return model to caller and close this prize form
                callingForm.PrizeComplete(model);
                this.Close();
            }
            else
            {
                MessageBox.Show("This form has invalid information. Please check it and try again.");
            }
        }

        /// <summary>
        /// Validate user inputs on a Create Prize form.
        /// </summary>
        /// <returns>Whether the inputs were valid.</returns>
        private bool ValidInputs()
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

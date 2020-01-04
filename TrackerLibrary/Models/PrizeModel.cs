using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one prize rewarded in a tournament.
    /// </summary>
    public class PrizeModel
    {
        /// <summary>
        /// The unique identifier for the prize.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The place number of the team that gets this prize.
        /// </summary>
        public int PlaceNumber { get; set; }

        /// <summary>
        /// The place name of the team that gets this prize.
        /// </summary>
        public string PlaceName { get; set; }

        /// <summary>
        /// The monetary amount of this prize. 
        /// </summary>
        public decimal PrizeAmount { get; set; }

        /// <summary>
        /// The percentage of the total income from this tournament that
        /// is given as this prize.
        /// </summary>
        public double PrizePercentage { get; set; }

        public PrizeModel() { }

        public PrizeModel(string placeNumber, string placeName, string prizeAmount, string prizePercentage)
        {
            PlaceName = placeName;

            int.TryParse(placeNumber, out int placeNumberValue);
            PlaceNumber = placeNumberValue;

            decimal.TryParse(prizeAmount, out decimal prizeAmountValue);
            PrizeAmount = prizeAmountValue;

            double.TryParse(prizePercentage, out double prizePercentageValue);
            PrizePercentage = prizePercentageValue;
        }

        public string PrizeDisplay
        {
            get {
                string display = $"{PlaceName}: ";
                display += PrizeAmount == 0 ? $"{PrizeAmount}" : $"{PrizePercentage} %";
                return display; 
            }
        }
    }
}

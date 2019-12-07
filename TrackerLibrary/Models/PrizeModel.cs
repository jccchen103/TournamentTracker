using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
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
        public int PrizePercentage { get; set; }

        public PrizeModel() { }

        public PrizeModel(int placeNumber, string placeName, decimal prizeAmount, int prizePercentage)
        {
            PlaceNumber = placeNumber;
            PlaceName = placeName;
            PrizeAmount = prizeAmount;
            PrizePercentage = prizePercentage;
        }
    }
}

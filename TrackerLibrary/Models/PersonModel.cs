using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one person.
    /// </summary>
    public class PersonModel
    {
        /// <summary>
        /// The unique identifier for the person.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The first name of this person.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of this person.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The email this person registered with.
        /// </summary>
        public string EmailAddress { get; set; }
    }
}

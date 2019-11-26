﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents one person.
    /// </summary>
    class PersonModel
    {
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

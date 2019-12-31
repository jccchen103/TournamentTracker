using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerUI
{
    /// <summary>
    /// An interface for callers of the CreateTeam form.
    /// </summary>
    interface ITeamRequestor
    {
        void ReceiveTeamModel();
    }
}

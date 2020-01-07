using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerUI
{
    /// <summary>
    /// An interface for callers of the CreatePrize form.
    /// </summary>
    public interface IPrizeRequestor
    {
        void PrizeComplete(PrizeModel model);
    }
}

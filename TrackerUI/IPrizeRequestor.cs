using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerUI
{
    /// <summary>
    /// An interface for callers of the CreatePrize form.
    /// </summary>
    interface IPrizeRequestor
    {
        void ReceivePrizeModel();
    }
}

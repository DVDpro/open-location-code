using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOL.OpenLocationCode
{
    /// <summary>
    /// Possible Open location code format version
    /// </summary>
    public enum FormatVersion
    {
        /// <summary>
        /// Representation of open location code version 1.0 (released 4 Nov 2014).  http://openlocationcode.com
        /// </summary>
        V1 = 0,

        /// <summary>
        /// Representation of open location code latest development version.  http://openlocationcode.com
        /// </summary>
        VNext = 1,
    }

}

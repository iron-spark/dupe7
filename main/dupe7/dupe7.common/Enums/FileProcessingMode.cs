using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace dupe7.common.Enums
{
    public enum FileProcessingMode
    {
        /// <summary>
        /// By default nothing will happen - must be explicit
        /// </summary>
        None,
        /// <summary>
        /// Calculate a hash for each file - faster than full but not 100% accurate
        /// </summary>
        //[EnumMember(Value = "hash")]
        Hash,

        /// <summary>
        /// Just check name of file (incl. ext)- ignores contents
        /// </summary>
        //[EnumMember(Value = "name")]
        Name,

        /// <summary>
        /// Compares data of each file against each file - slower but 100% accurate.
        /// </summary>
       // [EnumMember(Value = "full")]
        Full
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LirisLibCorrBan.Common
{
    public static class Enum
    {
        public enum LogTypes : int
        {
            [System.Runtime.Serialization.EnumMemberAttribute()]
            Debug,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Info,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Warn,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Error,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Fatal,
        }
    }
}

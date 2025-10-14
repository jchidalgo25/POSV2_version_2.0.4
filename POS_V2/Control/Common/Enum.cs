using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Control.Common
{
    public static class Enum
    {
        public enum CatalogHeaders : int
        {
            [System.Runtime.Serialization.EnumMemberAttribute()]
            TblConfiguraEmp,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            TblTipoCorrBan,
        }

        public enum CatalogDetails : int
        {
            [System.Runtime.Serialization.EnumMemberAttribute()]
            EmpresaRecaudadora,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            EmpresaRecarga,
        }
        public enum LogTypes : int
        {
            [System.Runtime.Serialization.EnumMemberAttribute()]
            Debug,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Info,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Warn,
            [System.Runtime.Serialization.EnumMemberAttribute()]
            Warning,
            [System.Runtime.Serialization.EnumMemberAttribute()]
            Error,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Fatal,
        }

        public enum GiftcardSaleType : int
        {
            [System.Runtime.Serialization.EnumMemberAttribute()]
            Sale,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Recharge,
        }

        public enum TimeAdditionType : int
        {
            [System.Runtime.Serialization.EnumMemberAttribute()]
            Month,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Day,
        }

        public enum ParkingItemType : int
        {
            [System.Runtime.Serialization.EnumMemberAttribute()]
            ParqueoConCompra,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            Parqueo,

            [System.Runtime.Serialization.EnumMemberAttribute()]
            PerdidaTicket,
        }

        public enum NumAutorizador: int
        {
            [System.Runtime.Serialization.EnumMemberAttribute()]
            MediaNet = 1,


            [System.Runtime.Serialization.EnumMemberAttribute()]
            DataFast = 2,


            [System.Runtime.Serialization.EnumMemberAttribute()]
            Austro = 3
        }

    }
}

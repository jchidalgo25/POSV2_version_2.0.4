using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models.CorrBan
{
    public class CustAccount
    {
        public string AccountCode { get; set; }
        public string CustName { get; set; }
        public string CustIdentification { get; set; }
        public string CustAddress { get; set; }
        public Decimal ValuePaid { get; set; }
        public Decimal ValueEfective { get; set; }
        public Decimal ValueTotal { get; set; }
        public Decimal ValueMin { get; set; }
        public Decimal ValueActivaCharge { get; set; }
        public Decimal ValuePaySuggested { get; set; }
        public Decimal ValueDisability { get; set; }
        public Decimal ValueTax { get; set; }
        public Decimal ValueAditional { get; set; }
        public Decimal ValueOthers { get; set; }
        public string SequSara { get; set; }
        public string SequCarrier { get; set; }
        #region Support Properties
        public Decimal ValueMinTotal { get; set; }
        public Decimal ValueMaxTotal { get; set; }

        #endregion
    }
}

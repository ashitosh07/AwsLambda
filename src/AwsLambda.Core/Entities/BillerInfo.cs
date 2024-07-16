using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsLambda.Core.Entities
{
    public class BillerInfo
    {
        public int Id { get; set; }
        public string BillerInfoId { get; set; }
        public string PartnerId { get; set; }
        public string BillerId { get; set; }
        public string MerchantId { get; set; }
        public BillChoice BillChoice { get; set; }
        public string BillerName { get; set; }
        public bool IsExtraData { get; set; }
        public bool IsCompliance { get; set; }
        public int MaxStubs { get; set; }
        public List<string> PaymentTypes { get; set; }
        public DateTime CreatedDatetime { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class BillChoice
    {
        public string Option { get; set; }
    }

}

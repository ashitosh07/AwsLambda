using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsLambda.Core.Entities
{
    public class ProcessBillPay
    {
        public string TerminalId { get; set; }
        public string BillerId { get; set; }
        public string TransactionId { get; set; }
        public bool IsTransactionSummary { get; set; }
        public ScreenData ScreenData { get; set; }
    }

    public class ScreenData
    {
        public DataElement[] DataElements { get; set; }
    }

    public class DataElement
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
}

namespace AwsLambda.Application.Contracts.Dtos
{
    public class ProcessBillPayDto
    {
        public string TerminalId { get; set; }
        public string UserId { get; set; }
        public string BillerId { get; set; }
        public string TransactionId { get; set; }
        public bool IsTransactionSummary { get; set; }
        public ScreenDataDto ScreenData { get; set; }
    }

    public class ScreenDataDto
    {
        public DataElementDto[] DataElements { get; set; }
    }

    public class DataElementDto
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }
}

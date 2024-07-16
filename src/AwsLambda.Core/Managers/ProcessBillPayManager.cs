using AwsLambda.Core.Entities;
using AwsLambda.Core.RepositoryInterfaces;

namespace AwsLambda.Core.Managers
{
    public class ProcessBillPayManager
    {
        private readonly IProcessBillPayRepository processBillPayRepository;

        public ProcessBillPayManager(IProcessBillPayRepository processBillPayRepository)
        {
            this.processBillPayRepository = processBillPayRepository;
        }

        public async Task AddAsync(ProcessBillPay processBillPay)
        {
            // Include business logic here, if any
            await processBillPayRepository.AddAsync(processBillPay);
        }
    }
}

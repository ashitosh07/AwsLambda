using AwsLambda.Core.Entities;
using System.Threading.Tasks;

namespace AwsLambda.Core.RepositoryInterfaces
{
    public interface IProcessBillPayRepository
    {
        Task AddAsync(ProcessBillPay entity);
        // Define other methods as needed, e.g.:
        // Task<ProcessBillPay> GetByIdAsync(int id);
        // Task<IEnumerable<ProcessBillPay>> GetAllAsync();
    }
}

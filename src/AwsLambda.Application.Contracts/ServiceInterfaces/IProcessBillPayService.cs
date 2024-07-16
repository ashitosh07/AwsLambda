using System.Threading.Tasks;
using AwsLambda.Application.Contracts.Dtos;
using AwsLambda.Core.Entities;

namespace AwsLambda.Application.Contracts.ServiceInterfaces
{
    public interface IProcessBillPayService
    {
        Task<ProcessBillPayDto> CreateProcessBillPayAsync(ProcessBillPayDto input);
    }
}

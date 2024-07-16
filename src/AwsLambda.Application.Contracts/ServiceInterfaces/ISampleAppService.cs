using AwsLambda.Application.Contracts.Dtos;

namespace AwsLambda.Application.Contracts.ServiceInterfaces;

public interface ISampleAppService
{
    Task<SampleDto> CreateSampleAsync(CreateUpdateSampleDto input);

    Task<SampleDto> GetSampleAsync(int id);
}

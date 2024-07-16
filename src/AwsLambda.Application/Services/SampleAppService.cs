using AutoMapper;
using AwsLambda.Application.Contracts.Dtos;
using AwsLambda.Application.Contracts.ServiceInterfaces;
using AwsLambda.Core.Entities;
using AwsLambda.Core.Managers;
using AwsLambda.Core.RepositoryInterfaces;

namespace AwsLambda.Application.Services;

public class SampleAppService : ISampleAppService
{
    private readonly ISampleRepository sampleRepository;
    private readonly IMapper mapper;
    private readonly SampleManager sampleManager;
    public SampleAppService(
        ISampleRepository sampleRepository,
        IMapper mapper,
        SampleManager sampleManager)
    {
        this.sampleRepository = sampleRepository;
        this.mapper = mapper;
        this.sampleManager = sampleManager;
    }

    public async Task<SampleDto> CreateSampleAsync(CreateUpdateSampleDto input)
    {
        var sample = await sampleManager.CreateAsync(input.Name, input.Age, input.Email);
        await sampleRepository.AddAsync(sample);
        return mapper.Map<Sample, SampleDto>(sample);
    }

    public async Task<SampleDto> GetSampleAsync(int id)
    {
        var sample = await sampleRepository.GetByIdAsync(id);
        return mapper.Map<Sample, SampleDto>(sample);
    }
}

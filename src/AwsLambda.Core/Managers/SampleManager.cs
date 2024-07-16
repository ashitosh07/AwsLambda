using AwsLambda.Core.Entities;
using AwsLambda.Core.Exceptions;
using AwsLambda.Core.RepositoryInterfaces;

namespace AwsLambda.Core.Managers;

public class SampleManager
{
    private readonly ISampleRepository sampleRepository;
    public SampleManager(ISampleRepository sampleRepository)
    {
        this.sampleRepository = sampleRepository;
    }

    public async Task<Sample> CreateAsync(
        string name,
        int age,
        string email)
    {
        var exists = await sampleRepository.GetByNameAsync(name);
        if (exists != null)
        {
            throw new AlreadyExistException("Sample already exists");
        }
        return new Sample(name, age, email);
    }
}

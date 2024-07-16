using AwsLambda.Core.RepositoryInterfaces;

namespace AwsLambda.Infrastructure.Repository;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(ISampleRepository sampleRepository)
    {
        Sample = sampleRepository;
    }
    public ISampleRepository Sample { get; }
}

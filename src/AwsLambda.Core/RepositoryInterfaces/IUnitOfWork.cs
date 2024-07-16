namespace AwsLambda.Core.RepositoryInterfaces;

public interface IUnitOfWork
{
    ISampleRepository Sample { get; }
}

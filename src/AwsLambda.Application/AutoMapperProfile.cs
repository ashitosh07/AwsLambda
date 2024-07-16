using AutoMapper;
using AwsLambda.Application.Contracts.Dtos;
using AwsLambda.Core.Entities;

namespace AwsLambda.Application;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Sample, SampleDto>().ReverseMap();
    }
}

using AwsLambda.Core.Dtos;
using AwsLambda.Core.Entities;

namespace AwsLambda.Lambda.ApiGw;

public static class Mapper
{
    public static SampleResponseDto GenerateSampleResponseDto(Sample sample)
    {
        SampleResponseDto sampleResponseDto = new();

        if (sample is not null)
        {
            sampleResponseDto.Success = true;
            sampleResponseDto.Message = "Record Found";
            sampleResponseDto.Sample = sample;
        }

        return sampleResponseDto;
    }
}

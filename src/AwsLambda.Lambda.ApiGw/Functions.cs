using Amazon.Lambda.Core;
using AwsLambda.Application.Interfaces;
using AwsLambda.Core.Dtos;
using AwsLambda.Core.Entities;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AwsLambda.Lambda.ApiGw;

public class Functions
{
    private readonly ISampleRepository sampleRepository;
    public Functions(ISampleRepository sampleRepository)
    {
        this.sampleRepository = sampleRepository;
    }

    public async Task<SampleResponseDto> GetSampleAsyncHandler(string id, ILambdaContext context)
    {
        SampleResponseDto sampleResponseDto = new();

        if (string.IsNullOrEmpty(id))
        {
            context.Logger.LogDebug($"Did NOT receive the valid request for Employee.");

            sampleResponseDto.Message = "Received Invalid Employee Id";

            return sampleResponseDto;
        }

        context.Logger.LogDebug($"Received the request with Employee Id {id}.");
        Sample sample = await sampleRepository.GetByIdAsync(int.Parse(id));

        context.Logger.LogDebug($"Sending the response for Employee Id {id}.");
        sampleResponseDto = Mapper.GenerateSampleResponseDto(sample);
        return sampleResponseDto;
    }
}

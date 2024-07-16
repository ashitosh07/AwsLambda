using AutoMapper;
using AwsLambda.Application.Contracts.Dtos;
using AwsLambda.Application.Contracts.ServiceInterfaces;
using AwsLambda.Core.Entities;
using AwsLambda.Core.Managers;
using AwsLambda.Core.RepositoryInterfaces;

namespace AwsLambda.Application.Services
{
    internal class ProcessBillPayService : IProcessBillPayService
    {
        private readonly ISampleRepository sampleRepository;
        private readonly IMapper mapper;
        private readonly ProcessBillPayManager processBillPayManager;

        public ProcessBillPayService(ISampleRepository sampleRepository, IMapper mapper, ProcessBillPayManager processBillPayManager)
        {
            this.sampleRepository = sampleRepository;
            this.mapper = mapper;
            this.processBillPayManager = processBillPayManager;
        }

        public async Task<ProcessBillPayDto> CreateProcessBillPayAsync(ProcessBillPayDto input)
        {
            // Map input DTO to entity
            var processBillPay = new ProcessBillPay
            {
                TerminalId = input.TerminalId,
                BillerId = input.BillerId,
                TransactionId = input.TransactionId,
                IsTransactionSummary = input.IsTransactionSummary,
                ScreenData = mapper.Map<ScreenDataDto, ScreenData>(input.ScreenData) // Assuming ScreenData is another DTO
            };

            // Use the manager to handle the logic
            await processBillPayManager.AddAsync(processBillPay);

            // Map the created entity back to DTO
            return mapper.Map<ProcessBillPay, ProcessBillPayDto>(processBillPay);
        }


    }
}

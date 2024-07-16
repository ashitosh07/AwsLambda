using Amazon.Lambda;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;
using AwsLambda.Application.Contracts.Dtos;
using AwsLambda.Core.Entities;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System;
using MySql.Data.MySqlClient;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AwsLambda.Lambda
{
    public class ProcessBillPayFunction
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly IAmazonLambda lambdaClient = new AmazonLambdaClient();
        private readonly string connectionString = "your-connection-string";
        private static readonly ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("your-redis-connection-string");

        [LambdaFunction]
        public async Task<Dictionary<string, object>> ProcessBillPay(ProcessBillPayDto input, ILambdaContext context)
        {
            // Step 1: Fetch biller information
            var billerInfo = await GetBillerInfoAsync(input.BillerId, context);
            if (billerInfo == null)
            {
                context.Logger.LogError($"No biller found for ID: {input.BillerId}");
                throw new Exception("Biller not found.");
            }

            // Initialize response and cache key
            Dictionary<string, object> response = null;
            string cacheKey = $"biller:{input.BillerId}";

            // Validate input data
            input = ValidateInputData(input);

            // Primary Bill Details Flow
            if (!billerInfo.IsExtraData && !billerInfo.IsCompliance)
            {
                response = await RequestPrimaryBillDetails(input, context);
                
                await ValidatePrimaryBillDetails(response, input, context);
            }
            // Extra Fields Flow
            else if (billerInfo.IsExtraData)
            {
                response = await HandleExtraFieldsFlow(input, billerInfo, context, cacheKey);
            }
            // Compliance Flow
            else if (billerInfo.IsCompliance)
            {
                response = await HandleComplianceFlow(input, billerInfo, context, cacheKey);
            }

            // Step 5: Check user limit
            var userDailyLimit = await GetUserDailyLimit(input.UserId, context);
            if (response != null && response.ContainsKey("Amount"))
            {
                if (userDailyLimit.DailyRemainCnt <= 0 || userDailyLimit.DailyRemainAmount < (decimal)response["Amount"])
                {
                    throw new Exception("User limit exceeded.");
                }
            }

            // Step 6: Invoke ValidateBillPay Lambda
            var validateResponse = await InvokeLambdaAsync("ValidateBillPay", response, input, context);

            // Step 7: Invoke ProcessTransaction Lambda
            var transactionResponse = await InvokeLambdaAsync("ProcessTransaction", validateResponse, input, context);

            // Step 8: Update user daily limit
            UpdateUserDailyLimit(input.UserId, (decimal)response["Amount"], context);

            // Step 9: Store transaction data in Aurora DB
            await StoreTransactionData(input, response, transactionResponse, context);

            return transactionResponse;
        }

        private async Task<Dictionary<string, object>> HandleExtraFieldsFlow(ProcessBillPayDto input, BillerInfo billerInfo, ILambdaContext context, string cacheKey)
        {
            var cachedExtraFieldsResponse = await GetCachedResponse(cacheKey);
            if (cachedExtraFieldsResponse == null)
            {
                cachedExtraFieldsResponse = await RequestExtraFields(input, context);
                await CacheResponse(cacheKey, cachedExtraFieldsResponse, TimeSpan.FromDays(1));
            }

            await ValidateExtraFields(cachedExtraFieldsResponse, input, context);
            var primaryBillDetailsResponse = await RequestPrimaryBillDetails(input, context);
            await ValidatePrimaryBillDetails(primaryBillDetailsResponse, input, context);

            return primaryBillDetailsResponse;
        }

        private async Task<Dictionary<string, object>> HandleComplianceFlow(ProcessBillPayDto input, BillerInfo billerInfo, ILambdaContext context, string cacheKey)
        {
            var cachedExtraFieldsResponse = await GetCachedResponse(cacheKey);
            if (cachedExtraFieldsResponse == null)
            {
                cachedExtraFieldsResponse = await RequestExtraFields(input, context);
                await CacheResponse(cacheKey, cachedExtraFieldsResponse, TimeSpan.FromDays(1));
            }

            await ValidateExtraFields(cachedExtraFieldsResponse, input, context);
            var primaryBillDetailsResponse = await RequestPrimaryBillDetails(input, context);
            await ValidatePrimaryBillDetails(primaryBillDetailsResponse, input, context);

            var complianceResponse = await RequestComplianceInformation(input, context);
            await ValidateComplianceInformation(complianceResponse, input, context);

            return primaryBillDetailsResponse;
        }

        private async Task ValidatePrimaryBillDetails(Dictionary<string, object> response, ProcessBillPayDto input, ILambdaContext context)
        {
            // Validate the primary bill details
            var validatedResponse = await InvokeLambdaAsync("ValidateBillPay", response, input, context);
            if (validatedResponse == null || !validatedResponse.ContainsKey("Valid") || !(bool)validatedResponse["Valid"])
            {
                throw new Exception("Primary bill details validation failed.");
            }
        }

        private async Task ValidateExtraFields(Dictionary<string, object> response, ProcessBillPayDto input, ILambdaContext context)
        {
            // Validate the extra fields data
            var validatedResponse = await InvokeLambdaAsync("ValidateBillPay", response, input, context);
            if (validatedResponse == null || !validatedResponse.ContainsKey("Valid") || !(bool)validatedResponse["Valid"])
            {
                throw new Exception("Extra fields validation failed.");
            }
        }

        private async Task ValidateComplianceInformation(Dictionary<string, object> response, ProcessBillPayDto input, ILambdaContext context)
        {
            // Validate the compliance information
            var validatedResponse = await InvokeLambdaAsync("ValidateBillPay", response, input, context);
            if (validatedResponse == null || !validatedResponse.ContainsKey("Valid") || !(bool)validatedResponse["Valid"])
            {
                throw new Exception("Compliance information validation failed.");
            }
        }

        private async Task<BillerInfo> GetBillerInfoAsync(string billerId, ILambdaContext context)
        {
            try
            {
                var url = $"https://your-api-url/get-biller?billerId={billerId}";
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<BillerInfo>(json);
                }
                else
                {
                    context.Logger.LogError($"Error fetching biller info: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Exception occurred while fetching biller info: {ex.Message}");
                return null;
            }
        }

        private async Task<Dictionary<string, object>> RequestPrimaryBillDetails(ProcessBillPayDto input, ILambdaContext context)
        {
            return await PostToApiAsync("https://your-api-url/process-billpay", input, context);

        }

        private async Task<Dictionary<string, object>> RequestExtraFields(ProcessBillPayDto input, ILambdaContext context)
        {
            return await PostToApiAsync("https://your-api-url/request-extra-fields", input, context);
        }

        private async Task<Dictionary<string, object>> RequestComplianceInformation(ProcessBillPayDto input, ILambdaContext context)
        {
            return await PostToApiAsync("https://your-api-url/request-compliance-info", input, context);
        }

        private async Task<Dictionary<string, object>> PostToApiAsync(string url, object input, ILambdaContext context)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(input);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
                }
                else
                {
                    throw new Exception($"Error in API call: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Exception in PostToApiAsync: {ex.Message}");
                throw;
            }
        }

        private async Task<Dictionary<string, object>> InvokeLambdaAsync(string functionName, Dictionary<string, object> payload, ProcessBillPayDto input, ILambdaContext context)
        {
            try
            {
                var request = new InvokeRequest
                {
                    FunctionName = functionName,
                    Payload = JsonConvert.SerializeObject(payload)
                };

                var response = await lambdaClient.InvokeAsync(request);

                if (response.StatusCode == 200)
                {
                    using (var sr = new StreamReader(response.Payload))
                    {
                        var jsonResponse = await sr.ReadToEndAsync();
                        return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
                    }
                }
                else
                {
                    throw new Exception($"Error invoking Lambda function: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Exception in InvokeLambdaAsync: {ex.Message}");
                throw;
            }
        }

        private async Task CacheResponse(string cacheKey, Dictionary<string, object> response, TimeSpan expiry)
        {

            var db = redis.GetDatabase();
            var jsonResponse = JsonConvert.SerializeObject(response);
            await db.StringSetAsync(cacheKey, jsonResponse, expiry);
        }

        private async Task<Dictionary<string, object>> GetCachedResponse(string cacheKey)
        {
            var db = redis.GetDatabase();
            var cachedResponse = await db.StringGetAsync(cacheKey);
            return cachedResponse.HasValue ? JsonConvert.DeserializeObject<Dictionary<string, object>>(cachedResponse) : null;
        }

        private static void UpdateUserDailyLimit(string userId, decimal amount, ILambdaContext context)
        {
            // Implementation to update user daily limit
        }

        private async Task<UserDailyLimit> GetUserDailyLimit(string userId, ILambdaContext context)
        {
            // Implementation to fetch user daily limit
            return new UserDailyLimit { DailyRemainCnt = 5, DailyRemainAmount = 1000m };
        }

        private async Task StoreTransactionData(ProcessBillPayDto input, Dictionary<string, object> response, Dictionary<string, object> transactionResponse, ILambdaContext context)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "INSERT INTO transactions (TransactionId, UserId, Amount, Status, Response) VALUES (@TransactionId, @UserId, @Amount, @Status, @Response)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@TransactionId", input.TransactionId);
                        command.Parameters.AddWithValue("@UserId", input.UserId);
                        command.Parameters.AddWithValue("@Amount", (decimal)response["Amount"]);
                        command.Parameters.AddWithValue("@Status", transactionResponse["Status"]);
                        command.Parameters.AddWithValue("@Response", JsonConvert.SerializeObject(transactionResponse));
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Exception in StoreTransactionData: {ex.Message}");
                throw;
            }
        }

        private static ProcessBillPayDto ValidateInputData(ProcessBillPayDto input)
        {
            // Validate input data
            return input;
        }

        public class UserDailyLimit
        {
            public int DailyRemainCnt { get; set; }
            public decimal DailyRemainAmount { get; set; }
        }

    }
}

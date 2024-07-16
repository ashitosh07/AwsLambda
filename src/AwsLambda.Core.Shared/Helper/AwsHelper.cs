using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using AwsLambda.Core.Shared.Const;
using Newtonsoft.Json.Linq;

namespace AwsLambda.Core.Shared.Helper;

public static class AwsHelper
{
    public static string GetDocumentDatabaseConnectionString()
    {
        GetSecretValueRequest request = new()
        {
            SecretId = AwsConst.DocSecretName,
            VersionStage = AwsConst.Version,
        };

        using var secretsManagerClient = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(AwsConst.Region));

        try
        {
            var response = secretsManagerClient.GetSecretValueAsync(request).Result;
            string secret = response.SecretString;
            JObject jsonObject = JObject.Parse(secret);

            string username = (string)jsonObject["username"];
            string password = (string)jsonObject["password"];
            string host = (string)jsonObject["host"];
            string port = (string)jsonObject["port"];
            string dbName = (string)jsonObject["username"];
            string authSource = (string)jsonObject["authSource"];
            string authMechanism = (string)jsonObject["authMechanism"];

            string connectionString = $"mongodb://{username}:{password}@{host}:{port}/{dbName}?authSource={AwsConst.Admin}&authMechanism={AwsConst.Mechanism}";

            return connectionString;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public static string GetRdsDatabaseConnectionString()
    {
        using var secretsManagerClient = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(AwsConst.Region));
        var request = new GetSecretValueRequest
        {
            SecretId = AwsConst.RdsSecretName,
            VersionStage = AwsConst.Version,
        };
        try
        {
            var response = secretsManagerClient.GetSecretValueAsync(request).Result;
            var secret = response.SecretString;
            var secretObject = JObject.Parse(secret);

            var rdsProxyHost = secretObject["rds_proxy_host"].ToString();
            var userName = secretObject["user_name"].ToString();
            var port = secretObject["port"].ToString();
            var password = secretObject["password"].ToString();
            var complianceDatabase = secretObject["compliance_database"].ToString();

            var connectionString = $"Server={rdsProxyHost};Port={port};Database={complianceDatabase};Uid={userName};Pwd={password};Connect Timeout=5";

            return connectionString;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }
}

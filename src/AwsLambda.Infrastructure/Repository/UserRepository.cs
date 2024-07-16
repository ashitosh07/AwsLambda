using AwsLambda.Core.RepositoryInterfaces;
using AwsLambda.Core.Shared.Const;
using AwsLambda.Core.Shared.Helper;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AwsLambda.Infrastructure.Repository;

public class UserRepository : IUserRepository
{
    public async Task<IEnumerable<BsonDocument>> GetAllUsers()
    {
        var mongoClient = new MongoClient(AwsHelper.GetDocumentDatabaseConnectionString());
        var collection = mongoClient.GetDatabase(DocumentDbConst.DocumentCollection).GetCollection<BsonDocument>(DocumentDbConst.UsersDbTable);
        var filter = Builders<BsonDocument>.Filter.Empty;
        var documents = await collection.Find(filter).ToListAsync();
        return documents;
    }
}

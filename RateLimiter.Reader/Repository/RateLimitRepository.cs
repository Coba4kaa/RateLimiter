using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RateLimiter.Reader.Repository.DbModels;
using RateLimiter.Reader.Repository.Mappers;
using RateLimiter.Reader.Service.DomainModels;

namespace RateLimiter.Reader.Repository
{
    public class RateLimitRepository : IRateLimitRepository
    {
        private readonly IMongoCollection<RateLimitDbModel> _rateLimitCollection;
        private readonly int _batchSize;

        public RateLimitRepository(IOptions<DatabaseSettings> dbSettings)
        {
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DbName);
            _rateLimitCollection = mongoDatabase.GetCollection<RateLimitDbModel>(dbSettings.Value.CollectionName);
            _batchSize = dbSettings.Value.BatchSize;
        }

        public async Task<ConcurrentDictionary<string, RateLimitDomainModel>> GetRateLimitsBatchAsync()
        {
            var filter = Builders<RateLimitDbModel>.Filter.Empty;
            var rateLimits = new ConcurrentDictionary<string, RateLimitDomainModel>();

            using var cursor = await _rateLimitCollection.FindAsync(filter, new FindOptions<RateLimitDbModel> { BatchSize = _batchSize });
            while (await cursor.MoveNextAsync())
            {
                foreach (var dbModel in cursor.Current)
                {
                    var domainModel = RateLimitMapper.ToDomainModel(dbModel);
                    rateLimits[dbModel.Id.ToString()] = domainModel;
                }
            }

            return rateLimits;
        }

        public async IAsyncEnumerable<ChangeStreamDocument<RateLimitDbModel>> WatchRateLimitChangesAsync()
        {
            var changeStream = await _rateLimitCollection.WatchAsync();
            while (await changeStream.MoveNextAsync())
            {
                foreach (var change in changeStream.Current)
                {
                    yield return change;
                }
            }
        }
    }
}
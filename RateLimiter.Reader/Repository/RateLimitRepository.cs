using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RateLimiter.Reader.Repository.DbModels;
using RateLimiter.Reader.Repository.Mappers;
using RateLimiter.Reader.Service.DomainModels;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace RateLimiter.Reader.Repository
{
    public class RateLimitRepository : IRateLimitRepository
    {
        private readonly IMongoCollection<RateLimitDbModel> _rateLimitCollection;
        private ConcurrentDictionary<string, RateLimitDomainModel> _rateLimits = new();
        private readonly int _batchSize;

        public RateLimitRepository(IOptions<DatabaseSettings> dbSettings)
        {
            var mongoClient = new MongoClient(dbSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSettings.Value.DbName);
            _rateLimitCollection = mongoDatabase.GetCollection<RateLimitDbModel>(dbSettings.Value.CollectionName);
            _batchSize = dbSettings.Value.BatchSize;

            _ = GetRateLimitsBatchAsync();
        }
        
        public Collection<RateLimitDomainModel> GetCurrentRateLimits()
        {
            return new Collection<RateLimitDomainModel>(_rateLimits.Values.ToList());
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

            _rateLimits = rateLimits;
            return _rateLimits;
        }

        public void StartProcessingEvents()
        {
            Task.Run(async () => await WatchRateLimitChangesAsync());
        }

        public async Task WatchRateLimitChangesAsync()
        {
            var changeStream = await _rateLimitCollection.WatchAsync();
            while (await changeStream.MoveNextAsync())
            {
                foreach (var change in changeStream.Current)
                {
                    Console.WriteLine($"Change detected: {change.OperationType} for ID: {change.DocumentKey["_id"]}");
                    ApplyRateLimitChange(change);
                }
            }
        }

        private void ApplyRateLimitChange(ChangeStreamDocument<RateLimitDbModel> change)
        {
            Console.WriteLine($"Applying change: {change.OperationType} for document ID: {change.DocumentKey["_id"]}");

            switch (change.OperationType)
            {
                case ChangeStreamOperationType.Insert:
                    var insertedLimit = RateLimitMapper.ToDomainModel(change.FullDocument);
                    _rateLimits[change.FullDocument.Id.ToString()] = insertedLimit;
                    Console.WriteLine($"Inserted new rate limit: {insertedLimit.Route} - {insertedLimit.RequestsPerMinute} RPM");
                    break;

                case ChangeStreamOperationType.Update:
                    if (change.UpdateDescription?.UpdatedFields != null)
                    {
                        var updatedId = change.DocumentKey["_id"].ToString();
                        if (_rateLimits.ContainsKey(updatedId))
                        {
                            var updatedRoute = _rateLimits[updatedId].Route;
                            var updatedRequestsPerMinute = change.UpdateDescription.UpdatedFields["RequestsPerMinute"].AsInt32;

                            _rateLimits[updatedId] = new RateLimitDomainModel(updatedRoute, updatedRequestsPerMinute);
                            Console.WriteLine($"Updated rate limit: {updatedRoute} - {updatedRequestsPerMinute} RPM");
                        }
                    }
                    break;

                case ChangeStreamOperationType.Delete:
                    _rateLimits.Remove(change.DocumentKey["_id"].ToString(), out var value);
                    Console.WriteLine($"Deleted rate limit with ID: {change.DocumentKey["_id"]}");
                    break;
            }
        }
    }
}

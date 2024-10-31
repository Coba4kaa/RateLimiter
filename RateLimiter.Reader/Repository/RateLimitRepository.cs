using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RateLimiter.Reader.Repository.DbModels;
using RateLimiter.Reader.Repository.Mappers;
using RateLimiter.Reader.Service.DomainModels;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Timers;
using Timer = System.Timers.Timer;

namespace RateLimiter.Reader.Repository
{
    public class RateLimitRepository : IRateLimitRepository
    {
        private readonly IMongoCollection<RateLimitDbModel> _rateLimitCollection;
        private readonly int _batchSize;
        private Dictionary<string, RateLimitDomainModel> _rateLimits = new();
        private readonly ConcurrentQueue<Action> _changeQueue = new();
        private readonly Timer _timer;

        public RateLimitRepository(IOptions<DatabaseSettings> dbSettings)
        {
            var mongoClient = new MongoClient(dbSettings.Value.RateLimiterDb);
            var mongoDatabase = mongoClient.GetDatabase("rate_limiter_db");
            _rateLimitCollection = mongoDatabase.GetCollection<RateLimitDbModel>("rate_limits");
            _batchSize = dbSettings.Value.BatchSize;

            _timer = new Timer(60000);
            _timer.Elapsed += ApplyQueuedChanges;
            _timer.Start();
        }

        public async Task<Dictionary<string, RateLimitDomainModel>> GetRateLimitsBatchAsync()
        {
            var filter = Builders<RateLimitDbModel>.Filter.Empty;
            var rateLimits = new Dictionary<string, RateLimitDomainModel>();

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

        public async Task WatchRateLimitChangesAsync()
        {
            var changeStream = await _rateLimitCollection.WatchAsync();
            while (await changeStream.MoveNextAsync())
            {
                foreach (var change in changeStream.Current)
                {
                    Console.WriteLine($"Change detected: {change.OperationType} for ID: {change.DocumentKey["_id"]}");
                    EnqueueRateLimitChange(change);
                }
            }
        }
        
        public Collection<RateLimitDomainModel> GetCurrentRateLimits()
        {
            if (!_changeQueue.IsEmpty)
                ApplyQueuedChanges(null, null);

            return new Collection<RateLimitDomainModel>(_rateLimits.Values.ToList());
        }


        private void EnqueueRateLimitChange(ChangeStreamDocument<RateLimitDbModel> change)
        {
            _changeQueue.Enqueue(() => ApplyRateLimitChange(change));
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
                    _rateLimits.Remove(change.DocumentKey["_id"].ToString());
                    Console.WriteLine($"Deleted rate limit with ID: {change.DocumentKey["_id"]}");
                    break;
            }
        }

        private void ApplyQueuedChanges(object? sender, ElapsedEventArgs? e)
        {
            if (_changeQueue.IsEmpty) return;

            while (_changeQueue.TryDequeue(out var action))
                action();

            Console.WriteLine("All queued changes have been applied.");
        }
    }
}

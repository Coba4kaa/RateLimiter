using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using MongoDB.Driver;
using RateLimiter.Reader.Repository;
using RateLimiter.Reader.Repository.DbModels;
using RateLimiter.Reader.Repository.Mappers;
using RateLimiter.Reader.Service.DomainModels;

namespace RateLimiter.Reader.Service.DomainServices
{
    public class RateLimitService(IRateLimitRepository rateLimitRepository) : IRateLimitService
    {
        private ConcurrentDictionary<string, RateLimitDomainModel> _rateLimits = new();

        public async Task InitializeRateLimitsAsync()
        {
            _rateLimits = await rateLimitRepository.GetRateLimitsBatchAsync();
        }

        public IReadOnlyCollection<RateLimitDomainModel> GetCurrentRateLimits()
        {
            return new ReadOnlyCollection<RateLimitDomainModel>(_rateLimits.Values.ToList());
        }

        public async Task ProcessRateLimitChangesAsync(IAsyncEnumerable<ChangeStreamDocument<RateLimitDbModel>> changes)
        {
            await foreach (var change in changes)
            {
                ApplyRateLimitChange(change);
            }
        }

        private void ApplyRateLimitChange(ChangeStreamDocument<RateLimitDbModel> change)
        {
            switch (change.OperationType)
            {
                case ChangeStreamOperationType.Insert:
                    var insertedLimit = RateLimitMapper.ToDomainModel(change.FullDocument);
                    _rateLimits[change.FullDocument.Id.ToString()] = insertedLimit;
                    break;

                case ChangeStreamOperationType.Update:
                    if (change.UpdateDescription?.UpdatedFields != null)
                    {
                        var updatedId = change.DocumentKey["_id"].ToString();
                        if (_rateLimits.TryGetValue(updatedId, out RateLimitDomainModel? value))
                        {
                            var updatedRoute = value.Route;
                            var updatedRequestsPerMinute = change.UpdateDescription.UpdatedFields["RequestsPerMinute"].AsInt32;
                            _rateLimits[updatedId] = new RateLimitDomainModel(updatedRoute, updatedRequestsPerMinute);
                        }
                    }
                    break;

                case ChangeStreamOperationType.Delete:
                    _rateLimits.TryRemove(change.DocumentKey["_id"].ToString(), out _);
                    break;
            }
        }
    }
}
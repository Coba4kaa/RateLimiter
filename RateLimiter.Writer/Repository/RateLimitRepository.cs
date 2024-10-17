using MongoDB.Driver;
using RateLimiter.Writer.Repository.DbModels;
using RateLimiter.Writer.Repository.Mappers;
using RateLimiter.Writer.Service.DomainModels;

namespace RateLimiter.Writer.Repository;

public class RateLimitRepository : IRateLimitRepository
{
    private readonly IMongoCollection<RateLimitDbModel> _rateLimitCollection;

    public RateLimitRepository(string connectionString)
    {
        var mongoClient = new MongoClient(connectionString);
        var mongoDatabase = mongoClient.GetDatabase("rate_limiter_db");
        _rateLimitCollection = mongoDatabase.GetCollection<RateLimitDbModel>("rate_limits");
    }

    public async Task<bool> CreateAsync(RateLimitDomainModel rateLimitDomainModel, CancellationToken cancellationToken)
    {
        var rateLimitDbModel = RateLimitMapper.ToDbModel(rateLimitDomainModel);
        try
        {
            var existing = await _rateLimitCollection
                .Find(x => x.Route == rateLimitDbModel.Route)
                .FirstOrDefaultAsync(cancellationToken);
            if (existing != null)
            {
                return false;
            }
            await _rateLimitCollection.InsertOneAsync(rateLimitDbModel, cancellationToken: cancellationToken);
            return true;
        }
        catch (MongoException)
        {
            return false;
        }
    }

    public async Task<RateLimitDomainModel?> GetByRouteAsync(string route, CancellationToken cancellationToken)
    {
        try
        {
            var filter = Builders<RateLimitDbModel>.Filter.Eq(x => x.Route, route);
            var rateLimitDbModel = await _rateLimitCollection
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);
            
            return RateLimitMapper.ToDomainModel(rateLimitDbModel);
        }
        catch (MongoException)
        {
            return null;
        }
    }

    public async Task<bool> UpdateAsync(RateLimitDomainModel rateLimitDomainModel, CancellationToken cancellationToken)
    {
        var rateLimitDbModel = RateLimitMapper.ToDbModel(rateLimitDomainModel);
        try
        {
            var updateDefinition = Builders<RateLimitDbModel>.Update
                .Set(x => x.RequestsPerMinute, rateLimitDbModel.RequestsPerMinute);
            var result = await _rateLimitCollection.UpdateOneAsync(x => x.Route == rateLimitDbModel.Route,
                updateDefinition, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }
        catch (MongoException)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(string route, CancellationToken cancellationToken)
    {
        try
        {
            var filter = Builders<RateLimitDbModel>.Filter.Eq(x => x.Route, route);
            var result = await _rateLimitCollection.DeleteOneAsync(filter, cancellationToken: cancellationToken);
            return result.DeletedCount > 0;
        }
        catch (MongoException)
        {
            return false;
        }
    }
}
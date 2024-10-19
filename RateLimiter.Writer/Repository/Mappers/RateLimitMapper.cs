using RateLimiter.Writer.Repository.DbModels;
using RateLimiter.Writer.Service.DomainModels;

namespace RateLimiter.Writer.Repository.Mappers;

using Riok.Mapperly.Abstractions;

[Mapper]
public static partial class RateLimitMapper {
    public static partial RateLimitDbModel ToDbModel(RateLimitDomainModel rateLimitDomainModel);
    public static partial RateLimitDomainModel? ToDomainModel(RateLimitDbModel? rateLimitDbModel);
}
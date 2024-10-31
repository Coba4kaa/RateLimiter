using RateLimiter.Reader.Repository.DbModels;
using RateLimiter.Reader.Service.DomainModels;
using Riok.Mapperly.Abstractions;

namespace RateLimiter.Reader.Repository.Mappers;

[Mapper]
public static partial class RateLimitMapper {
    public static partial RateLimitDomainModel? ToDomainModel(RateLimitDbModel? rateLimitDbModel);
}
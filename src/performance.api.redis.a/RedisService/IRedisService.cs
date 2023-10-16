using Amazon.Lambda.Core;

namespace performance.api.redis.a.RedisService;

/// <summary>
/// An interface for a service that implements the business logic of our Lambda functions
/// </summary>
public interface IRedisService
{
    Task<ResultTime> SetupData(SetupRequest request, ILambdaContext context);
    Task<ResultTime> DirectData(DataRequest request, ILambdaContext context);
    Task<ResultTime> IndirectData(DataRequest request, ILambdaContext context);
}

using Amazon.Lambda.Core;

namespace performance.api.redis.a.RedisService;

/// <summary>
/// An interface for a service that implements the business logic of our Lambda functions
/// </summary>
public interface IRedisService
{
    Task<ResultTime> SetupData(ILambdaContext context);
    Task<ResultTime> DirectData(string key, ILambdaContext context);
    Task<ResultTime> IndirectData(string key, ILambdaContext context);
}

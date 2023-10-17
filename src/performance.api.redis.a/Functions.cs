using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using performance.api.redis.a.RedisService;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace performance.api.redis.a;

/// <summary>
/// A collection of sample Lambda functions that call redis directly and indirectly.
/// </summary>
public class Functions
{
    private readonly IRedisService _redisService;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <remarks>
    /// The <see cref="IRedisService"/> implementation that we
    /// instantiated in <see cref="Startup"/> will be injected here.
    ///
    /// As an alternative, a dependency could be injected into each
    /// Lambda function handler via the [FromServices] attribute.
    /// </remarks>
    public Functions(IRedisService redisService)
    {
        _redisService = redisService;
    }

    /// <summary>
    /// Root route that provides information about the other requests that can be made.
    /// </summary>
    /// <returns>API descriptions.</returns>
    [LambdaFunction(@Policies = "AWSLambdaBasicExecutionRole,AmazonVPCFullAccess,AmazonElastiCacheFullAccess")]
    [HttpApi(LambdaHttpMethod.Get, "/")]
    public string Default()
    {
        const string docs = @"Performance API Redis A B testing.
POST /setup - adds data to each redis endpoint passed in { RedisEndpoint: string }
POST /direct - retrieves data from redis directly, { Key: small|medium|large, RedisEndpoint: string }
POST /indirect - retrieves data redis by calling another api, { Key: small|medium|large, RedisEndpoint: string, ApiEndPoint: string }";
        return docs;
    }

    /// <summary>
    /// Setup the data for redis cluster.
    /// </summary>
    /// <returns>200 status</returns>
    [LambdaFunction(@Policies = "AWSLambdaBasicExecutionRole,AmazonVPCFullAccess,AmazonElastiCacheFullAccess")]
    [HttpApi(LambdaHttpMethod.Post, "/setup")]
    public async Task<IHttpResult> Setup([FromBody] SetupRequest request, ILambdaContext context)
    {
        return HttpResults.Ok(await _redisService.SetupData(request, context));
    }

    /// <summary>
    /// Get data from redis directly.
    /// </summary>
    /// <returns>200 status</returns>
    [LambdaFunction(@Policies = "AWSLambdaBasicExecutionRole,AmazonVPCFullAccess,AmazonElastiCacheFullAccess")]
    [HttpApi(LambdaHttpMethod.Post, "/direct")]
    public async Task<IHttpResult> DirectData([FromBody] DataRequest request, ILambdaContext context)
    {
        return HttpResults.Ok(await _redisService.DirectData(request, context));
    }

    /// <summary>
    /// Get data from redis indirectly.
    /// </summary>
    /// <returns>200 status</returns>
    [LambdaFunction(@Policies = "AWSLambdaBasicExecutionRole,AmazonVPCFullAccess,AmazonElastiCacheFullAccess,AWSLambdaVPCAccessExecutionRole")]
    [HttpApi(LambdaHttpMethod.Post, "/indirect")]
    public async Task<IHttpResult> IndirectData([FromBody] DataRequest request, ILambdaContext context)
    {
        return HttpResults.Ok(await _redisService.IndirectData(request, context));
    }
}
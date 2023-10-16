using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Amazon.Lambda.Core;
using StackExchange.Redis;

namespace performance.api.redis.a.RedisService;

/// <summary>
/// The implementation of <see cref="IRedisService"/>
/// that will be used by our Lambda functions.
/// </summary>
public class RedisService : IRedisService
{
    public async Task<ResultTime> SetupData(SetupRequest request, ILambdaContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        foreach (var endpoint in request.Redis)
        {
            var redis = await ConnectionMultiplexer.ConnectAsync(endpoint);
            var db = redis.GetDatabase();

            var ran = new Random();
            var testList = new List<int>();

            for (var i = 0; i <= 1000; i++)
            {
                testList.Add(ran.Next());
            }

            await db.KeyDeleteAsync("small");
            await db.HashSetAsync("small", new[]
            {
                new HashEntry("test", JsonSerializer.Serialize(testList))
            });

            for (var i = 0; i <= 50000; i++)
            {
                testList.Add(ran.Next());
            }

            await db.KeyDeleteAsync("medium");
            await db.HashSetAsync("medium", new[]
            {
                new HashEntry("test", JsonSerializer.Serialize(testList))
            });

            for (var i = 0; i <= 500000; i++)
            {
                testList.Add(ran.Next());
            }

            await db.KeyDeleteAsync("large");
            await db.HashSetAsync("large", new[]
            {
                new HashEntry("test", JsonSerializer.Serialize(testList))
            });
        }

        stopwatch.Stop();
        var result = new ResultTime()
        {
            Message = $"Set cluster data. Total time in ms: {stopwatch.ElapsedMilliseconds}",
            ElapsedTime = stopwatch.ElapsedMilliseconds
        };

        context.Logger.LogInformation(result.Message);
        return result;
    }

    public async Task<ResultTime> DirectData(DataRequest request, ILambdaContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var redis = await ConnectionMultiplexer.ConnectAsync(request.RedisEndpoint);
        var db = redis.GetDatabase();
        var data = await db.HashGetAsync(request.Key,"test");

        stopwatch.Stop();
        var result = new ResultTime()
        {
            Message = $"Retrieve cluster data. Total time in ms: {stopwatch.ElapsedMilliseconds}",
            ElapsedTime = stopwatch.ElapsedMilliseconds,
            Data = JsonSerializer.Deserialize<List<int>>(data!)
        };

        context.Logger.LogInformation(result.Message);
        return result;
    }

    public async Task<ResultTime> IndirectData(DataRequest request, ILambdaContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var content = new StringContent(
            JsonSerializer.Serialize(request),
            null,
            "application/json");

        var client = new HttpClient();
        var response = await client.PostAsync(request.ApiEndpoint, content);
        var jsonResponse = await response.Content.ReadFromJsonAsync<ResultTime>();

        context.Logger.LogInformation($"API B: {jsonResponse}");

        stopwatch.Stop();
        var result = new ResultTime()
        {
            Message = $"Retrieve cluster data. Total time in ms: {stopwatch.ElapsedMilliseconds}",
            ElapsedTime = stopwatch.ElapsedMilliseconds,
            ApiB = jsonResponse
        };

        context.Logger.LogInformation(result.Message);
        return result;
    }
}
using System.Diagnostics;
using System.Net.Http.Json;
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
    public async Task<ResultTime> SetupData(ILambdaContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var redis = await ConnectionMultiplexer.ConnectAsync(Environment.GetEnvironmentVariable("redisEndpoint")!);
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

        stopwatch.Stop();
        var result = new ResultTime()
        {
            Message = $"Set cluster data. Total time in ms: {stopwatch.ElapsedMilliseconds}",
            ElapsedTime = stopwatch.ElapsedMilliseconds
        };

        context.Logger.LogInformation(result.Message);
        return result;
    }

    public async Task<ResultTime> DirectData(string key, ILambdaContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var redis = await ConnectionMultiplexer.ConnectAsync(Environment.GetEnvironmentVariable("redisEndpoint")!);
        var db = redis.GetDatabase();
        var data = await db.HashGetAsync(key,"test");

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

    public async Task<ResultTime> IndirectData(string key, ILambdaContext context)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var client = new HttpClient();
        var response = await client.GetAsync(Environment.GetEnvironmentVariable("bEndpoint") + $"/{key}");
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
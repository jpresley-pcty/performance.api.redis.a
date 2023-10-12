using Xunit;
using Amazon.Lambda.TestUtilities;
using Moq;
using performance.api.redis.a.RedisService;


namespace performance.api.redis.a.Tests;

public class FunctionsTest
{
    private readonly IRedisService _mockRedisService;

    public FunctionsTest()
    {
        var context = new TestLambdaContext();
        var key = "small";
        var mock = new Mock<IRedisService>();
        mock.Setup(m => m.DirectData(key, context)).Returns(() => new ResultTime(){Message = "test", ElapsedTime = 0});

        _mockRedisService = mock.Object;
    }

    [Fact]
    public void TestDefault()
    {
        var functions = new Functions(_mockRedisService);
        Assert.NotNull(functions.Default());
    }
}

namespace performance.api.redis.a;

public class ResultTime
{
  public string? Message { get; set; }
  public long ElapsedTime { get; set; }
  public ResultTime? ApiB { get; set; }
  public List<int>? Data { get; set; }
}
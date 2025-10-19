namespace Web.Jobs;

public class ResumableJob : BackgroundService
{
    private readonly ILogger<ResumableJob> _logger;

    public ResumableJob(ILogger<ResumableJob> logger)
    {
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CoreRunAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task CoreRunAsync()
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        _logger.LogInformation("Today is {Today}", today);
    }
    
}
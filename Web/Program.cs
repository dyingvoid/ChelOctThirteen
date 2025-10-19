using Core.ApiIntegration;
using Core.FileManagement;
using Core.FileManagement.Interfaces;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Web.Jobs;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var pipelineConfigure =
    (ResiliencePipelineBuilder<HttpResponseMessage> pipelineBuilder) =>
    {
        pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            Delay = TimeSpan.FromSeconds(2)
        });
        pipelineBuilder.AddTimeout(TimeSpan.FromSeconds(10));
    };

builder.Services
    .AddHttpClient<IApiClient, ApiClient>(client =>
    {
        client.BaseAddress = new Uri("http://fias.nalog.ru");
    })
    .AddResilienceHandler("fias-pipeline", pipelineConfigure);
builder.Services
    .AddHttpClient<IFileLoader, FileLoader>("fias-file")
    .AddResilienceHandler("fias-pipeline", pipelineConfigure);

builder.Services.Configure<FileLoaderOptions>(builder.Configuration.GetSection("FileLoader"));
builder.Services.AddScoped<IFileLoader, FileLoader>();
builder.Services.AddScoped<IUnzipper, Unzipper>();
builder.Services.AddScoped<FileManager>();

builder.Services.AddHostedService<ResumableJob>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

app.MapGet("/health", () => "healthy");
app.MapGet("/load", async (FileManager manager) =>
{ 
    await manager.LoadLatest();
    return Results.Ok();
});

app.Run();
using System.Text.Json;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using RepoGrinder.Core.Client;
using RepoGrinder.DataModels;
using RepoGrinder.Github.Client;
using RepoGrinder.Services;

var builder = WebApplication.CreateBuilder(args);

ConfigureApplicationSettings(builder);
BuildContainer(builder.Services);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

var options = new DashboardOptions
{
    Authorization = Array.Empty<IDashboardAuthorizationFilter>()
};

app.UseHangfireDashboard(options: options);

app.Run();


void BuildContainer(IServiceCollection services)
{
    services.Configure<JsonSerializerOptions>(jsonSerializerOptions =>
    {
        jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        jsonSerializerOptions.PropertyNameCaseInsensitive = false;
    });

    services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.AddConsole();
    });

    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();

    services.AddHangfire(configuration =>
    {
        configuration.UseMemoryStorage();
    });

    services.AddHangfireServer();
    services.AddHttpClient();

    services.AddScoped<GrinderJob>();
    services.AddScoped<IGrinderService, GrinderService>();
    services.AddScoped<ICrawlerClient, GithubCrawlerClient>();
    services.AddSingleton(_ => new CrawlerConfiguration {Uri = "https://api.github.com"});
    services.AddSingleton<IUriParser, GithubUriParser>();
    services.AddSingleton<Newtonsoft.Json.JsonSerializer>();
    services.AddSingleton<IDataRepository<CodeStats>, MemoryDataRepository>();
}

void ConfigureApplicationSettings(WebApplicationBuilder wab)
{
    wab.Configuration
        .AddJsonFile("appsettings.json", true)
        .AddJsonFile("appsettings.Development.json", true)
        .AddEnvironmentVariables();
}

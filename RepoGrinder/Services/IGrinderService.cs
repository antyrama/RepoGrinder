namespace RepoGrinder.Services;

public interface IGrinderService
{
    Task<string[]> GetStatsAsync(string uri);
}
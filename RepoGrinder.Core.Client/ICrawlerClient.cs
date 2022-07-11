namespace RepoGrinder.Core.Client
{
    public interface ICrawlerClient
    {
        IAsyncEnumerable<string> GetFileNamesAsync(string uri);
    }
}

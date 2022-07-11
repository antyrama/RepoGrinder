using RepoGrinder.Core.Client;

namespace RepoGrinder.Services
{
    public class GrinderService : IGrinderService
    {
        private readonly ICrawlerClient _crawlerClient;

        public GrinderService(ICrawlerClient crawlerClient)
        {
            _crawlerClient = crawlerClient;
        }

        public async Task<string[]> GetStatsAsync(string uri)
        {
            var dictionary = new Dictionary<string, int>();

            await foreach (var fileName in _crawlerClient.GetFileNamesAsync(uri))
            {
                var extension = Path.GetExtension(fileName).ToLowerInvariant();

                if (!dictionary.ContainsKey(extension))
                {
                    dictionary.Add(extension, 1);
                }
                else
                {
                    dictionary[extension]++;
                }
            }

            return dictionary.OrderByDescending(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .Take(5)
                .ToArray();
        }
    }
}

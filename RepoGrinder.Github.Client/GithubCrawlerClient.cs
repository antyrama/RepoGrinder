using System.Collections.Concurrent;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using RepoGrinder.Core.Client;
using RepoGrinder.Github.Client.Models;

namespace RepoGrinder.Github.Client
{
    public class GithubCrawlerClient : ICrawlerClient
    {
        private readonly JsonSerializer _serializer;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IUriParser _uriParser;

        public GithubCrawlerClient(JsonSerializer serializer,
            IHttpClientFactory clientFactory,
            IUriParser uriParser)
        {
            _serializer = serializer;
            _clientFactory = clientFactory;
            _uriParser = uriParser;
        }

        public async IAsyncEnumerable<string> GetFileNamesAsync(string uri)
        {
            // using concurrent queue to be safe for read/write operations for nested threads
            var queue = new ConcurrentQueue<string>();

            var task = await Task.Factory.StartNew(() => GetFileNamesAsync(_uriParser.Parse(uri), queue));

            while (!task.IsCompleted || queue.Any())
            {
                if (queue.TryDequeue(out var item))
                {
                    yield return item;
                }
            }

            // nasty but required to catch exceptions
            if (task.Exception is not null)
            {
                throw task.Exception;
            }
        }

        private async Task GetFileNamesAsync(Uri uri, ConcurrentQueue<string> queue)
        {
            var files = await GetFilesAsync(uri);

            var downloadTasks = new List<Task>();

            foreach (var file in files)
            {
                switch (file.Type)
                {
                    case "file":
                        queue.Enqueue(file.Name!);
                        break;
                    case "dir":
                        // download task start immediately 
                        downloadTasks.Add(await Task.Factory.StartNew(() => GetFileNamesAsync(new Uri(file.Url!), queue)));
                        break;
                }
            }

            // waiting for all to finish
            await Task.WhenAll(downloadTasks);
        }

        private async Task<ICollection<RepoFile>> GetFilesAsync(Uri uri)
        {
            using var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, uri);
            requestMessage.Headers.UserAgent.Add(new ProductInfoHeaderValue("request", "1"));

            // ResponseHeadersRead used to avoid allocation of memory for whole load
            using var responseMessage = await _clientFactory.CreateClient()
                .SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);

            responseMessage.EnsureSuccessStatusCode();

            await using var stream = await responseMessage.Content.ReadAsStreamAsync();

            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);

            return _serializer.Deserialize<GithubGetResponse>(jsonTextReader);
        }
    }
}

using RepoGrinder.Core.Client;

namespace RepoGrinder.Github.Client
{
    public class GithubUriParser : IUriParser
    {
        private readonly CrawlerConfiguration _configuration;

        public GithubUriParser(CrawlerConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Uri Parse(string uri)
        {
            const string githubUri = "https://github.com/";

            try
            {
                if (uri.StartsWith(githubUri, StringComparison.InvariantCultureIgnoreCase))
                {
                    var parts = uri[githubUri.Length..].Split('/');
                    
                    return new Uri(new Uri(_configuration.Uri!),
                        new Uri($"repos/{parts[0]}/{parts[1]}/contents", UriKind.Relative));
                }

                throw new InvalidOperationException($"URI must start with [{githubUri}]");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Given URI is invalid", ex);
            }
        }
    }
}

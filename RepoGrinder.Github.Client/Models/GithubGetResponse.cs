using System.Collections.ObjectModel;

namespace RepoGrinder.Github.Client.Models
{
    internal class GithubGetResponse : Collection<RepoFile>
    {
    }

    internal class RepoFile
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Url { get; set; }
    }
}

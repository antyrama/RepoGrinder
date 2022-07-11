using RepoGrinder.Models;

namespace RepoGrinder.DataModels
{
    public class CodeStats
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public Status Status { get; set; }
        public string[]? Extensions { get; set; }
        public TimeSpan? Elapsed { get; set; }
    }
}

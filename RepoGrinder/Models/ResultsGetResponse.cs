using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace RepoGrinder.Models
{
    public class ResultsGetAllResponse : Collection<ResultsGetResponse>
    {
        public ResultsGetAllResponse(IList<ResultsGetResponse> list, Meta links) : base(list)
        {
            Links = links;
        }

        public Meta Links { get; }
    }

    public class ResultsGetResponse
    {
        public string Id { get; set; }
        public string Url { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status Status { get; set; }
        public TimeSpan? Elapsed { get; set; }
        public string[]? Results { get; set; }
        public Meta? Links { get; set; }
    }

    public enum Status
    {
        Finished,
        Running,
        Failed
    }
}

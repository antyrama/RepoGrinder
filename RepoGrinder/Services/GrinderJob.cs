using System.Diagnostics;
using RepoGrinder.DataModels;
using RepoGrinder.Models;

namespace RepoGrinder.Services
{
    public class GrinderJob
    {
        private readonly IGrinderService _service;
        private readonly IDataRepository<CodeStats> _repository;
        private readonly ILogger<GrinderJob> _logger;

        public GrinderJob(IGrinderService service,
            IDataRepository<CodeStats> repository,
            ILogger<GrinderJob> logger)
        {
            _service = service;
            _repository = repository;
            _logger = logger;
        }

        public async Task RunAsync(string id, string uri)
        {
            using var logger = _logger.BeginScope(new Dictionary<string, string> {{"jobId", id}});

            try
            {
                var codeStats = new CodeStats
                {
                    Id = id,
                    Url = uri,
                    Status = Status.Running
                };

                await _repository.AddAsync(codeStats);

                var sw = new Stopwatch();
                sw.Start();

                var extensions = await _service.GetStatsAsync(uri);

                sw.Stop();

                codeStats.Elapsed = sw.Elapsed;
                codeStats.Extensions = extensions;
                codeStats.Status = Status.Finished;

                await _repository.UpdateAsync(codeStats);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to process URI [{uri}]");

                var codeStats = await _repository.GetByIdAsync(id);
                if (codeStats is not null)
                {
                    codeStats.Status = Status.Failed;
                    await _repository.UpdateAsync(codeStats);
                }
            }
        }
    }
}

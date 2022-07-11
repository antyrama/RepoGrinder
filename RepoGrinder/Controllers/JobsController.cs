using Hangfire;
using Microsoft.AspNetCore.Mvc;
using RepoGrinder.Core.Client;
using RepoGrinder.Models;
using RepoGrinder.Services;

namespace RepoGrinder.Controllers
{
    [ApiController]
    [Route("jobs")]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> _logger;
        private readonly GrinderJob _grinderJob;
        private readonly IUriParser _uriParser;

        public JobsController(ILogger<JobsController> logger,
            GrinderJob grinderJob, IUriParser uriParser)
        {
            _logger = logger;
            _grinderJob = grinderJob;
            _uriParser = uriParser;
        }

        [HttpPost]
        [ProducesResponseType(typeof(SchedulePostResponse), 202)]
        [ProducesResponseType(statusCode: 400)]
        public async Task<IActionResult> Post(SchedulePostRequest request)
        {
            try
            {
                _uriParser.Parse(request.Uri);

                var id = Guid.NewGuid().ToString();

                BackgroundJob.Enqueue(() => _grinderJob.RunAsync(id, request.Uri));

                var self = $"{Request.Host}/results/{id}";

                return new AcceptedResult(self, new SchedulePostResponse { Id = id, Links = new Meta { Self = self } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to process request");
                
                return BadRequest();
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using RepoGrinder.DataModels;
using RepoGrinder.Models;
using RepoGrinder.Services;

namespace RepoGrinder.Controllers
{
    [ApiController]
    [Route("results")]
    public class ResultsController : ControllerBase
    {
        private readonly IDataRepository<CodeStats> _repository;

        public ResultsController(IDataRepository<CodeStats> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ResultsGetAllResponse), 200)]
        [ProducesResponseType(statusCode: 404)]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repository.GetAsync();

            if (!items.Any())
            {
                return NotFound();
            }

            return new OkObjectResult(
                new ResultsGetAllResponse(items
                    .Select(i => new ResultsGetResponse
                    {
                        Id = i.Id,
                        Elapsed = i.Elapsed,
                        Status = i.Status,
                        Results = i.Extensions,
                        Url = i.Url
                    }).ToList(), new Meta {Self = $"{Request.Host}{Request.Path}"}));
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ResultsGetResponse), 200)]
        [ProducesResponseType(statusCode: 404)]
        // good to add validation by FluentValidator or just use Guid type instead of string
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _repository.GetByIdAsync(id);

            if (item is null)
            {
                return NotFound();
            }

            return new OkObjectResult(new ResultsGetResponse
            {
                Id = item.Id,
                Elapsed = item.Elapsed,
                Status = item.Status,
                Results = item.Extensions,
                Url = item.Url,
                Links = new Meta { Self = $"{Request.Host}{Request.Path}" }
            });
        }
    }
}
using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RepoGrinder.DataModels;
using RepoGrinder.Models;
using RepoGrinder.Services;
using Xunit;

namespace RepoGrinder.Tests.Services
{
    public class GrinderJobTests
    {
        [Fact]
        public async Task ShouldStoreResultsWithFinishedStatus()
        {
            // assign
            var id = Guid.NewGuid().ToString();

            var grinderService = new Mock<IGrinderService>();
            var dataRepo = new Mock<IDataRepository<CodeStats>>();
            CodeStats? callbackObject = null;
            dataRepo.Setup(m => m.UpdateAsync(It.IsAny<CodeStats>()))
                .Callback<CodeStats>(cb => callbackObject = cb);

            var logger = new Mock<ILogger<GrinderJob>>();

            var sut = new GrinderJob(grinderService.Object, dataRepo.Object, logger.Object);
            
            // act
            await sut.RunAsync(id, "https://github.com/antyrama/Pinger");

            // assert
            callbackObject.Status.Should().Be(Status.Finished);
        }

        [Fact]
        public async Task ShouldStoreResultsWithFailedStatus()
        {
            // assign
            var id = Guid.NewGuid().ToString();

            var grinderService = new Mock<IGrinderService>();
            grinderService.Setup(gs => gs.GetStatsAsync(It.IsAny<string>()))
                .Throws(new Exception());

            var dataRepo = new Mock<IDataRepository<CodeStats>>();
            CodeStats? callbackObject = null;
            dataRepo.Setup(m => m.UpdateAsync(It.IsAny<CodeStats>()))
                .Callback<CodeStats>(cb => callbackObject = cb);
            dataRepo.Setup(m => m.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new CodeStats());

            var logger = new Mock<ILogger<GrinderJob>>();

            var sut = new GrinderJob(grinderService.Object, dataRepo.Object, logger.Object);

            // act
            await sut.RunAsync(id, "https://github.com/antyrama/Pinger");

            // assert
            callbackObject.Status.Should().Be(Status.Failed);
        }
    }

    public class TestFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient();
        }
    }
}
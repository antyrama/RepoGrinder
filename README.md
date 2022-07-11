# Introduction 
Repository Grinder help you to get most commonly used files extensions in your GitHub repository, for e.g.: **.cs, .yaml, .csproj, .gitignore, .sln**. As you can see on above example, the given repository probably contains some C# project. It will crawl through repository and collect only five most common file extensions. Application implemented for a technical interview purposes I participated in, showing off beauty of `IAsyncEnumerable<>` and `HttpCompletionOption.ResponseHeadersRead` on `HttpClient`.

Solution was build as ASP.NET Core application exposing restAPI. Call API endpoints to schedule job to process given code repository.

# Development requirements
1. Visual Studio 2022
2. netCore 6
3. Docker
4. Powershell

# Solution structure
There are few projects in solution
* `RepoGrinder` - main webapp
* `RepoGrinder.Core.Client` - core shared library for further client development
* `RepoGrinder.Github.Client` - client library for GitHub
* `RepoGrinder.Tests` - unit tests

# Build and Test
The easiest way to build and play is to use Docker:
1. Go to main folder with solution (should see the docker-compose.yaml file there)
2. Open PowerShell console
3. Run `docker-compose -f .\docker-compose.yml build` (optional when no changes to code)
4. Run `docker-compose -f .\docker-compose.yml up`
5. Open new tab in browser and navigate to [http://localhost:8090/swagger](http://localhost:8090/swagger)
6. Open new tab in browser and navigate to [http://localhost:8090/hangfire](http://localhost:8090/hangfire)

To run application from binaries, just navigate to `RepoGrinder\bin\Debug\net6.0` folder and run `RepoGrinder.exe`.

To run unit tests you have to open solution in Visual Studio and use Test Explorer.

To run application in Visual Studio, set `RepoGrinder` as startup project and hit F5, a web browser should pop up with Swagger dashboard.

# Using application
There are two main endpoints on Swagger documentation page:
* job
* results

To schedule job send, POST request to `job` endpoint with body
```
{
  "uri": "https://github.com/antyrama/Pinger"
}
```

In response you will get:
```
{
  "id": "ed189862-736f-492d-8ef0-b9dbbcbf3104",
  "links": {
    "self": "localhost:7277/results/ed189862-736f-492d-8ef0-b9dbbcbf3104"
  }
}
```

Surprised? Where are my extensions? By calling above endpoint you just scheduled a background job, which will be executed as soon as possible by Hangfire. What you got there is an `id` you can copy/paste into `results` endpoint request to get the extensions when job is finished. So you get:

```
{
  "id": "9058414b-0f4b-48be-a557-291f0bc46338",
  "url": "https://github.com/antyrama/Pinger",
  "status": "Finished",
  "elapsed": "00:00:01.4191150",
  "results": [
    ".cs",
    ".yaml",
    ".csproj",
    ".gitignore",
    ".sln"
  ],
  "links": {
    "self": "localhost:7277/results/9058414b-0f4b-48be-a557-291f0bc46338"
  }
}
```

# Remarks
* Pay attention to count of directories within a code repository, because of GitHub unauthenticated requests limits, which is 600 per hour, after that it returns 403 - mechanism of pulling file names have to recursively traverse whole repository, directory by directory.
* When running app in Visual Studio the port number of exposed web pages and API may vary depend on current launch settings.
* Application does not use any external storage to persist results - everything is in memory - so, when you restart the application, previous results will be gone.
* `GithubCrawlerClient.cs` - this construct may look weird at the first glance, but what I wanted to achieve was limit - as much possible - the usage memory by streaming from the top to bottom. Also the usage of `HttpClient` is a bit unusual, as you can see [HttpCompletionOption.ResponseHeadersRead](https://www.stevejgordon.co.uk/using-httpcompletionoption-responseheadersread-to-improve-httpclient-performance-dotnet) option there.

# TODO's
Things I'd really like to implement but already crossed the time boundary way much :)
* Configure [retry policy](https://github.com/App-vNext/Polly) on `AddHttpClient()` to ensure retrying on failure, for e.g.: try max 3 times, with exponentially growing sleep time between
* Implement couple of integration tests to show how it can be done, using Wiremock/Wiremock.Net to mimic GitHub API and Docker to run it.
* More unit tests, especially to cover `GithubCrawlerClient`
* Another simple client, for different source of files (let's say folder on disk), to show how easily it can be incorporated with current solution, registering factory for `ICrawlerClient` depend on URI supplying with required instance type by matching URI (https://github.com/antyrama/Pinger or files://c:/Windows).

# Links
* [ASP.NET Core](https://github.com/aspnet/Home)
* [Swagger](https://swagger.io)
* [Hangfire](https://www.hangfire.io/)
* [Async Streams](https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/generate-consume-asynchronous-stream)
* [Polly](https://github.com/App-vNext/Polly)
* [HttpClient performance](https://www.stevejgordon.co.uk/using-httpcompletionoption-responseheadersread-to-improve-httpclient-performance-dotnet)
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function;

public class HttpPostBody
{
    private readonly ILogger _logger;

    public HttpPostBody(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<HttpPostBody>();
    }

    [Function("httppost")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req,
        [Microsoft.Azure.Functions.Worker.Http.FromBody] Person person)
    {
        _logger.LogInformation($"C# HTTP POST trigger function processed a request for url {req.Body}");

        if (string.IsNullOrEmpty(person.Name) | string.IsNullOrEmpty(person.Age.ToString()) | person.Age == 0)
        {
            _logger.LogInformation("C# HTTP POST trigger function processed a request with no name/age provided.");
            return new BadRequestObjectResult("Please provide both name and age in the request body.");
        }

        var returnValue = $"Hello, {person.Name}! You are {person.Age} years old.";

        _logger.LogInformation($"C# HTTP POST trigger function processed a request for {person.Name} who is {person.Age} years old.");
        return new OkObjectResult(returnValue);
    }
}

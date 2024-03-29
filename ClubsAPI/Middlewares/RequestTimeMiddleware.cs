﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ClubsAPI.Middlewares
{
  public class RequestTimeMiddleware : IMiddleware
  {
    private Stopwatch _stopWatch;
    private readonly ILogger<RequestTimeMiddleware> _logger;
    public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger)
    {
      _stopWatch = new Stopwatch();
      _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
      _stopWatch.Start();
      await next.Invoke(context);
      _stopWatch.Stop();

      var ellapsedMilliseconds = _stopWatch.ElapsedMilliseconds;

      if (ellapsedMilliseconds / 1000 > 4)
      {
        var message = $"Request [{context.Request.Method}] at {context.Request.Path} took {ellapsedMilliseconds} ms";

        _logger.LogInformation(message);
      }
    }
  }
}

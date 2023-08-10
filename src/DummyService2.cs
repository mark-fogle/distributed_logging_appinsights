using System;
using Microsoft.Extensions.Logging;

namespace ApplicationInsightsTest;

public class DummyService2
{
    private readonly ILogger<DummyService2> _logger;

    public DummyService2(ILogger<DummyService2> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void DoSomething()
    {
        _logger.LogInformation("Service 2 doing something");
        _logger.LogInformation("Service 2 complete");
    }
}
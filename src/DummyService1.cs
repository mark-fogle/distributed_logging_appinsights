using System;
using Microsoft.Extensions.Logging;

namespace ApplicationInsightsTest;

public class DummyService1
{
    private readonly DummyService2 _service2;
    private readonly ILogger<DummyService1> _logger;

    public DummyService1(DummyService2 service2, ILogger<DummyService1> logger)
    {
        _service2 = service2 ?? throw new ArgumentNullException(nameof(service2));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void DoSomething()
    {
        _logger.LogInformation("Service 1 doing something");
        _service2.DoSomething();
        _logger.LogInformation("Service 1 complete");
    }
}
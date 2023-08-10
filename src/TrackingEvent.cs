namespace ApplicationInsightsTest;

public abstract class TrackingEvent<TPayload>
{
    public string Subject { get; }
    public abstract string EventType { get; }
    public abstract string Version { get; }

    public TPayload Payload { get; }

    protected TrackingEvent(string subject, TPayload payload)
    {
        Subject = subject;
        Payload = payload;
    }
}
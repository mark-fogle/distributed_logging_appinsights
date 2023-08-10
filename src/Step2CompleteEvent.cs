using System;

namespace ApplicationInsightsTest;

public class Step2CompleteEvent : TrackingEvent<Step2CompletePayload>
{
    public override string EventType => "Step2Completed";
    public override string Version => "1.0";

    public Step2CompleteEvent(string subject, Guid trackingId)
        : base(subject, new Step2CompletePayload(trackingId))
    {
    }
}

public record Step2CompletePayload(Guid TrackingId) : TrackingEventPayload(TrackingId);
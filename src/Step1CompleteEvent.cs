// File:  Step1CompleteEvent.cs
// Author: Mark Fogle
// Company: ActiGraph
// Created: 2023-08-08
// Purpose:

using System;

namespace ApplicationInsightsTest;

public class Step1CompleteEvent : TrackingEvent<Step1CompletePayload>
{
    public override string EventType => "Step1Completed";
    public override string Version => "1.0";

    public Step1CompleteEvent(string subject, Guid trackingId)
        : base(subject, new Step1CompletePayload(trackingId))
    {
    }
}

public record Step1CompletePayload(Guid TrackingId) : TrackingEventPayload(TrackingId);


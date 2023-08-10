using System;

namespace ApplicationInsightsTest;

public record Step1TriggerEventPayload(Guid TrackingId) : TrackingEventPayload(TrackingId);
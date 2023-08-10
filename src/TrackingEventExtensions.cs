using System;
using Azure.Messaging.EventGrid;

namespace ApplicationInsightsTest;

public static class TrackingEventExtensions
{
    public static TPayload FromEventGridQueueMessage<TPayload>(string eventString) 
    {
        var eventGridEvent = EventGridEvent.Parse(BinaryData.FromString(eventString));
        var payload = eventGridEvent.Data.ToObjectFromJson<TPayload>();
        return payload;
    }

    public static EventGridEvent ToEventGridEvent<TPayload>(this TrackingEvent<TPayload> trackingEvent)
    {
        var eventGridEvent = new EventGridEvent(trackingEvent.Subject, trackingEvent.EventType, trackingEvent.Version,
            BinaryData.FromObjectAsJson(trackingEvent.Payload));
        return eventGridEvent;
    }
}
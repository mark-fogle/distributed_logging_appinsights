using System;
using System.Collections.Generic;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;

namespace ApplicationInsightsTest
{
    public class Functions
    {
        private readonly DummyService1 _service1;

        public Functions(DummyService1 service1)
        {
            _service1 = service1 ?? throw new ArgumentNullException(nameof(service1));
        }

        [FunctionName(nameof(Step1))]
        public void Step1([QueueTrigger("step1-queue", Connection = "StorageConnection")] Step1TriggerEventPayload step1TriggerEventPayload,
            [EventGrid(TopicEndpointUri = "EventGridEndpoint", TopicKeySetting = "EventGridKey")] IAsyncCollector<EventGridEvent> eventCollector,
            ILogger log)
        {

            var trackingId = step1TriggerEventPayload.TrackingId;
            var logScope = new Dictionary<string, object> { [nameof(TrackingEventPayload.TrackingId)] = trackingId };
            using var scope = log.BeginScope(logScope);
            log.LogInformation("Step 1 function started");

            _service1.DoSomething();

            log.LogInformation("Step 1 function completed");

            var step1CompleteEvent = new Step1CompleteEvent("Subject", trackingId);
            eventCollector.AddAsync(step1CompleteEvent.ToEventGridEvent());
        }

        [FunctionName(nameof(Step2))]
        public void Step2([QueueTrigger("step2-queue", Connection = "StorageConnection")] string eventGridMessage,
            [EventGrid(TopicEndpointUri = "EventGridEndpoint", TopicKeySetting = "EventGridKey")] IAsyncCollector<EventGridEvent> eventCollector,
            ILogger log)
        {
            var eventPayload = TrackingEventExtensions.FromEventGridQueueMessage<Step1CompletePayload>(eventGridMessage);
            var logScope = new Dictionary<string, object> { [nameof(TrackingEventPayload.TrackingId)] = eventPayload.TrackingId };
            using var scope = log.BeginScope(logScope);
            try
            {
                log.LogInformation("Step 2 function started");

                //Simulate error
                if (eventPayload.TrackingId == Guid.Parse("3b5bd18c-fc76-4cc6-a590-6c21a22fe323"))
                {
                    throw new Exception("TEST ERROR");
                }

                _service1.DoSomething();
            
                log.LogInformation("Step 2 function completed");

                var step2CompleteEvent = new Step2CompleteEvent("Subject", eventPayload.TrackingId);
                eventCollector.AddAsync(step2CompleteEvent.ToEventGridEvent());
            }
            catch (Exception e)
            {
                log.LogError(e, e.Message);
                throw;
            }
        }
    }


}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedTail.WpfLib
{
    public static class EventAggregator
    {
        private class EventSubscriber
        {
            public EventAggregatorDelgate HandleAggregateEvent { get; set; }
        }

        private static readonly Dictionary<int, List<EventSubscriber>> Subscribers = new Dictionary<int, List<EventSubscriber>>();

        public static void Subscribe(int type, EventAggregatorDelgate handler)
        {
            if(!Subscribers.ContainsKey(type))
            {
                Subscribers.Add(type, new List<EventSubscriber>());
            }

            Subscribers[type].Add(new EventSubscriber { HandleAggregateEvent = handler });
        }

        public static void Publish(int type, object data)
        {
            if (!Subscribers.ContainsKey(type))
            {
                Subscribers.Add(type, new List<EventSubscriber>());
            }

            foreach (var subscriber in Subscribers[type])
            {
                subscriber.HandleAggregateEvent(data);
            }
        }
    }


    public delegate void EventAggregatorDelgate(object data);
}

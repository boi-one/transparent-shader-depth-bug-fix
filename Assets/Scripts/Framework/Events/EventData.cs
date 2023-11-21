public class EventData
{
        private EventTypes _eventTypes = EventTypes.NULL;

        public EventTypes GetEventType()
        {
                return _eventTypes;
        }

        public void SetEventType(EventTypes aEventTypes)
        {
                _eventTypes = aEventTypes;
        }
}

using EventBus;

namespace InTime; 

public static class EventExtendMethod {
    [Event(eventAttributeType = EventAttributeType.no)]
    public static T onEvent<T>(this T @event) where T : Event => (T)World.getWorld().getEventBus().onEvent(@event);
}
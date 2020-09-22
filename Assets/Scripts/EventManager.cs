using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager {

    private int maxEventsPerDay;
    public List<DayEvent> possibleEvents;

    public EventManager(int maxEventsPerDay) {
        possibleEvents = new List<DayEvent>();
        this.maxEventsPerDay = maxEventsPerDay;
    }

    public List<DayEvent> GetDayEvents() {
        return possibleEvents;
    }

}

public class DayEvent {
    public string eventTitle;
    public string eventDescription;
    public int eventTaskCode;
    public float eventTaskModifier;
}

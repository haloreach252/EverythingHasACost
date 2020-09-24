using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class EventManager {

    private int maxEventsPerDay;
    public List<DayEvent> possibleEvents;

    public EventManager(int maxEventsPerDay) {
        possibleEvents = new List<DayEvent>();
        this.maxEventsPerDay = maxEventsPerDay;
        GetEvents("DayEvents");
    }

    private void GetEvents(string path) {
        Object[] jsonEvents = Resources.LoadAll(path, typeof(TextAsset));
        foreach (Object jsonEvent in jsonEvents) {
            TextAsset taJson = (TextAsset)jsonEvent;
            string jsonText = taJson.ToString();
            possibleEvents.Add(JsonConvert.DeserializeObject<DayEvent>(jsonText));
        }
    }

    public List<DayEvent> GetDayEvents() {
        List<DayEvent> eventsToday = new List<DayEvent>();
        int amountToday = Random.Range(1, maxEventsPerDay + 1);
        for (int i = 0; i < amountToday; i++) {
            DayEvent e = possibleEvents[Random.Range(0, possibleEvents.Count)];
            if (!eventsToday.Contains(e)) {
                eventsToday.Add(e);
            }
        }
        return eventsToday;
    }

}

[System.Serializable]
public class DayEvent {
    public string eventTitle;
    public string eventDescription;
    public int eventTaskCode;
    public float eventTaskModifier;

    public void DisplayEvent(MessageManager messageManager) {
        messageManager.CreateMessage(eventTitle, eventDescription);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

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
        return possibleEvents;
    }

}

[System.Serializable]
public class DayEvent {
    public string eventTitle;
    public string eventDescription;
    public int eventTaskCode;
    public float eventTaskModifier;
}

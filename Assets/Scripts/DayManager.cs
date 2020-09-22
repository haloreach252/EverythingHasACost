using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayManager : MonoBehaviour {

    /* Task Codes:
     * 0 - Work
     * 1 - Sleep
     * 2 - Eat
     * 3 - Socialize
     */

    /* Slider values:
     * 0 - Money
     * 1 - Sleep
     * 2 - Mental Health
     * 3 - Health
     */

    // Total hours per day, set to 24
    private const int maxHoursPerDay = 24;

    // The event manager handles getting and choosing events for the day
    public EventManager eventManager;
    List<DayEvent> thisDayEvents;

    // Costs
    public Costs costs;

    // Message Handling
    MessageManager messageManager;

    // Hours management
    private int[] hours;
    private int totalHours;
    // Day management
    private int daysPassed;

    // Text references for reference and modification, the index should be the same as the task codes eg: The work text is at pos 0
    public Text[] hourDisplayTexts;

    // Slider references to update the sliders
    public Slider[] statSliders;
    private Text[] statSliderTexts;

    // Stats
    private float money;
    private float mentalHealth;
    private float sleepState;
    private float healthState;

    private SleepState sleepStateEnum;
    private HealthStates healthStateEnum;
    private MentalHealthStates mentalHealthStateEnum;

    private float hourlyWage = 10.00f;

    private void Start() {
        eventManager = new EventManager(3);

        thisDayEvents = new List<DayEvent>();

        messageManager = GetComponent<MessageManager>();

        hours = new int[4] { 0,0,0,0 };

        totalHours = 0;
        daysPassed = 0;

        money = 500f;
        mentalHealth = 70f;
        sleepState = 70f;
        healthState = 70f;

        statSliderTexts = new Text[statSliders.Length];
        for (int i = 0; i < statSliders.Length; i++) {
            statSliderTexts[i] = statSliders[i].gameObject.GetComponentInChildren<Text>();
        }

        UpdateSliders();
    }

    private void UpdateSliders() {
        statSliders[0].value = money;
        statSliders[1].value = sleepState;
        statSliders[2].value = mentalHealth;
        statSliders[3].value = healthState;

        sleepStateEnum = (SleepState)GetSleepEnum();
        mentalHealthStateEnum = (MentalHealthStates)GetMentalHealthEnum();
        healthStateEnum = (HealthStates)GetHealthEnum();

        statSliderTexts[0].text = "Money - $" + money;
        statSliderTexts[1].text = "Sleep - " + sleepStateEnum.ToString();
        statSliderTexts[2].text = "Mental Health - " + mentalHealthStateEnum.ToString();
        statSliderTexts[3].text = "Health - " + healthStateEnum.ToString();
    }

    // This occurs after a day ends at the beginning of the next day
    private void StartDay() {
        daysPassed++;
        totalHours = 0;
        // Get this days events from the event manager
        thisDayEvents = eventManager.GetDayEvents();
    }

    // Adds hours to specific tasks
    public void AddHour(int taskCode) {
        if(totalHours < 24) {
            totalHours++;
            hours[taskCode]++;
            hourDisplayTexts[taskCode].text = hours[taskCode].ToString();
        }
    }

    // Removes hours from specific tasks
    public void RemoveHour(int taskCode) {
        if (totalHours > 0 && hours[taskCode] > 0) {
            totalHours--;
            hours[taskCode]--;
            hourDisplayTexts[taskCode].text = hours[taskCode].ToString();
        }
    }

    public void BeginDay() {
        if(totalHours != maxHoursPerDay) {
            messageManager.CreateMessage("Error!", "Make sure you have a full 24 hours in your schedule");
            return;
        }

        RunDayLogic();
    }

    private void RunDayLogic() {

        WorkCalculations(hours[0]);
        SleepCalculations(hours[1]);
        EatCalculations(hours[2]);
        SocialCalculations(hours[3]);

        for (int i = 0; i < hours.Length; i++) {
            hours[i] = 0;
            hourDisplayTexts[i].text = "0";
        }

        // Run daily costs with events
        if (daysPassed != 0 && daysPassed % 7 == 0) {
            WeekendCalculations();
        }
        if(thisDayEvents.Count > 0) {
            
        }

        // Show messages for events and such


        UpdateSliders();

        if(healthState <= 0) {
            Die();
        }

        // Begin a new day
        StartDay();
    }

    private void WeekendCalculations() {

    }

    private void WorkCalculations(int hours) {
        float statMult = 0.0f;
        if(hours != 0) {
            if((daysPassed % 6 == 0 || daysPassed % 7 == 0) && hours > 3) {
                mentalHealth -= 5 * (hours - 3);
            }
            if(hours <= 6) {
                statMult = 1.0f;
            } else if (hours <= 8) {
                statMult = 1.0f;
                mentalHealth -= 5 * (hours - 6);
            } else {
                statMult = 2.0f;
                mentalHealth -= 5 * (hours - 6);
            }
        }

        money += hours * statMult * hourlyWage;
    }

    private void SleepCalculations(int hours) {
        if(hours <= 4) {
            healthState -= 5;
            mentalHealth -= 10;
        } else if(hours >= 7) {
            healthState += 5;
            mentalHealth += 5;
        }
    }

    private void EatCalculations(int hours) {
        if(hours != 0) 
            money -= hours * costs.mealCost;

        switch (hours) {
            case 0:
                mentalHealth -= 10;
                healthState -= 10;
                break;
            case 1:
                mentalHealth -= 5;
                break;
            case 3:
                healthState += 5;
                mentalHealth += 5;
                break;
            default:
                break;
        }
    }

    private void SocialCalculations(int hours) {
        if(hours != 0) {
            money -= 5 * hours;
            mentalHealth += 5 * hours;
        }
    }

    private void Die() {

    }

	#region Ugly code
    private int GetSleepEnum() {
        if (sleepState <= 20) {
            return 0;
        } else if (sleepState <= 40) {
            return 1;
        } else if(sleepState <= 60) {
            return 2;
        } else if (sleepState <= 80) {
            return 3;
        } else if (sleepState <= 100) {
            return 4;
        } else {
            return 5;
        }
    }

    private int GetHealthEnum() {
        if (healthState == 0) {
            return 0;
        } else if (healthState <= 20) {
            return 1;
        } else if (healthState <= 40) {
            return 2;
        } else if (healthState <= 50) {
            return 3;
        } else if (healthState <= 90) {
            return 4;
        } else {
            return 5;
        }
    }

    private int GetMentalHealthEnum() {
        if (sleepState <= 10) {
            return 0;
        } else if (sleepState <= 30) {
            return 1;
        } else if (sleepState <= 40) {
            return 2;
        } else if (sleepState <= 60) {
            return 3;
        } else if (sleepState <= 70) {
            return 4;
        } else if(mentalHealth <= 90){
            return 5;
        } else {
            return 6;
        }
    }
    #endregion

}

[System.Serializable]
public class Costs {
    public float rentCost = 120.0f;
    public float mealCost = 10.0f;
    public float utilityCost = 35.0f;
    public float carCost = 65.0f;
}

public enum HealthStates {
    Dead,
    Bedridden,
    Sick,
    Unwell,
    Healthy,
    Radiant
}

public enum MentalHealthStates {
    Suicidal,
    Depressed,
    Struggling,
    Decent,
    Normal,
    Happy,
    Ecstatic
}

public enum SleepState {
    Exhaused,
    Tired,
    Sleepy,
    Groggy,
    Regular,
    Rested
}
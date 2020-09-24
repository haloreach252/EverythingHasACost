using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

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

    // The event manager handles getting and choosing events for the day
    private EventManager eventManager;
    private List<DayEvent> thisDayEvents;

    // Message Handling
    private MessageManager messageManager;

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

    // New Costs
    public ScriptableCosts costs;
    public ScriptableJob job;
    public ScriptableSleep sleep;
    public ScriptableSocial social;
    public ScriptableEating eat;

    // Stats
    public float maxMoney;
    public float maxSleep;
    public float maxMental;
    public float maxHealth;

    private float money;
    private float mentalHealth;
    private float sleepState;
    private float healthState;

    private void Start() {
        eventManager = new EventManager(3);

        thisDayEvents = new List<DayEvent>();
        thisDayEvents = eventManager.GetDayEvents();

        messageManager = GetComponent<MessageManager>();

        hours = new int[4] { 0,0,0,0 };

        totalHours = 0;
        daysPassed = 0;

        statSliders[0].maxValue = maxMoney;
        statSliders[1].maxValue = maxSleep;
        statSliders[2].maxValue = maxMental;
        statSliders[3].maxValue = maxHealth;

        money = job.moneyCost * 25;
        mentalHealth = 90f;
        sleepState = 90f;
        healthState = 90f;

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

        statSliderTexts[0].text = "Money - $" + money;
        statSliderTexts[1].text = "Sleep - " + GetSleepString();
        statSliderTexts[2].text = "Mental Health - " + GetMentalString();
        statSliderTexts[3].text = "Health - " + GetHealthString();
    }

    // This occurs after a day ends at the beginning of the next day
    private void StartDay() {
        daysPassed++;
        totalHours = 0;
        // Get this days events from the event manager
        thisDayEvents.Clear();
        thisDayEvents = eventManager.GetDayEvents();

        if(money <= 0) {
            mentalHealth -= 10;
        }
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
            foreach (DayEvent dayEvent in thisDayEvents) {
                dayEvent.DisplayEvent(messageManager);
                switch (dayEvent.eventTaskCode) {
                    case 0:
                        money += dayEvent.eventTaskModifier;
                        break;
                    case 1:
                        mentalHealth += dayEvent.eventTaskModifier;
                        healthState += dayEvent.eventTaskModifier;
                        break;
                    case 2:
                        mentalHealth += dayEvent.eventTaskModifier;
                        healthState += dayEvent.eventTaskModifier;
                        break;
                    case 3:
                        mentalHealth += dayEvent.eventTaskModifier;
                        break;
                    default:
                        break;
                }
            }
        }

        money = Mathf.Clamp(money, -1000, maxMoney);

        if (money >= maxMoney) 
            messageManager.CreateMessage("Max Money!", "You've reached the money limit, consider spending less time working.");

        UpdateSliders();

        if(healthState <= 0) {
            Die();
        }

        // Begin a new day
        StartDay();
    }

    private void WeekendCalculations() {
        money -= costs.carCost + costs.rentCost + costs.utilityCost;
    }

    private void WorkCalculations(int hours) {
        if(hours > 0) {
            if((daysPassed % 6 == 0 || daysPassed % 7 == 0) && hours > 3) {
                mentalHealth -= job.mentalCost * 2 * (hours - 3);
            }

            if(hours > 6 && hours <= 8) {
                mentalHealth -= job.mentalCost * (hours - 6);
            } else if(hours > 8) {
                mentalHealth -= job.mentalCost * (hours - 6);
            }

            if (hours <= 8) {
                money += hours * job.moneyCost;
            } else {
                int overtime = hours - 8;
                money += (overtime * 2 * job.moneyCost) + ((hours - overtime) * job.moneyCost);
            }
        }
    }

    private void SleepCalculations(int hours) {
        if(hours <= 4) {
            healthState -= sleep.healthCost;
            mentalHealth -= sleep.mentalCost;
        } else if(hours >= 7) {
            healthState += sleep.healthGain;
            mentalHealth += sleep.mentalGain;
        }
    }

    private void EatCalculations(int hours) {
        if(hours > 0) 
            money -= hours * eat.moneyCost;

        switch (hours) {
            case 0:
                mentalHealth -= eat.mentalCost * 2;
                healthState -= eat.healthCost * 2;
                break;
            case 1:
                mentalHealth -= eat.mentalCost;
                break;
            case 3:
                healthState += eat.mentalGain;
                mentalHealth += eat.healthGain;
                break;
            default:
                break;
        }
    }

    private void SocialCalculations(int hours) {
        if(hours > 0) {
            money -= social.moneyCost * hours;
            mentalHealth += social.mentalGain * hours;
            healthState += social.healthGain;
            healthState -= social.healthCost;
        }
    }

    private void Die() {

    }

	#region Ugly code
    private string GetSleepString() {
        if (sleepState <= 20) {
            return "Exhausted";
        } else if (sleepState <= 40) {
            return "Tired";
        } else if(sleepState <= 60) {
            return "Sleepy";
        } else if (sleepState <= 80) {
            return "Groggy";
        } else if (sleepState <= 100) {
            return "Rested";
        } else {
            return "Well Rested";
        }
    }

    private string GetHealthString() {
        if (healthState == 0) {
            return "Dead";
        } else if (healthState <= 20) {
            return "Bedridden";
        } else if (healthState <= 40) {
            return "Sick";
        } else if (healthState <= 50) {
            return "Unwell";
        } else if (healthState <= 90) {
            return "Healthy";
        } else {
            return "Radiant";
        }
    }

    private string GetMentalString() {
        if (mentalHealth <= 10) {
            return "Suicidal";
        } else if (mentalHealth <= 30) {
            return "Depressed";
        } else if (mentalHealth <= 40) {
            return "Struggling";
        } else if (mentalHealth <= 60) {
            return "Decent";
        } else if (mentalHealth <= 70) {
            return "Normal";
        } else if(mentalHealth <= 90){
            return "Happy";
        } else {
            return "Ecstatic";
        }
    }
    #endregion

}
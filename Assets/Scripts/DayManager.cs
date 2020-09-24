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

	#region Variables
	// Important references and declarations
	private EventManager eventManager;
    private List<DayEvent> thisDayEvents;
    private MessageManager messageManager;

    // The prefab for the game over message box
    [FoldoutGroup("Important Prefabs")]
    public GameObject diePrefab;
    [FoldoutGroup("Important Prefabs")]
    public GameObject newDayButton;

    // Hours management
    private int[] hours;

    // New implementation for weekends
    private int[] hoursWeekday;
    private int[] hoursWeekend;

    private int totalHours;

    // Day management
    private int daysPassed;
    private int daysPassedSinceEvent;
    private bool isWeekend;

    // These track the schedule, but not the specified hours
    private bool scheduleLocked;
    // New implementation for weekends
    private bool scheduleLockedWeekday;
    private bool scheduleLockedWeekend;

    // Texts for the statuses
    [FoldoutGroup("Status Texts")]
    public Text moneyText;
    [FoldoutGroup("Status Texts")]
    public Text sleepText;
    [FoldoutGroup("Status Texts")]
    public Text mentalText;
    [FoldoutGroup("Status Texts")]
    public Text healthText;

    // Cost texts on the sticky note
    [FoldoutGroup("Info Texts")]
    public Text weeklyCostText;
    [FoldoutGroup("Info Texts")]
    public Text mealCostText;
    [FoldoutGroup("Info Texts")]
    public Text socialCostText;
    [FoldoutGroup("Info Texts")]
    public Text hourlyWageText;

    // Text references for reference and modification, the index should be the same as the task codes eg: The work text is at pos 0
    [FoldoutGroup("Hour Texts")]
    public Text[] hourDisplayTexts;

    // Days passed text
    [FoldoutGroup("Day Texts")]
    public Text daysPassedText;

    // Scriptables to calculate stuff
    [FoldoutGroup("Scriptables")]
    public ScriptableCosts costs;
    [FoldoutGroup("Scriptables")]
    public ScriptableJob job;
    [FoldoutGroup("Scriptables")]
    public ScriptableSleep sleep;
    [FoldoutGroup("Scriptables")]
    public ScriptableSocial social;
    [FoldoutGroup("Scriptables")]
    public ScriptableEating eat;

    // Stats

    // Max values as set in inspector
    [FoldoutGroup("Max values")]
    public float maxMoney;
    [FoldoutGroup("Max values")]
    public float maxSleep;
    [FoldoutGroup("Max values")]
    public float maxMental;
    [FoldoutGroup("Max values")]
    public float maxHealth;

    // These are the starting values as set in the inspector
    [FoldoutGroup("Starting values")]
    public float startMoney;
    [FoldoutGroup("Starting values")]
    public float startSleep;
    [FoldoutGroup("Starting values")]
    public float startMental;
    [FoldoutGroup("Starting values")]
    public float startHealth;

    // These track the stats
    private float money;
    private float mentalHealth;
    private float sleepState;
    private float healthState;

    // This holds the weekly payment so I dont have to calculate every time
    private float weeklyPayment;
	#endregion

	#region Accessors
    public float Money { get { return money; } set { money = value; } }
    public float Sleep { get { return sleepState; } set { sleepState = value; } }
    public float Mental { get { return mentalHealth; } set { mentalHealth = value; } }
    public float Health { get { return healthState; } set { healthState = value; } }
    #endregion

    #region Setup
    // Will have to rewrite this to implement weekends
    private void Start() {
        eventManager = new EventManager(2);
        thisDayEvents = new List<DayEvent>();

        messageManager = GetComponent<MessageManager>();

        hours = new int[4] { 0,0,0,0 };

        totalHours = 0;
        daysPassed = 0;
        daysPassedSinceEvent = 0;

        money = startMoney;
        mentalHealth = startMental;
        sleepState = startSleep;
        healthState = startHealth;

        scheduleLocked = false;

        weeklyPayment = costs.carCost + costs.rentCost + costs.utilityCost;

        weeklyCostText.text = "Weekly - $" + weeklyPayment;
        mealCostText.text = "Meal Cost - $" + eat.moneyCost;
        socialCostText.text = "Social Cost - $" + social.moneyCost;
        hourlyWageText.text = "Hourly - $" + job.moneyCost;

        UpdateText();
    }
	#endregion

	#region Upgrades
	public void UpgradeHouse(ScriptableCosts newCosts) {
        costs = newCosts;
        weeklyPayment = costs.carCost + costs.rentCost + costs.utilityCost;
        weeklyCostText.text = "Weekly - $" + weeklyPayment;
    }

    public void UpgradeJob(ScriptableJob newJob) {
        job = newJob;
        hourlyWageText.text = "Hourly - $" + job.moneyCost;
    }

    public void UpgradeSocial(ScriptableSocial newSocial) {
        social = newSocial;
        socialCostText.text = "Social Cost - $" + social.moneyCost;
    }

    public void UpgradeSleep(ScriptableSleep newSleep) {
        sleep = newSleep;
    }

    public void UpgradeEat(ScriptableEating newEat) {
        eat = newEat;
        mealCostText.text = "Meal Cost = $" + eat.moneyCost;
    }
	#endregion

	#region Boosts
	#endregion

	private void CheckValues() {
        money = Mathf.Clamp(money, -1000, maxMoney);
        if (money >= maxMoney)
            messageManager.CreateMessage("Max Money!", "You've reached the money limit, consider spending less time working.");

        mentalHealth = Mathf.Clamp(mentalHealth, 0, maxMental);
        healthState = Mathf.Clamp(healthState, 0, maxHealth);
        sleepState = Mathf.Clamp(sleepState, 0, maxSleep);
    }

    private void UpdateText() {
        moneyText.text = "Money - $" + money;
        sleepText.text = "Sleep - " + GetSleepString();
        mentalText.text = "Mental Health - " + GetMentalString();
        healthText.text = "Health - " + GetHealthString();
    }

    // This occurs after a day ends at the beginning of the next day
    private void StartDay() {
        CheckValues();

        // Die if your health is 0
        if (healthState <= 0) {
            Die(0);
        }

        // 1/400 chance of suicide IF SUICIDAL
        if(mentalHealth <= 10) {
            float chance = Random.Range(0f, 4f);
            if(chance <= 0.01) {
                Die(1);
            }
        }

        daysPassed++;
        totalHours = 0;

        // Is it a weekend?
        if((daysPassed + 1) % 6 == 0 || (daysPassed + 1) % 7 == 0) {
            isWeekend = true;
        }

        daysPassedText.text = "Day " + daysPassed;
        // Get this days events from the event manager
        thisDayEvents.Clear();
        if (CanEventHappenToday()) {
            thisDayEvents = eventManager.GetDayEvents();
        }

        if(money <= 0) {
            mentalHealth -= 10;
        }

        UpdateText();
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

    // Sets whether the schedule is locked or not
    public void SetSchedule() {
        scheduleLocked = !scheduleLocked;
    }

    // Starts the day
    public void BeginDay() {
        messageManager.ClearMessages();
        RunDayLogic();
    }

    // Checks if an event can happen today
    private bool CanEventHappenToday() {
        float chance = Random.Range(0f, 1f);
        chance += daysPassedSinceEvent * 0.05f;
        if(chance <= 0.90) {
            daysPassedSinceEvent++;
            return false;
        } else {
            daysPassedSinceEvent = 0;
            return true;
        }
    }

    private void RunDayLogic() {
        if (thisDayEvents.Count > 0) {
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

        WorkCalculations(hours[0]);
        SleepCalculations(hours[1]);
        EatCalculations(hours[2]);
        SocialCalculations(hours[3]);

        if (!scheduleLocked) {
            for (int i = 0; i < hours.Length; i++) {
                hours[i] = 0;
                hourDisplayTexts[i].text = "0";
            }
        }

        // If end of week do end of week tasks
        if ((daysPassed + 1) % 7 == 0) {
            money -= weeklyPayment;
        }

        mentalHealth += costs.mentalBenefit;
        healthState += costs.healthBenefit;

        // Begin a new day
        StartDay();
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

    // Reason, 0 is health, 1 is mental health
    private void Die(int reason) {
        newDayButton.SetActive(false);
        string title;
        string desc;
        messageManager.CreateCustomMessage("You died!", "Your health went to 0 and you died.", diePrefab);
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
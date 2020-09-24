using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

    private int totalHoursWeekday;
    private int totalHoursWeekend;

    // Day management
    private int daysPassed;
    private int daysPassedSinceEvent;
    private bool isWeekend;
    private bool weekendSchedule;

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
    [FoldoutGroup("Hour Texts")]
    public Text weekendDisplayText;

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
    [FoldoutGroup("Scriptables")]
    public ScriptableCosts bgChange;
    [FoldoutGroup("Scriptables")]
    public Image bgImage;
    [FoldoutGroup("Scriptables")]
    public Image bedImage;
    [FoldoutGroup("Scriptables")]
    public Sprite bgSprite;

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

    public GameObject infoPanel;
    public Image lockButton;
    public List<DeathMessage> deathMessages;

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

        hoursWeekday = new int[4] { 0, 0, 0, 0 };
        hoursWeekend = new int[4] { 0, 0, 0, 0 };

        totalHoursWeekday = 0;
        totalHoursWeekend = 0;

        totalHours = 0;
        daysPassed = 0;
        daysPassedSinceEvent = 0;

        money = startMoney;
        mentalHealth = startMental;
        sleepState = startSleep;
        healthState = startHealth;

        scheduleLockedWeekday = false;
        scheduleLockedWeekend = false;

        scheduleLocked = false;

        weeklyPayment = costs.carCost + costs.rentCost + costs.utilityCost;

        weeklyCostText.text = "Weekly - $" + weeklyPayment;
        mealCostText.text = "Meal Cost - $" + eat.moneyCost;
        socialCostText.text = "Social Cost - $" + social.moneyCost;
        hourlyWageText.text = "Hourly - $" + job.moneyCost;

        UpdateText();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.I)) {
            infoPanel.SetActive(!infoPanel.activeSelf);
        }
    }
    #endregion

    #region Upgrades
    public void UpgradeHouse(ScriptableCosts newCosts) {
        costs = newCosts;
        weeklyPayment = costs.carCost + costs.rentCost + costs.utilityCost;
        weeklyCostText.text = "Weekly - $" + weeklyPayment;

        if(costs = bgChange) {
            bgImage.sprite = bgSprite;
            bedImage.gameObject.SetActive(true);
        }
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
            Die();
        }

        // 1/100000 chance of suicide IF SUICIDAL
        if(mentalHealth <= 10) {
            float chance = Random.Range(0f, 100f);
            if(chance <= 0.01) {
                Die();
            }
        }

        daysPassed++;

        if (!scheduleLockedWeekday) {
            totalHoursWeekday = 0;
        }

        if (!scheduleLockedWeekend) {
            totalHoursWeekend = 0;
        }

        // Is it a weekend?
        if((daysPassed + 1) % 6 == 0 || (daysPassed + 1) % 7 == 0) {
            isWeekend = true;
        } else {
            isWeekend = false;
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

        if(money <= -900) {
            messageManager.CreateMessage("Money!", "You may want to start finding ways to build up some money, you're getting dangerously low");
        }

        UpdateText();
    }

	#region Button Stuff
	public void CloseIntro(GameObject o) {
        o.SetActive(false);
    }

    // Adds hours to specific tasks
    public void AddHour(int taskCode) {
        if (weekendSchedule) {
            if (totalHoursWeekend < 24) {
                totalHoursWeekend++;
                hoursWeekend[taskCode]++;
                hourDisplayTexts[taskCode].text = hoursWeekend[taskCode].ToString();
            }
        } else {
            if(totalHoursWeekday < 24) {
                totalHoursWeekday++;
                hoursWeekday[taskCode]++;
                hourDisplayTexts[taskCode].text = hoursWeekday[taskCode].ToString();
            }
        }
    }

    // Removes hours from specific tasks
    public void RemoveHour(int taskCode) {
        if (weekendSchedule) {
            if (totalHoursWeekend > 0 && hoursWeekend[taskCode] > 0) {
                totalHoursWeekend--;
                hoursWeekend[taskCode]--;
                hourDisplayTexts[taskCode].text = hoursWeekend[taskCode].ToString();
            }
        } else {
            if (totalHoursWeekday > 0 && hoursWeekday[taskCode] > 0) {
                totalHoursWeekday--;
                hoursWeekday[taskCode]--;
                hourDisplayTexts[taskCode].text = hoursWeekday[taskCode].ToString();
            }
        }
    }

    public void WeekendSchedule(bool isWeekendS) {
        weekendSchedule = isWeekendS;

        if (weekendSchedule) {
            for (int i = 0; i < hoursWeekend.Length; i++) {
                hourDisplayTexts[i].text = hoursWeekend[i].ToString();
            }
            weekendDisplayText.gameObject.SetActive(true);
        } else {
            for (int i = 0; i < hoursWeekday.Length; i++) {
                hourDisplayTexts[i].text = hoursWeekday[i].ToString();
            }
            weekendDisplayText.gameObject.SetActive(false);
        }

        SetButtonColor(weekendSchedule);
    }

    private void SetButtonColor(bool weekendSchedule) {
        if (weekendSchedule) {
            if (scheduleLockedWeekend) {
                lockButton.color = Color.red;
            } else {
                lockButton.color = Color.white;
            }
        } else {
            if (scheduleLockedWeekday) {
                lockButton.color = Color.red;
            } else {
                lockButton.color = Color.white;
            }
        }
    }

    // Sets whether the schedule is locked or not
    public void SetSchedule() {
        if (weekendSchedule) {
            scheduleLockedWeekend = !scheduleLockedWeekend;
        } else {
            scheduleLockedWeekday = !scheduleLockedWeekday;
        }

        SetButtonColor(weekendSchedule);
    }

    // Starts the day
    public void BeginDay() {
        messageManager.ClearMessages();
        RunDayLogic();
    }
	#endregion
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

        if (isWeekend) {
            WorkCalculations(hoursWeekend[0]);
            SleepCalculations(hoursWeekend[1]);
            EatCalculations(hoursWeekend[2]);
            SocialCalculations(hoursWeekend[3]);
        } else {
            WorkCalculations(hoursWeekday[0]);
            SleepCalculations(hoursWeekday[1]);
            EatCalculations(hoursWeekday[2]);
            SocialCalculations(hoursWeekday[3]);
        }

        if (isWeekend) {
            if (!scheduleLockedWeekend) {
                for (int i = 0; i < hoursWeekend.Length; i++) {
                    hoursWeekend[i] = 0;
                    hourDisplayTexts[i].text = "0";
                }
            }
        } else {
            if (!scheduleLockedWeekday) {
                for (int i = 0; i < hoursWeekday.Length; i++) {
                    hoursWeekday[i] = 0;
                    hourDisplayTexts[i].text = "0";
                }
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
            if(isWeekend && hours > 3) {
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
        DeathMessage message = deathMessages[Random.Range(0,deathMessages.Count)];
        newDayButton.SetActive(false);
        messageManager.CreateCustomMessage(message.title, message.description, diePrefab);
        PlayerPrefs.SetInt("HighScore", daysPassed);
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

[System.Serializable]
public class DeathMessage {
    public string title;
    public string description;
}
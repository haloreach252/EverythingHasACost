using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayManager : MonoBehaviour {

    // Total hours per day, set to 24
    private const int maxHoursPerDay = 24;

    /* Task Codes:
     * 0 - Work
     * 1 - Sleep
     * 2 - Eat
     * 3 - Socialize
     */

    // Hours management
    private int hoursWork;
    private int hoursSleep;
    private int hoursEat;
    private int hoursSocialize;

    private int[] hours;

    private int totalHours = 0;

    // Text references for reference and modification, the index should be the same as the task codes eg: The work text is at pos 0
    public Text[] displayTexts;

    // Stats
    private float money;
    private float mentalHealth;
    private float sleepState;
    private float healthState;

    private float hourlyWage = 12.50f;

    private void Start() {
        hours = new int[4] { 0,0,0,0 };

        money = 0f;
        mentalHealth = 100f;
        sleepState = 100f;
        healthState = 100f;
    }

    // Adds hours to specific tasks, 
    public void AddHour(int taskCode) {
        if(totalHours < 24) {
            totalHours++;
            hours[taskCode]++;
            displayTexts[taskCode].text = hours[taskCode].ToString();
        } else return;
    }

    // Removes hours from specific tasks, 
    public void RemoveHour(int taskCode) {
        if (totalHours > 0 && hours[taskCode] > 0) {
            totalHours--;
            hours[taskCode]--;
            displayTexts[taskCode].text = hours[taskCode].ToString();
        } else return;
    }

    public void BeginDay() {
        // TODO: make error messaging system
        if(totalHours != maxHoursPerDay)
            return;

        totalHours = 0;
        RunDayLogic();
    }

    // TODO: Daily cost (dailty cost of each stat like money, health, etc)
    private void RunDayLogic() {

        float statMult = 1.0f;

        int workHours = hours[0];
        int sleepHours = hours[1];
        int eatHours = hours[2];
        int socialHours = hours[3];

        // First we do work, this dictates money
        if(workHours > 9) 
            statMult = 1.5f;
        else if(workHours < 1)
            statMult = 0;

        money += workHours * statMult * hourlyWage;

        statMult = 1.0f;

        // Then we do sleep, which dictates health, mental health, and sleep
        if (sleepHours < 1)
            statMult = -1.0f;
        else if (sleepHours > 1 && sleepHours < 8)
            statMult = 0.5f;



        // Then eat, health

        // Then social, mental health
    }

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
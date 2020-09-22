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

    private void Start() {
        hours = new int[4] { 0,0,0,0 };
    }


    private void Update() {
        
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
        if (totalHours > 0) {
            totalHours--;
            hours[taskCode]--;
            displayTexts[taskCode].text = hours[taskCode].ToString();
        } else return;
    }

    public void BeginDay() {
        // TODO: make error messaging system
        if(totalHours != maxHoursPerDay) {
            return;
        }

        totalHours = 0;

        // TODO: Game logic like reducing health and mental health with less sleep and eat, etc
    }

}

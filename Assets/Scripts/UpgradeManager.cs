using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class UpgradeManager : MonoBehaviour {

	#region Variables
	// Important references
	private DayManager dayManager;
	private MessageManager messageManager;

	// Whether the upgrade stuff is open or not
	private bool upgradeOpen;

	// References to the panels to open/close
	[FoldoutGroup("Main Panels")]
	public GameObject upgradePanel;
	[FoldoutGroup("Main Panels")]
	public GameObject mainGamePanel;

	// My solution for the fuckery
	[FoldoutGroup("Scriptable Lists")]
	public List<ScriptableCosts> scriptableCosts;
	[FoldoutGroup("Scriptable Lists")]
	public List<ScriptableJob> scriptableJobs;
	[FoldoutGroup("Scriptable Lists")]
	public List<ScriptableSocial> scriptableSocial;
	[FoldoutGroup("Scriptable Lists")]
	public List<ScriptableEating> scriptableEating;
	[FoldoutGroup("Scriptable Lists")]
	public List<ScriptableSleep> scriptableSleep;

	// Tracks current tab to display correct info
	private GameObject openTab;
	#endregion

	private void Start() {
		dayManager = GetComponent<DayManager>();
		messageManager = GetComponent<MessageManager>();

		upgradeOpen = false;
	}

	#region Button Methods
	// Toggles
	public void ToggleUpgrades() {
		upgradeOpen = !upgradeOpen;
		upgradePanel.SetActive(upgradeOpen);
		mainGamePanel.SetActive(upgradeOpen);
	}

	public void ToggleTab(GameObject tabToOpen) {
		openTab.SetActive(false);
		tabToOpen.SetActive(true);
		openTab = tabToOpen;
	}

	// Purchase
	public void PurchaseUpgrade(ScriptableUpgrade upgrade) {
		ApplyUpgrade(upgrade);
	}
	#endregion

	#region Logic
	private void ApplyUpgrade(ScriptableUpgrade upgrade) {
		// TODO: Cost checking and tier checking
		bool canUpgrade = false;

		if(upgrade.costTaskCode == 0) {
			canUpgrade = dayManager.Money >= upgrade.cost;
			if (canUpgrade) dayManager.Money -= upgrade.cost;
		} else if(upgrade.costTaskCode == 1) {
			canUpgrade = dayManager.Mental >= upgrade.cost;
			if (canUpgrade) dayManager.Mental -= upgrade.cost;
		} else if(upgrade.costTaskCode == 2) {
			canUpgrade = dayManager.Health >= upgrade.cost;
			if (canUpgrade) dayManager.Health -= upgrade.cost;
		} else {
			canUpgrade = dayManager.Sleep >= upgrade.cost;
			if (canUpgrade) dayManager.Sleep -= upgrade.cost;
		}

		if (canUpgrade) {
			switch (upgrade.upgradeCode) {
				case 0:
					if (scriptableCosts[upgrade.upgradeIndex] != null) {
						dayManager.UpgradeHouse(scriptableCosts[upgrade.upgradeIndex]);
					}
					break;
				case 1:
					if (scriptableJobs[upgrade.upgradeIndex] != null) {
						dayManager.UpgradeJob(scriptableJobs[upgrade.upgradeIndex]);
					}
					break;
				case 2:
					if (scriptableSocial[upgrade.upgradeIndex] != null) {
						dayManager.UpgradeSocial(scriptableSocial[upgrade.upgradeIndex]);
					}
					break;
				case 3:
					if (scriptableEating[upgrade.upgradeIndex] != null) {
						dayManager.UpgradeEat(scriptableEating[upgrade.upgradeIndex]);
					}
					break;
				case 4:
					if (scriptableSleep[upgrade.upgradeIndex] != null) {
						dayManager.UpgradeSleep(scriptableSleep[upgrade.upgradeIndex]);
					}
					break;
				default:
					break;
			}
		}
	}
	#endregion

	/* Upgrade codes:
	 * 0 - Cost
	 * 1 - Job
	 * 2 - Social
	 * 3 - Food
	 * 4 - Sleep
	 */

}

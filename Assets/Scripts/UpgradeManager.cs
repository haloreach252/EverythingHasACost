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

	}
	#endregion

}

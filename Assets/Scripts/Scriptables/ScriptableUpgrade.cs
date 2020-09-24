using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu()]
public class ScriptableUpgrade : ScriptableObject {

	[FoldoutGroup("Upgrade Settings")]
	public int upgradeTier;
	[FoldoutGroup("Upgrade Settings")]
	public float cost;
	[FoldoutGroup("Upgrade Settings")]
	public int upgradeCode;
	[FoldoutGroup("Upgrade Settings")]
	public int upgradeIndex;

	[InfoBox("Upgrade codes are as follows:\n0 - House\n1 - Job\n2 - Social\n3 - Food\n4 - Sleep")]
	public string na;
}

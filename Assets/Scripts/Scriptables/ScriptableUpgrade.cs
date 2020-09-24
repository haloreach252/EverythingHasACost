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
	public int costTaskCode;
}

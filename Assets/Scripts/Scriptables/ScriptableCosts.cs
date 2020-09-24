using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu()]
public class ScriptableCosts : ScriptableObject {

	[BoxGroup("Weekly Costs")]
	public float rentCost;
	[BoxGroup("Weekly Costs")]
	public float carCost;
	[BoxGroup("Weekly Costs")]
	public float utilityCost;

	[BoxGroup("Benefits")]
	public float healthBenefit;
	[BoxGroup("Benefits")]
	public float mentalBenefit;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(), System.Serializable]
public class ScriptableJob : ScriptableObject {
	public float moneyCost;
	public float healthCost;
	public float mentalCost;

	public float healthGain;
	public float mentalGain;

}

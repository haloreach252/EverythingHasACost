using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(), System.Serializable]
public class ScriptableSleep : ScriptableObject {
	public float healthCost;
	public float mentalCost;

	public float healthGain;
	public float mentalGain;

}

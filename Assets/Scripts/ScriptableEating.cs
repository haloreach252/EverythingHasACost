﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ScriptableEating : ScriptableObject {
	public float moneyCost;
	public float healthCost;
	public float mentalCost;

	public float healthGain;
	public float mentalGain;
}

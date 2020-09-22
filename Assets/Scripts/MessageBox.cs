﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour {

	public Text titleText;
	public Text descText;

	public void CloseBox() {
		Destroy(gameObject);
	}

}

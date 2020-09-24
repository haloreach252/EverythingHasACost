using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	public Text highScoreText;

	public void StartGame() {
		SceneManager.LoadScene(1);
	}

	public void Start() {
		int highscore = PlayerPrefs.GetInt("HighScore");
		highScoreText.text = "High score: " + highscore;
	}

}

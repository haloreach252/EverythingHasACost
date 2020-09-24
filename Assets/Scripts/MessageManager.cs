using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour {

    public GameObject messageBoxPrefab;
    public GameObject canvas;

    public void CreateMessage(string title, string desc) {
        GameObject mb = Instantiate(messageBoxPrefab, canvas.transform);
        MessageBox messageBox = mb.GetComponent<MessageBox>();
        messageBox.titleText.text = title;
        messageBox.descText.text = desc;
    }
}
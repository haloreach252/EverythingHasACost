using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageManager : MonoBehaviour {

    public GameObject messageBoxPrefab;
    public GameObject canvas;
    private List<GameObject> messages = new List<GameObject>();

    public void CreateMessage(string title, string desc) {
        GameObject mb = Instantiate(messageBoxPrefab, canvas.transform);
        MessageBox messageBox = mb.GetComponent<MessageBox>();
        messageBox.titleText.text = title;
        messageBox.descText.text = desc;
        messages.Add(mb);
    }

    public void CreateCustomMessage(string title, string desc, GameObject customMessage) {
        GameObject cm = Instantiate(customMessage, canvas.transform);
        MessageBox mb = cm.GetComponent<MessageBox>();
        mb.titleText.text = title;
        mb.descText.text = desc;
    }

    public void ClearMessages() {
        foreach (GameObject go in messages) {
            Destroy(go);
        }
    }
}
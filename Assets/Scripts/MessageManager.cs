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

    public void CreateMessage(MessageBase messageBase) {
        GameObject mb = Instantiate(messageBoxPrefab, canvas.transform);
        MessageBox messageBox = mb.GetComponent<MessageBox>();
        messageBox.titleText.text = messageBase.messageTitle;
        messageBox.descText.text = messageBase.messageDescription;
    }

}

[System.Serializable]
public class MessageBase {
    public string messageTitle;
    public string messageDescription;
    public bool isError;
}
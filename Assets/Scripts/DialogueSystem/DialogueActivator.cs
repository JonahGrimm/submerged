using System;
using UnityEngine;

public class DialogueActivator : MonoBehaviour
{
    public DialogueText textObject;
    private Collider col;
    public bool activate;           //This variable is reserved for cutscenes and debugging primarily

    private void Start()
    {
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        if (activate)
        {
            activate = false;
            SendText();            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SendText();
            col.enabled = false;
        }
    }

    private void SendText()
    {
        //Disgusting. But it works... I'd definitely revise this for bigger projects/multiplayer games
        DialogueBox db = GameObject.Find("DialogueBox").GetComponent<DialogueBox>();
        db.ReadText(textObject);
    }
}


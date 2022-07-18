using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChatMessageController : MonoBehaviour
{
    public TMP_Text from;
    public TMP_Text message;


    public void Clear() 
    {
        from.text = "";
        message.text = "";
    }

    public void SetMessage(string fromText, string mesasgeText) 
    {
        from.text = fromText;
        message.text = mesasgeText;
    }
}

using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Utils;

public class ChatMessageController : MonoBehaviour
{
    public TMP_Text from;
    public TMP_Text message;


    public void Clear() 
    {
        from.text = "";
        message.text = "";
    }

    public void SetMessage(string fromText, string mesasgeText, long messageTime) 
    {
        //string timeFormatted = MissingLanguageFunctions.TimeStampToDateTime(messageTime);
        from.text = fromText;
        message.text = mesasgeText;// + ", " + timeFormatted;
    }
}

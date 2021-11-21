using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomDebugger : MonoBehaviour
{
    [SerializeField]
    public int MaxLength = 100000;

    public Text text;
    private void OnEnable()
    {

        Application.logMessageReceived += Application_logMessageReceived;
    }


    private void OnDisable()
    {
        Application.logMessageReceived -= Application_logMessageReceived;
    }

    private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        
        text.text += $"[{Enum.GetName(typeof(LogType), type)}] {condition}\n";
        if(text.text.Length > MaxLength)
        {
            text.text = text.text.Substring(text.text.IndexOf('\n'));
        }
    }
}

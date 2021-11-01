using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomDebugger : MonoBehaviour
{
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
    }
}

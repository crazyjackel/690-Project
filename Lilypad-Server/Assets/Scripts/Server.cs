using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Server : MonoBehaviour
{
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (GUILayout.Button("Start Server")) NetworkManager.Singleton.StartServer();
        GUILayout.EndArea();
    }
}

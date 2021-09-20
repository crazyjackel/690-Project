using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Server : MonoBehaviour
{
    void Start()
    {
        NetworkManager.Singleton.StartServer();    
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class Startup : MonoBehaviour
{
    void Start()
    {
        NetworkManager.Singleton.StartServer();    
    }
}

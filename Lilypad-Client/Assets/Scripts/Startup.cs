using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class Startup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.Singleton.StartClient();
    }
}

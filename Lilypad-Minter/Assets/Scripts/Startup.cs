using MLAPI;
using MLAPI.Transports.UNET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Startup : MonoBehaviour
{
    public UNetTransport transport;
    void Start()
    {
        ServerNetworkPortal.OnStart.AddListener(() =>
        {
            transport.ServerListenPort = 7778;
            NetworkManager.Singleton.StartServer();
        });
    }
}
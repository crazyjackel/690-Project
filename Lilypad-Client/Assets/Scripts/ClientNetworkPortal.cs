using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System.Text;

public class ClientNetworkPortal : MonoBehaviour
{

    public static ClientNetworkPortal Singleton;
    void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        SharedNetworkPortal.Singleton.OnConnectionFinished += OnConnectionFinished;

        Connect();
    }

    private void OnConnectionFinished(bool result)
    {
        if (result)
        {
            Debug.Log("Connection Successful");
        }
    }

    public void Connect()
    {
        var Validation = "Valid";
        byte[] payload = Encoding.UTF8.GetBytes(Validation);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payload;

        NetworkManager.Singleton.StartClient();
    }
}

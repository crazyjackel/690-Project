using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using System.Text;
using MLAPI.Messaging;

/// <summary>
/// Portal for Starting the Server
/// Tells what the Server should do based on Events
/// </summary>
public class ServerNetworkPortal : MonoBehaviour
{

    public static ServerNetworkPortal Singleton;
    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;
    }
    private void ConnectionApprovalCallback(byte[] data, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string validator = Encoding.UTF8.GetString(data);

        Debug.Log($"Connection Approval: {validator}");

        bool status = false;
        if(validator == "Valid")
        {
            status = true;
        }

        callback(false, 0, status, null, null);

        SharedNetworkPortal.Singleton.ServerToClientConnectResult(clientId, status);
    }
}

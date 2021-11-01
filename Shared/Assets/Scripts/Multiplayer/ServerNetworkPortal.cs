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
    /// <summary>
    /// Opens when Callbacks done Subscribing, Closes Never
    /// </summary>
    public static QueuedGate OnStart = new QueuedGate();
    /// <summary>
    /// Opens when Portal Opens, Closes Never
    /// </summary>
    public static QueuedGate OnInit = new QueuedGate();

    public ValidationToken token;
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
        NetworkManager.Singleton.OnServerStarted += ServerStartCallback;
        OnStart.Open();
    }

    private void ServerStartCallback()
    {
        OnInit.Open();
    }

    private void ConnectionApprovalCallback(byte[] data, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string validator = Encoding.UTF8.GetString(data);

        Debug.Log($"Connection Approval: {validator}");

        bool status = false;
        if(token != null && validator == token.HashedVersion)
        {
            status = true;
        }

        callback(false, 0, status, null, null);

        SharedNetworkPortal.Singleton.ServerToClientConnectResult(clientId, status);
    }
}

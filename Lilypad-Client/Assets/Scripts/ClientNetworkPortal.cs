using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System.Text;
using System;

public class ClientNetworkPortal : MonoBehaviour
{
    public static ClientNetworkPortal Singleton;

    /// <summary>
    /// Called When Timing out after attempting to Connect
    /// </summary>
    public event Action OnTimeout;
    /// <summary>
    /// Connection After Network Portal Acknowledges Connection
    /// </summary>
    public event Action OnConnect;
    /// <summary>
    /// Disconnection After Network Portal Acknowledges Disconnect
    /// </summary>
    public event Action OnDisconnect;

    public SharedNetworkPortal.NetworkRole? connectedRole;

    private bool m_isConnected = false;
    public bool isConnected { get { return m_isConnected; } }

    public ValidationToken token;
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
        SharedNetworkPortal.Singleton.OnClientDisconnected += OnClientDisconnected;
        SharedNetworkPortal.Singleton.OnDisconnect += OnDisconnectFinished;
    }

    private void OnClientDisconnected(ulong obj)
    {
        if (!m_isConnected)
        {
            OnTimeout?.Invoke();
            Debug.Log("Timeout");
        }
    }

    private void OnDisconnectFinished()
    {
        Debug.Log("Disconnected From Server");

        m_isConnected = false;
        connectedRole = null;

        OnDisconnect?.Invoke();
    }

    private void OnConnectionFinished(bool result, SharedNetworkPortal.NetworkRole Role)
    {
        Debug.Log($"Connection Finished {result} {Role}");

        m_isConnected = result; 
        connectedRole = Role;

        OnConnect?.Invoke();
    }

    public void Connect()
    {
        byte[] payload = Encoding.UTF8.GetBytes(token.HashedVersion);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payload;

        NetworkManager.Singleton.StartClient();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System.Text;
using System;

public class ClientNetworkPortal : MonoBehaviour, IClient, IProvider
{
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

    private SharedNetworkPortal _portal;

    private bool m_isConnected = false;
    public bool isConnected { get { return m_isConnected; } }

    public ValidationToken token;

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

    public void Connect(NetworkManager _network)
    {
        byte[] payload = Encoding.UTF8.GetBytes(token.HashedVersion);
        _network.NetworkConfig.ConnectionData = payload;
        _network.StartClient();
    }

    private void OnEnable()
    {
        DepInjector.AddProvider(this);
    }
    private void OnDisable()
    {
        DepInjector.Remove(this);
    }
    private void OnDestroy()
    {
        DepInjector.Remove(this);
    }
    public void NewProviderAvailable(IProvider newProvider)
    {
        if (DepInjector.MapProvider(newProvider, ref _portal))
        {
            _portal.OnConnectionFinished += OnConnectionFinished;
            _portal.OnClientDisconnected += OnClientDisconnected;
            _portal.OnDisconnect += OnDisconnectFinished;
        }
    }

    public void ProviderRemoved(IProvider removeProvider)
    {
        DepInjector.UnmapProvider(removeProvider, ref _portal);
    }

    public void NewProviderFullyInstalled(IProvider newProvider)
    {

    }
}

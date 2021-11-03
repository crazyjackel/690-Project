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
public class ServerNetworkPortal : MonoBehaviour, IClient, IProvider
{

    public ValidationToken token;

    private SharedNetworkPortal _portal;

    private NetworkManager _network;

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

    private void ConnectionApprovalCallback(byte[] data, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string validator = Encoding.UTF8.GetString(data);

        Debug.Log($"Connection Approval: {validator}");

        bool status = false;
        if (token != null && validator == token.HashedVersion && _portal != null)
        {
            status = true;
        }

        callback(false, 0, status, null, null);

        _portal?.ServerToClientConnectResult(clientId, status);
    }

    public void NewProviderAvailable(IProvider newProvider)
    {
        DepInjector.MapProvider(newProvider, ref _portal);
        if(DepInjector.MapProvider<NetworkManagerProvider, NetworkManager>(newProvider, ref _network))
        {
            _network.ConnectionApprovalCallback += ConnectionApprovalCallback;
        }
    }

    public void ProviderRemoved(IProvider removeProvider)
    {
        DepInjector.UnmapProvider(removeProvider, ref _portal);
        DepInjector.UnmapProvider<NetworkManagerProvider, NetworkManager>(removeProvider, ref _network);
    }

    public void NewProviderFullyInstalled(IProvider newProvider)
    {
    }
}

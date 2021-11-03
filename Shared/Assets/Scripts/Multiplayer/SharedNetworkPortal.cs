using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using MLAPI.Messaging;
using MLAPI.Serialization.Pooled;
using System.Linq;
using MLAPI.Transports;


public class SharedNetworkPortal : MonoBehaviour, IClient, IProvider
{
    public enum NetworkRole : ushort
    {
        Host,
        Mint,
        Client
    }

    public NetworkRole netRole;
    /// <summary>
    /// Called Whenever Server Acknowledges that Specific Client has connected
    /// </summary>
    public event Action<bool, NetworkRole> OnConnectionFinished;
    /// <summary>
    /// Called Whenever an Unfamiliar Client Connects to the Server
    /// </summary>
    public event Action<ulong> OnClientConnected;
    /// <summary>
    /// Called Whenever another Client Disconnects
    /// </summary>
    public event Action<ulong> OnClientDisconnected;
    /// <summary>
    /// Called when you are disconnected from a server.
    /// </summary>
    public event Action OnDisconnect;

    private NetworkManager _network;


    void Start()
    {
        RegisterClientMessageHandlers();
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

    private void ClientDisconnected(ulong obj)
    {
        if (obj == _network.LocalClientId)
        {
            OnDisconnect?.Invoke();
        }
        else
        {
            OnClientDisconnected?.Invoke(obj);
        }
    }

    private void ClientConnected(ulong obj)
    {
        if (obj == _network.LocalClientId) return;
        OnClientConnected?.Invoke(obj);
    }

    private void RegisterClientMessageHandlers()
    {
        CustomMessagingManager.RegisterNamedMessageHandler("STC_ConnectionFinished", (id, stream) =>
        {
            Debug.Log("Received STC_ConnectionFinished");
            using (var reader = PooledNetworkReader.Get(stream))
            {
                bool connectedSuccessfully = reader.ReadBool();
                NetworkRole ServerType = (NetworkRole)reader.ReadUInt16();

                OnConnectionFinished?.Invoke(connectedSuccessfully, ServerType);
            }
        });
    }

    public void ServerToClientConnectResult(ulong netId, bool status)
    {
        if (_network == null) return;

        if (_network.ConnectedClients.Keys.Contains(netId))
        {
            using (var buffer = PooledNetworkBuffer.Get())
            {
                using (var writer = PooledNetworkWriter.Get(buffer))
                {
                    writer.WriteBool(status);
                    writer.WriteUInt16((ushort)netRole);
                    CustomMessagingManager.SendNamedMessage("STC_ConnectionFinished", netId, buffer);
                }
            }
        }
    }

    public void NewProviderAvailable(IProvider newProvider)
    {
        if(DepInjector.MapProvider<NetworkManagerProvider, NetworkManager>(newProvider,ref _network))
        {
            _network.OnClientConnectedCallback += ClientConnected;
            _network.OnClientDisconnectCallback += ClientDisconnected;
        }
    }

    public void ProviderRemoved(IProvider removeProvider)
    {
        DepInjector.UnmapProvider<NetworkManagerProvider, NetworkManager>(removeProvider, ref _network);
    }

    public void NewProviderFullyInstalled(IProvider newProvider)
    {

    }
}

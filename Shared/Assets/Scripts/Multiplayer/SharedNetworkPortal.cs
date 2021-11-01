using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System;
using MLAPI.Messaging;
using MLAPI.Serialization.Pooled;
using System.Linq;
using MLAPI.Transports;


public class SharedNetworkPortal : MonoBehaviour
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

    public static SharedNetworkPortal Singleton;
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
        NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;

        RegisterClientMessageHandlers();
    }

    private void ClientDisconnected(ulong obj)
    {
        if (obj == NetworkManager.Singleton.LocalClientId)
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
        if (obj == NetworkManager.Singleton.LocalClientId) return;
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
        if (NetworkManager.Singleton.ConnectedClients.Keys.Contains(netId))
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

}

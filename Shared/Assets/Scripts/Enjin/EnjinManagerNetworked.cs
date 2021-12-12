using Enjin.SDK.Core;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnjinManagerNetworked : NetworkBehaviour, IClient, IProvider
{
    private EnjinManager _manager;

    private SharedNetworkPortal _portal;

    [ServerRpc(RequireOwnership = false)]
    public void PingServerRpc(ulong clientID)
    {
        Debug.Log("Received Ping");

        ClientRpcParams para = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };
        PongClientRpc(para);
    }

    [ServerRpc(RequireOwnership = false)]
    public void LoginServerRpc(ulong clientId, string username)
    {
        Debug.Log($"Received Login Request from {clientId}:{username}");

        if(_manager is ServerEnjinManager _serverManager)
        {
            _serverManager
                .Login(
                    clientId,
                    username,
                    () =>
                    {
                        ClientRpcParams para = new ClientRpcParams
                        {
                            Send = new ClientRpcSendParams
                            {
                                TargetClientIds = new ulong[] { clientId }
                            }
                        };
                        LoginSuccessClientRpc(para);
                    },
                    (x) =>
                    {
                        ClientRpcParams para = new ClientRpcParams
                        {
                            Send = new ClientRpcSendParams
                            {
                                TargetClientIds = new ulong[] { clientId }
                            }
                        };
                        ErrorClientRpc($"Error Logging in. Code: {x}", para);
                    });
        }
    }

    [ClientRpc]
    public void PongClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Pong");
    }

    [ClientRpc]
    public void LoginSuccessClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Login Successful");
    }

    [ClientRpc]
    public void ErrorClientRpc(string errorDetails, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log(errorDetails);   
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
        DepInjector.MapProvider(newProvider, ref _manager);
        DepInjector.MapProvider(newProvider, ref _portal);
    }

    public void NewProviderFullyInstalled(IProvider newProvider)
    {
    }

    public void ProviderRemoved(IProvider removeProvider)
    {
        DepInjector.UnmapProvider(removeProvider, ref _manager);
        DepInjector.UnmapProvider(removeProvider, ref _portal);
    }
}

/*
public class EnjinManagerNetworked : NetworkBehaviour
{
    public static EnjinManagerNetworked Singleton;
    /// <summary>
    /// Initialization that occurs Once Server Starts
    /// </summary>
    public static QueuedGate<EnjinManagerNetworked> OnInit = new QueuedGate<EnjinManagerNetworked>();
    public static QueuedGate OnDeInit = new QueuedGate();

    NetworkVariable<string> variable;

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

    public void OnEnable()
    {
        ServerNetworkPortal.OnInit.AddListener(() =>
        {
            OnInit.Open(this);
        });
    }

    public void OnDisable()
    {
        Debug.Log("DeInitNetwork");
        OnDeInit.Open();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestTokenServerRPC(ulong clientID)
    {
        if (EnjinManager.Singleton.TryGetAccessToken(out string token))
        {

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { clientID }
                }
            };

            NotifyTokenUpdateClientRPC(token, clientRpcParams);
        }
    }

    [ClientRpc]
    public void NotifyTokenUpdateClientRPC(string newToken, ClientRpcParams RPCparams = default)
    {
        if (NetworkManager.Singleton.IsServer) return;
        EnjinManager.Singleton.OnAccessTokenUpdate?.Invoke(newToken);
    }
}*/

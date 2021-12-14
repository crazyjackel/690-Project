using Enjin.SDK.Core;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class EnjinManagerNetworked : NetworkBehaviour, IClient, IProvider
{
    private EnjinManager _manager;

    private SharedNetworkPortal _portal;

    #region Server RPCs
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
    public void MintItemServerRpc(ulong clientId, string itemId, int value)
    {
        Debug.Log($"Received Mint Request from {clientId}:{itemId}");
        if (_manager is ServerEnjinManager _serverManager)
        {
            _serverManager.MintOneItemToAddress(clientId, itemId, value,
                () =>
                {
                    ClientRpcParams para = new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new ulong[] { clientId }
                        }
                    };
                    MintSuccessClientRpc(para);
                },
                (x, y) =>
                {
                    ClientRpcParams para = new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new ulong[] { clientId }
                        }
                    };
                    ErrorClientRpc($"Error Registering. Code: {x}; Details : {y}", para);
                });
        }    
    }

    [ServerRpc(RequireOwnership = false)]
    public void RegisterServerRpc(ulong clientId, string username)
    {
        Debug.Log($"Received Register Request from {clientId}:{username}");

        if (_manager is ServerEnjinManager _serverManager)
        {
            _serverManager
                .Register(
                    username,
                    x =>
                    {
                        ClientRpcParams para = new ClientRpcParams
                        {
                            Send = new ClientRpcSendParams
                            {
                                TargetClientIds = new ulong[] { clientId }
                            }
                        };
                        CreateSuccessClientRpc(x, para);
                    },
                    (x, y) =>
                    {
                        ClientRpcParams para = new ClientRpcParams
                        {
                            Send = new ClientRpcSendParams
                            {
                                TargetClientIds = new ulong[] { clientId }
                            }
                        };
                        ErrorClientRpc($"Error Registering. Code: {x}; Details : {y}", para);
                    });
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void LoginServerRpc(ulong clientId, string username)
    {
        Debug.Log($"Received Login Request from {clientId}:{username}");

        if (_manager is ServerEnjinManager _serverManager)
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
                    (x, y) =>
                    {
                        ClientRpcParams para = new ClientRpcParams
                        {
                            Send = new ClientRpcSendParams
                            {
                                TargetClientIds = new ulong[] { clientId }
                            }
                        };
                        ErrorClientRpc($"Error Logging in. Code: {x}; Details : {y}", para);
                    });
        }
    }
    #endregion

    #region Client RPCs

    public event Action OnPong;
    [ClientRpc]
    public void PongClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Pong");
        OnPong?.Invoke();
    }

    public event Action OnLogicSuccess;
    [ClientRpc]
    public void LoginSuccessClientRpc(ClientRpcParams clientRpcParams = default)
    {
        Debug.Log("Login Successful");
        OnLogicSuccess?.Invoke();
    }

    public event Action<string> OnCreateSuccess;
    [ClientRpc]
    public void CreateSuccessClientRpc(string QrCode, ClientRpcParams clientRpcParams = default)
    {
        OnCreateSuccess?.Invoke(QrCode);
    }

    public event Action OnMintSuccess;
    [ClientRpc]
    public void MintSuccessClientRpc (ClientRpcParams clientRpcParams = default)
    {
        OnMintSuccess?.Invoke();
    }

    public event Action<string> OnError;
    [ClientRpc]
    public void ErrorClientRpc(string errorDetails, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log(errorDetails);
        OnError?.Invoke(errorDetails);
    }
    #endregion

    #region Management
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
    #endregion
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

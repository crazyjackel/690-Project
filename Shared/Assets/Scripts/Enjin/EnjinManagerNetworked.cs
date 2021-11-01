using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

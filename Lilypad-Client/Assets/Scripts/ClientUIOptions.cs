using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;
using MLAPI.Transports.UNET;
using System;
using Enjin.SDK.Core;
using ENJ = Enjin.SDK.Core;

public class ClientUIOptions : MonoBehaviour
{
    [SerializeField]
    private UNetTransport Transport;

    [SerializeField]
    private PlayerWallet playerWallet;

    public void ConnectToServer()
    {
        Transport.ConnectPort = 7777;
        ClientNetworkPortal.Singleton.Connect();
    }

    public void ConnectToMinter()
    {
        Transport.ConnectPort = 7778;
        ClientNetworkPortal.Singleton.Connect();
    }

    public void GetAccessToken()
    {
        EnjinManagerNetworked.Singleton.RequestTokenServerRPC(NetworkManager.Singleton.LocalClientId);
    }

    public void Start()
    {
        ClientNetworkPortal.Singleton.OnConnect += OnConnect;
    }

    private void OnConnect()
    {
        EnjinManager.Singleton.OnAccessTokenUpdate += OnNewToken;
    }

    private void OnNewToken(string obj)
    {
        playerWallet.StartSession(obj);
    }
}

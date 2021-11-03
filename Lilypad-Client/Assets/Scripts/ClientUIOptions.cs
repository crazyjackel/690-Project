using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;
using MLAPI.Transports.UNET;
using System;
using Enjin.SDK.Core;
using ENJ = Enjin.SDK.Core;

public class ClientUIOptions : MonoBehaviour, IClient
{
    [SerializeField]
    private UNetTransport Transport;

    [SerializeField]
    private PlayerWallet playerWallet;

    ClientNetworkPortal _clientPortal;
    NetworkManager _network;
    EnjinManagerNetworked _enjinNetworked;
    EnjinManager _enjin;

    public void ConnectToServer()
    {
        if (_clientPortal == null || _network == null) return;

        Transport.ConnectPort = 7777;
        _clientPortal.Connect(_network);
    }
    public void ConnectToMinter()
    {
        if (_clientPortal == null || _network == null) return;

        Transport.ConnectPort = 7778;
        _clientPortal.Connect(_network);
    }
    public void GetAccessToken()
    {
        if (_enjinNetworked == null) return;

        _enjinNetworked.RequestTokenServerRPC(NetworkManager.Singleton.LocalClientId);
    }
    private void OnNewToken(string obj)
    {
        playerWallet.StartSession(obj);
    }

    private void OnEnable()
    {
        DepInjector.AddClient(this);
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
        DepInjector.MapProvider<NetworkManagerProvider, NetworkManager>(newProvider, ref _network);
        DepInjector.MapProvider(newProvider, ref _clientPortal);
        DepInjector.MapProvider(newProvider, ref _enjinNetworked);
        if(DepInjector.MapProvider(newProvider, ref _enjin))
        {
            _enjin.OnAccessTokenUpdate += OnNewToken;
        }
    }

    public void ProviderRemoved(IProvider removeProvider)
    {
        DepInjector.UnmapProvider<NetworkManagerProvider, NetworkManager>(removeProvider, ref _network);
        DepInjector.UnmapProvider(removeProvider, ref _clientPortal);
        DepInjector.UnmapProvider(removeProvider, ref _enjinNetworked);
        DepInjector.UnmapProvider(removeProvider, ref _enjin);
    }

    public void NewProviderFullyInstalled(IProvider newProvider)
    {
    }
}

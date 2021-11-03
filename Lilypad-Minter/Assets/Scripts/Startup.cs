using MLAPI;
using MLAPI.Transports.UNET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Startup : MonoBehaviour, IClient, Initializeable
{
    public UNetTransport transport;

    private NetworkManager _network;
    private ServerNetworkPortal _portal;

    private bool initialized = false;

    public void OnEnable()
    {
        DepInjector.AddClient(this);
    }
    public void OnDisable()
    {
        DepInjector.Remove(this);
    }
    public void OnDestroy()
    {
        DepInjector.Remove(this);
    }
    public bool CanInitialize()
    {
        return _network != null && _portal != null;
    }
    public void TryInitialize(bool requireIntiailization = false)
    {
        initialized = initialized && !requireIntiailization;
        if (!initialized && CanInitialize())
        {
            Initialize();
        }
    }
    public void Initialize()
    {
        initialized = true;
        transport.ServerListenPort = 7778;
        _network.StartServer();
    }

    public void NewProviderAvailable(IProvider newProvider)
    {
        DepInjector.MapProvider<NetworkManagerProvider, NetworkManager>(newProvider, ref _network);
        DepInjector.MapProvider(newProvider, ref _portal);
    }

    public void NewProviderFullyInstalled(IProvider newProvider)
    {
        TryInitialize(false);
    }

    public void ProviderRemoved(IProvider removeProvider)
    {
        DepInjector.UnmapProvider<NetworkManagerProvider, NetworkManager>(removeProvider, ref _network);
        DepInjector.UnmapProvider(removeProvider, ref _portal);
    }
}
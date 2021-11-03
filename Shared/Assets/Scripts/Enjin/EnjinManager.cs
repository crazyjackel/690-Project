using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enjin.SDK.Core;
using System.Threading;
using System;
using System.Threading.Tasks;
using MLAPI;


public class EnjinManager : MonoBehaviour, IClient, IProvider, Initializeable
{
    private EnjinManagerNetworked _enjinManagerNetworked;

    private NetworkManager _network;

    public ServerWallet serverWallet;

    public Action<string> OnAccessTokenUpdate;

    public CoroutineTimer RefreshPlatformTimer;

    private bool initialized = false;

    public bool TryGetAccessToken(out string token)
    {
        if (_network != null && _network.IsServer && serverWallet != null)
        {
            token = serverWallet.GetAccessToken();
            return true;
        }
        token = "";
        return false;
    }


    public void UpdatePlatform(EnjinManagerNetworked _EnjinNetwork)
    {
        if (_network == null || !_network.IsServer) return;
        Debug.Log("Updating Platform... ");
        serverWallet?.StartPlatform();
        _EnjinNetwork.NotifyTokenUpdateClientRPC(serverWallet.GetAccessToken());
    }

    private void StartUpdateCycle(EnjinManagerNetworked _EnjinNetwork)
    {
        if (_network == null || !_network.IsServer) return;
        RefreshPlatformTimer = new CoroutineTimer(
                    (float)TimeSpan.FromSeconds(10).TotalSeconds,
                    true,
                    () => UpdatePlatform(_EnjinNetwork),
                    true);
        StartCoroutine(RefreshPlatformTimer.Start());
    }

    public void OnEnable()
    {
        DepInjector.AddProvider(this);
    }

    public void OnDisable()
    {
        DepInjector.Remove(this);
    }

    public void OnDestroy()
    {
        DepInjector.Remove(this);
    }

    public void NewProviderAvailable(IProvider newProvider)
    {
        DepInjector.MapProvider<NetworkManagerProvider, NetworkManager>(newProvider, ref _network);
        DepInjector.MapProvider(newProvider, ref _enjinManagerNetworked);
    }
    public void NewProviderFullyInstalled(IProvider newProvider)
    {
        TryInitialize(false);
    }
    public void ProviderRemoved(IProvider removeProvider)
    {
        DepInjector.UnmapProvider<NetworkManagerProvider, NetworkManager>(removeProvider, ref _network);
        if (DepInjector.UnmapProvider(removeProvider, ref _enjinManagerNetworked))
        {
            RefreshPlatformTimer?.Stop();
        }
    }
    public bool CanInitialize()
    {
        return _enjinManagerNetworked != null && _network != null;
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
        StartUpdateCycle(_enjinManagerNetworked);
    }
}

/*
public class EnjinManager : MonoBehaviour
{
    public static EnjinManager Singleton;
    /// <summary>
    /// Opens When Start, Close Never
    /// </summary>
    public static QueuedGate<EnjinManager> OnStart = new QueuedGate<EnjinManager>();
    /// <summary>
    /// Opens when Networked Counterpart Opens, Closes when Networked Counterpart Closes or Disconnects
    /// </summary>
    public static QueuedGate OnInit = new QueuedGate();

    private bool IsStartup = false;

    public ServerWallet serverWallet;

    public Action<string> OnAccessTokenUpdate;

    public CoroutineTimer RefreshPlatformTimer;

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

    public void Start()
    {
        EnjinManagerNetworked.OnInit.AddListener(network =>
        {
            OnInit.Open();
        });
        OnStart.Open(this);
    }


    public bool TryGetAccessToken(out string token)
    {
        if(NetworkManager.Singleton.IsServer && serverWallet != null)
        {
            token = serverWallet.GetAccessToken();
            return true;
        }
        token = "";
        return false;
    }

    public void OnEnable()
    {
        OnInit.AddListener(() =>
        {
            if (NetworkManager.Singleton.IsServer && !IsStartup)
            {
                
                IsStartup = true;
            }
        });
    }

    public void UpdatePlatform()
    {
        Debug.Log("Updating Platform... ");
        EnjinManagerNetworked.OnInit.AddListener(network =>
        {
            serverWallet?.StartPlatform();
            if (NetworkManager.Singleton.IsServer)
            {
                network.NotifyTokenUpdateClientRPC(serverWallet.GetAccessToken());
            }
        });
    }

    private void OnDisable()
    {
        IsStartup = false;
    }
}
*/
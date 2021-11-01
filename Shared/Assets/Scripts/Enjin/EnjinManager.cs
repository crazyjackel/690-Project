using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enjin.SDK.Core;
using System.Threading;
using System;
using System.Threading.Tasks;
using MLAPI;

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
                RefreshPlatformTimer = new CoroutineTimer(
                    (float)TimeSpan.FromSeconds(10).TotalSeconds,
                    true,
                    UpdatePlatform,
                    true);
                StartCoroutine(RefreshPlatformTimer.Start());
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enjin.SDK.Core
{
    public class ServerEnjinManager : EnjinManager
    {
        public ServerWallet serverWallet;
        public CoroutineTimer RefreshPlatformTimer;

        public override bool TryGetAccessToken(out string token)
        {
            if (_network != null && serverWallet != null)
            {
                token = serverWallet.GetAccessToken();
                return true;
            }
            token = "";
            return false;
        }
        //Server-Only
        public void UpdatePlatform(EnjinManagerNetworked _EnjinNetwork)
        {
            if (_network == null || !_network.IsServer) return;
            Debug.Log("Updating Platform... ");
            serverWallet?.StartPlatform();
            _EnjinNetwork.NotifyTokenUpdateClientRPC(serverWallet.GetAccessToken());
        }
        //Server-Only
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
        public override void OnNetworkLost()
        {
            RefreshPlatformTimer?.Stop();
        }
        public override void Init()
        {
            StartUpdateCycle(_enjinManagerNetworked);
        }
    }
}
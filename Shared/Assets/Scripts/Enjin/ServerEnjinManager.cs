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

        //Server-Only
        public void UpdatePlatform(EnjinManagerNetworked _EnjinNetwork)
        {
            if (_network == null || !_network.IsServer) return;
            Debug.Log("Updating Platform... ");
            _EnjinNetwork.AccessToken.Value = serverWallet.StartPlatform();
        }
        //Server-Only
        private void StartUpdateCycle(EnjinManagerNetworked _EnjinNetwork)
        {
            if (_network == null || !_network.IsServer) return;
            RefreshPlatformTimer = new CoroutineTimer(
                        (float)TimeSpan.FromHours(10).TotalSeconds,
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
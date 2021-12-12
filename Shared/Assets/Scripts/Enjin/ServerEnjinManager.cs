using Enjin.SDK.DataTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Enjin.SDK.Core
{
    [Serializable] public class ClientToWalletDict : SerializableDictionary<ulong, PlayerWallet> { }
    public class ServerEnjinManager : EnjinManager
    {
        public ServerWallet serverWallet;
        public CoroutineTimer RefreshPlatformTimer;
        public ClientToWalletDict playerWallets = new ClientToWalletDict();

        //Server-Only
        public void UpdatePlatform()
        {
            if (_network == null || !_network.IsServer) return;
            Debug.Log("Updating Platform... ");
            serverWallet.StartPlatform();
        }
        //Server-Only
        private void StartUpdateCycle()
        {
            if (_network == null || !_network.IsServer) return;
            RefreshPlatformTimer = new CoroutineTimer(
                        (float)TimeSpan.FromHours(10).TotalSeconds,
                        true,
                        () => UpdatePlatform(),
                        true);
            StartCoroutine(RefreshPlatformTimer.Start());
        }
        public override void OnNetworkLost()
        {
            RefreshPlatformTimer?.Stop();
        }
        public override void Init()
        {
            var clients = _network.ConnectedClientsList;
            _network.OnClientConnectedCallback += x => playerWallets.Add(x, new PlayerWallet());
            _network.OnClientDisconnectCallback += x => playerWallets.Remove(x);
            foreach(var client in clients)
            {
                playerWallets.Add(client.ClientId, new PlayerWallet());
            }
            StartUpdateCycle();
            
        }

        public void Login(ulong clientId, string user, Action OnLogin, Action<ResponseCodes> OnFailure)
        {
            Task.Run(async () =>
            {
                var resp = await EnjinAsync.GetUser(user);
                if(resp.code == ResponseCodes.SUCCESS)
                {
                    User player = resp.response;
                    var identity = player.identities.FirstOrDefault(x => x.app.id == serverWallet.APPID);
                    if (identity == null) 
                    {
                        OnFailure(ResponseCodes.INTERNAL);
                        return;
                    }
                    if(playerWallets.TryGetValue(clientId, out PlayerWallet wallet))
                    {
                        wallet.LoadFromIdentity(identity);
                        OnLogin();
                        return;
                    }
                    else
                    {
                        OnFailure(ResponseCodes.INTERNAL);
                        return;
                    }
                }
                else
                {
                    OnFailure(resp.code);
                }
            });
        }
    }
}
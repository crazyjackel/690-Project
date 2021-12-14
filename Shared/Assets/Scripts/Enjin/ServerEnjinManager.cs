using Enjin.SDK.DataTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

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
        protected override void DeInit()
        {
            playerWallets.Clear();
            RefreshPlatformTimer?.Stop();
        }
        protected override void Init()
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

  
        public void MintOneItemToAddress(ulong clientID, string itemId, int value, Action OnMintSuccessful, Action<ResponseCodes, string> OnFailure)
        {
            if (playerWallets.TryGetValue(clientID, out PlayerWallet wallet) && wallet.IsLoaded) {
                EnjinAsync.MintFungibleToken(serverWallet.AppID, serverWallet.DeveloperID, wallet.PlayerETHAddress, itemId, value).Subscribe(resp =>
                {
                    if (resp.code == ResponseCodes.SUCCESS)
                    {
                        OnMintSuccessful?.Invoke();
                    }
                    else
                    {
                        OnFailure?.Invoke(resp.code, "Bad Response");
                    }
                });
            }
        }

        public void Register(string user, Action<string> OnRegister, Action<ResponseCodes,string> OnFailure)
        {

            EnjinAsync.GetUserObservable(user).Subscribe(resp =>
            {
                //Case 1: User not Registered.
                if(resp.code == ResponseCodes.NOTFOUND)
                {
                    //Okay this doesn't make sense much, but first request does not include Linking Code QR, so we have to get User after creating
                    EnjinAsync.CreateUser(user).Subscribe(resp =>
                    {
                        EnjinAsync.GetUserObservable(user).Subscribe(resp =>
                        {
                            if (resp.code == ResponseCodes.SUCCESS)
                            {
                                Register_Link(resp, OnRegister, OnFailure);
                            }
                            else
                            {
                                OnFailure?.Invoke(resp.code, "Bad Response");
                            }
                        });
                    });
                }
                //Case 2: User not Linked.
                else if(resp.code == ResponseCodes.SUCCESS)
                {
                    Register_Link(resp, OnRegister, OnFailure, "User Already Registered");
                }
                //Case 3: Bad Send
                else
                {
                    OnFailure?.Invoke(resp.code, "Bad Response");
                }
            });
        }
        private void Register_Link(EnjinAsync.Response<User> resp, Action<string> OnRegister, Action<ResponseCodes, string> OnFailure, string FailMessage = "Link Code Not Found")
        {
            if (resp == null) return;
            User player = resp.response;
            var identity = player.identities.FirstOrDefault(x => x.app.id == serverWallet.AppID);
            if (identity == null)
            {
                OnFailure?.Invoke(ResponseCodes.INTERNAL, "Identity Not Found");
                return;
            }

            if (identity.linkingCodeQr != "")
            {
                OnRegister?.Invoke(identity.linkingCodeQr);
            }
            else
            {
                OnFailure?.Invoke(ResponseCodes.INTERNAL, FailMessage);
            }
        }


        public void Login(ulong clientId, string user, Action OnLogin, Action<ResponseCodes, string> OnFailure)
        {
            EnjinAsync.GetUserObservable(user).Subscribe(resp =>
            {
                if (resp.code == ResponseCodes.SUCCESS)
                {
                    User player = resp.response;
                    var identity = player.identities.FirstOrDefault(x => x.app.id == serverWallet.AppID);
                    if (identity == null)
                    {
                        OnFailure.Invoke(ResponseCodes.INTERNAL, "Identity Not Found");
                        return;
                    }
                    if (playerWallets.TryGetValue(clientId, out PlayerWallet wallet))
                    {
                        wallet.LoadFromIdentity(identity);
                        OnLogin.Invoke();
                        return;
                    }
                    else
                    {
                        OnFailure.Invoke(ResponseCodes.INTERNAL, "Wallet Not Found on Server");
                        return;
                    }
                }
                else
                {
                    OnFailure.Invoke(resp.code, "Bad Response");
                }
            });
        }
    }
}
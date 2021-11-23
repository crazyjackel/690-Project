using Enjin.SDK.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Enjin.SDK.Core {
    public class ClientEnjinManager : EnjinManager
    {
        public PlayerWallet playerWallet;

        public IObservable<Texture2D> Login(string userName)
        {
            if (AccessToken.Value != null)
            {
                playerWallet.StartSession(AccessToken.Value);
                playerWallet.SetUser(userName);
                return playerWallet.RequestLinkQRCode();
            }
            return Observable.Empty<Texture2D>();
        }

        public bool CreateUser(string userName)
        {
            if (AccessToken.Value != null)
            {
                playerWallet.StartSession(AccessToken.Value);
                playerWallet.SetUser(userName);
                return playerWallet.CreateUser();
            }
            return false;
        }
        public void UpdateAccessToken()
        {
            if (_enjinManagerNetworked == null) return;
            Debug.Log("Requesting Update");
            _enjinManagerNetworked.RequestTokenServerRPC(_network.LocalClientId);
        }
    }
}